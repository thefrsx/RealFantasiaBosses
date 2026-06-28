using System;
using System.Collections.Generic;
using Server;
using Server.Custom.Gumps;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Combat
{
    // PROTÓTIPO de combate "por turnos ao vivo" (Modelo A) — coletor de participantes 1 mob vs N jogadores.
    //
    //  - Turnos por LADO: "SUA VEZ" (jogadores livres, mob Frozen) alterna com "TURNO DO INIMIGO".
    //  - 1 AÇÃO PRINCIPAL por turno (atacar/poção/pergaminho/passar/defender/fugir) + ORÇAMENTO DE MOVIMENTO
    //    = clamp(3 + Dex/50, 3, 6) tiles (simétrico mob+jogador). Agir congela o jogador (encerra a vez dele).
    //  - Casts (pergaminhos) não congelam na hora: a passagem p/ o turno do inimigo espera o cast terminar.
    //  - Ações via HUD (TurnHudGump) + seletor de itens (TurnActionPickerGump). Magia do spellbook: TODO.
    //
    // Reusa primitivas nativas (Frozen, NextCombatTime, EventSink.Movement, ResistanceMod). Sem editar Server.dll.
    public sealed class TurnEncounter
    {
        public static readonly TimeSpan PlayerTurnTimeout = TimeSpan.FromSeconds(15.0);
        private static readonly TimeSpan TickRate = TimeSpan.FromMilliseconds(250);
        private const long FarFuture = 600000;
        private const long MobStepDelayMs = 200;   // alinhado ao tick: 1 passo por tick (perseguição ágil)
        private const long MobLeadInMs = 2000;     // "beat": o mob encara e espera antes de agir
        private const long EnemyTurnTotalMs = 5000; // TETO do turno do inimigo (quando não alcança o alvo)
        private const long PostSwingMs = 1000;      // após o golpe (acerto/erro), passa o turno
        private const int LeashTiles = 20;
        private const double FleeChance = 0.55;
        private const double MutedSpeed = 100.0;
        private const double MobDefenseDuringPlayerTurn = 30.0;
        private const int DefendBonus = 25; // +resist em todos os tipos ao Defender

        private static readonly Dictionary<PlayerMobile, TurnEncounter> _registry = new();

        public static void Initialize()
        {
            EventSink.Movement += OnGlobalMovement;
        }

        private static void OnGlobalMovement(MovementEventArgs e)
        {
            if (e.Blocked || e.Mobile is not PlayerMobile p)
            {
                return;
            }

            if (_registry.TryGetValue(p, out var enc))
            {
                enc.OnPlayerMoveAttempt(p, e);
            }
        }

        private static int MoveBudget(Mobile m) => Math.Clamp(3 + m.Dex / 50, 3, 6);

        private readonly BaseCreature _mob;
        private readonly double _mobWrestlingBase;
        private readonly List<PlayerMobile> _players = new();
        private readonly HashSet<PlayerMobile> _acted = new();
        private readonly Dictionary<PlayerMobile, int> _stepsUsed = new();
        private readonly Dictionary<PlayerMobile, int> _budget = new();
        private readonly HashSet<PlayerMobile> _warnedNoMove = new();
        private readonly Dictionary<PlayerMobile, ResistanceMod[]> _defenseMods = new();
        private readonly Dictionary<PlayerMobile, List<Item>> _pickerItems = new();

        private const int MaxLog = 6;
        private readonly List<TurnHudGump.LogLine> _log = new();

        private bool _active;
        private bool _playersTurn;
        private long _phaseEndTick;
        private long _nextMobStep;
        private long _mobActFrom;
        private int _mobStepsUsed;
        private int _mobBudget;
        private bool _mobSwung;
        private bool _mobAttackedThisRound; // 1 ataque do mob por rodada (oportunidade OU turno dele)
        private string _lastKey;
        private TimerExecutionToken _tickToken;

        public TurnEncounter(BaseCreature mob, PlayerMobile initiator)
        {
            _mob = mob;
            _mobWrestlingBase = mob.Skills[SkillName.Wrestling].Base;
            Enroll(initiator);
        }

        public bool Active => _active;

        public void Start()
        {
            if (_active)
            {
                return;
            }

            _active = true;
            Timer.StartTimer(TickRate, TickRate, 0, Tick, out _tickToken);
            Log(0x40, "— Combate iniciado —");
            BeginPlayersTurn();
        }

        private void Enroll(PlayerMobile p)
        {
            if (_players.Contains(p))
            {
                return;
            }

            _players.Add(p);
            _registry[p] = this;
        }

        // ---- entrada de jogadores (multiplayer por first-hit) ---------------------------------------------------

        public void OnHitByPlayer(PlayerMobile p)
        {
            if (!_active || p is not { Alive: true })
            {
                return;
            }

            if (!_players.Contains(p))
            {
                Enroll(p);
                Log(0x3F, $"{p.Name} entrou no combate!");
                p.Frozen = !_playersTurn;
                p.NextCombatTime = Core.TickCount + FarFuture; // suprime auto-ataque
                _budget[p] = MoveBudget(p);
                _stepsUsed[p] = 0;

                if (_playersTurn)
                {
                    MarkActed(p);
                }
            }
            else
            {
                ForceRefresh();
            }
        }

        // ---- movimento do jogador (orçamento de tiles) ----------------------------------------------------------

        private void OnPlayerMoveAttempt(PlayerMobile p, MovementEventArgs e)
        {
            if (!_active || !_playersTurn || !_players.Contains(p))
            {
                e.Blocked = true;
                return;
            }

            var used = _stepsUsed.GetValueOrDefault(p);
            var budget = _budget.GetValueOrDefault(p, 3);

            if (used >= budget)
            {
                e.Blocked = true;
                if (_warnedNoMove.Add(p))
                {
                    p.SendMessage(0x22, "Voce ja usou todo o movimento deste turno.");
                }

                return;
            }

            // Sair do corpo-a-corpo provoca ataque de oportunidade (paralisa, dano, depois move).
            if (TryOpportunity(p, e))
            {
                return; // passo interrompido: não conta como movimento
            }

            _stepsUsed[p] = used + 1;
            ForceRefresh();
        }

        private static (int dx, int dy) DirOffset(Direction dir) =>
            (dir & Direction.Mask) switch
            {
                Direction.North => (0, -1),
                Direction.Right => (1, -1),
                Direction.East => (1, 0),
                Direction.Down => (1, 1),
                Direction.South => (0, 1),
                Direction.Left => (-1, 1),
                Direction.West => (-1, 0),
                Direction.Up => (-1, -1),
                _ => (0, 0)
            };

        // Disengage (sair do tile adjacente) -> ataque de oportunidade: 100% de acerto, 50% do dano,
        // paralisa o jogador rapidinho, aplica o dano, e depois ele move. 1 ataque do mob por rodada.
        private bool TryOpportunity(PlayerMobile p, MovementEventArgs e)
        {
            if (_mobAttackedThisRound || !_mob.InRange(p.Location, 1))
            {
                return false;
            }

            var (dx, dy) = DirOffset(e.Direction);
            if (_mob.InRange(new Point3D(p.X + dx, p.Y + dy, p.Z), 1))
            {
                return false; // continua adjacente: não é disengage
            }

            _mobAttackedThisRound = true;
            e.Blocked = true; // interrompe o passo: paralisa -> dano -> move

            _mob.Direction = _mob.GetDirectionTo(p);

            // Dano garantido = 50% do dano normal do mob (sem rolar acerto/erro).
            var dmg = Math.Max(1, Utility.RandomMinMax(_mob.DamageMin, _mob.DamageMax) / 2);
            _mob.DoHarmful(p);
            AOS.Damage(p, _mob, dmg, 100, 0, 0, 0, 0);

            p.Freeze(TimeSpan.FromSeconds(0.4)); // pausa curta, depois libera o movimento
            TurnFx.Opportunity(p);

            Log(0x22, $"Oportunidade em {p.Name}: -{dmg}");
            return true;
        }

        // ---- AÇÕES (via HUD) ------------------------------------------------------------------------------------

        private bool CanActNow(PlayerMobile p)
        {
            if (!_active || !_playersTurn || !_players.Contains(p))
            {
                p.SendMessage(0x22, "Nao e sua vez.");
                return false;
            }

            if (_acted.Contains(p))
            {
                p.SendMessage(0x22, "Voce ja agiu neste turno.");
                return false;
            }

            return true;
        }

        public void DoAttack(PlayerMobile p)
        {
            if (!CanActNow(p))
            {
                return;
            }

            if (!_mob.InRange(p.Location, 1))
            {
                p.SendMessage(0x22, "Muito longe para atacar. Aproxime-se.");
                return;
            }

            // Golpe deliberado (o auto-ataque fica suprimido via NextCombatTime durante o turno).
            p.Warmode = true;
            if (p.Combatant != _mob)
            {
                p.Combatant = _mob;
            }

            p.Direction = p.GetDirectionTo(_mob);

            if (p.Weapon is BaseWeapon weapon)
            {
                var before = _mob.Hits;
                weapon.OnSwing(p, _mob);
                if (_mob.Hits < before)
                {
                    TurnFx.Impact(_mob);
                }
            }

            MarkActed(p);
        }

        public void DoPass(PlayerMobile p)
        {
            if (!CanActNow(p))
            {
                return;
            }

            Log(0x3F, $"{p.Name} passou a vez.");
            MarkActed(p);
        }

        public void DoDefend(PlayerMobile p)
        {
            if (!CanActNow(p))
            {
                return;
            }

            ApplyDefense(p);
            TurnFx.Defend(p);
            Log(0x3F, $"{p.Name} defendeu (+{DefendBonus} resist).");
            MarkActed(p);
        }

        public void OpenPotions(PlayerMobile p)
        {
            if (!CanActNow(p))
            {
                return;
            }

            var items = CollectItems<BasePotion>(p);
            if (items.Count == 0)
            {
                p.SendMessage(0x22, "Voce nao tem pocoes na mochila.");
                return;
            }

            ShowPicker(p, "Escolha uma pocao:", items);
        }

        public void OpenScrolls(PlayerMobile p)
        {
            if (!CanActNow(p))
            {
                return;
            }

            var items = CollectItems<SpellScroll>(p);
            if (items.Count == 0)
            {
                p.SendMessage(0x22, "Voce nao tem pergaminhos na mochila.");
                return;
            }

            ShowPicker(p, "Escolha um pergaminho:", items);
        }

        public void OpenSpells(PlayerMobile p)
        {
            if (!CanActNow(p))
            {
                return;
            }

            p.SendMessage(0x3F, "Magia do spellbook: em breve (proxima etapa).");
        }

        public void UsePickedIndex(PlayerMobile p, int index)
        {
            if (!CanActNow(p))
            {
                p.CloseGump<TurnActionPickerGump>();
                return;
            }

            if (!_pickerItems.TryGetValue(p, out var items) || index < 0 || index >= items.Count)
            {
                return;
            }

            var it = items[index];
            p.CloseGump<TurnActionPickerGump>();
            _pickerItems.Remove(p);

            if (it is not { Deleted: false } || p.Backpack == null || !it.IsChildOf(p.Backpack))
            {
                p.SendMessage(0x22, "Item indisponivel.");
                return;
            }

            var used = false;
            if (it is BasePotion pot)
            {
                if (pot.CanDrink(p))
                {
                    pot.Drink(p);
                    used = true;
                }
            }
            else if (it is SpellScroll scroll)
            {
                scroll.OnDoubleClick(p); // inicia o cast (pode pedir alvo)
                used = true;
            }

            if (used)
            {
                Log(0x3F, $"{p.Name} usou {it.Name ?? it.GetType().Name}.");
                MarkActed(p);
            }
        }

        private static List<Item> CollectItems<T>(PlayerMobile p) where T : Item
        {
            var list = new List<Item>();
            var pack = p.Backpack;
            if (pack != null)
            {
                foreach (var it in pack.FindItemsByType<T>())
                {
                    list.Add(it);
                }
            }

            return list;
        }

        private void ShowPicker(PlayerMobile p, string title, List<Item> items)
        {
            _pickerItems[p] = items;

            var labels = new List<string>(items.Count);
            foreach (var it in items)
            {
                var name = it.Name ?? it.GetType().Name;
                labels.Add(it.Amount > 1 ? $"{name} ({it.Amount})" : name);
            }

            p.CloseGump<TurnActionPickerGump>();
            p.SendGump(new TurnActionPickerGump(this, p, title, labels));
        }

        private void ApplyDefense(PlayerMobile p)
        {
            if (_defenseMods.ContainsKey(p))
            {
                return;
            }

            var mods = new[]
            {
                new ResistanceMod(ResistanceType.Physical, "TurnDef", DefendBonus),
                new ResistanceMod(ResistanceType.Fire, "TurnDef", DefendBonus),
                new ResistanceMod(ResistanceType.Cold, "TurnDef", DefendBonus),
                new ResistanceMod(ResistanceType.Poison, "TurnDef", DefendBonus),
                new ResistanceMod(ResistanceType.Energy, "TurnDef", DefendBonus)
            };

            foreach (var m in mods)
            {
                p.AddResistanceMod(m);
            }

            _defenseMods[p] = mods;
        }

        private void ClearDefense(PlayerMobile p)
        {
            if (_defenseMods.Remove(p, out var mods) && p is { Deleted: false })
            {
                foreach (var m in mods)
                {
                    p.RemoveResistanceMod(m);
                }
            }
        }

        // ---- fugir ----------------------------------------------------------------------------------------------

        public void TryFlee(PlayerMobile p)
        {
            if (!CanActNow(p))
            {
                return;
            }

            if (Utility.RandomDouble() < FleeChance)
            {
                p.SendMessage(0x40, "Voce conseguiu fugir do combate!");
                TurnFx.FleeSuccess(p);
                RemovePlayer(p);

                if (_players.Count == 0)
                {
                    Stop();
                }
                else
                {
                    ForceRefresh();
                }
            }
            else
            {
                Log(0x22, $"{p.Name}: falha ao fugir!");
                MarkActed(p);
            }
        }

        // ---- núcleo de turnos -----------------------------------------------------------------------------------

        private void MarkActed(PlayerMobile p)
        {
            if (!_active || !_acted.Add(p))
            {
                return;
            }

            // Congela quem agiu (encerra a participacao no turno), EXCETO se ainda esta conjurando um cast.
            if (p is { Deleted: false } && p.Spell == null)
            {
                p.Frozen = true;
            }

            ForceRefresh();
        }

        private void Tick()
        {
            if (!_active)
            {
                return;
            }

            Prune();

            if (_mob is not { Deleted: false, Alive: true } || _mob.Map == Map.Internal || _players.Count == 0)
            {
                Stop();
                return;
            }

            if (_playersTurn)
            {
                // Congela quem ja agiu e terminou o cast (estava conjurando).
                FreezeFinishedCasters();

                var timeout = Core.TickCount >= _phaseEndTick;
                if (timeout || (AllActed() && !AnyoneCasting()))
                {
                    BeginEnemyTurn();
                    return;
                }
            }
            else
            {
                PursueStep();

                if (Core.TickCount >= _phaseEndTick)
                {
                    BeginPlayersTurn();
                    return;
                }
            }

            RefreshHud();
        }

        private void BeginPlayersTurn()
        {
            _playersTurn = true;
            _acted.Clear();
            _warnedNoMove.Clear();
            _mobAttackedThisRound = false;

            _mob.SetCurrentSpeedToActive();
            _mob.Frozen = true;
            _mob.NextCombatTime = Core.TickCount + FarFuture;
            _mob.Skills[SkillName.Wrestling].Base = MobDefenseDuringPlayerTurn;

            foreach (var p in _players)
            {
                if (p is { Deleted: false })
                {
                    ClearDefense(p); // a defesa durava ate aqui (cobriu o turno do inimigo)
                    p.Frozen = false;
                    p.NextCombatTime = Core.TickCount + FarFuture; // suprime auto-ataque no turno do jogador
                    _budget[p] = MoveBudget(p);
                    _stepsUsed[p] = 0;
                }
            }

            _phaseEndTick = Core.TickCount + (long)PlayerTurnTimeout.TotalMilliseconds;
            Log(0x3F, ">> Sua vez");
            ShowBanner(true);
        }

        private void BeginEnemyTurn()
        {
            _playersTurn = false;
            _mob.Skills[SkillName.Wrestling].Base = _mobWrestlingBase;

            foreach (var p in _players)
            {
                if (p is { Deleted: false })
                {
                    p.Frozen = true;
                }
            }

            var target = ClosestPlayer();

            _mob.Frozen = false;
            _mob.Warmode = true;
            _mob.CurrentSpeed = MutedSpeed;

            if (target != null && _mob.Combatant != target)
            {
                _mob.Combatant = target;
            }

            _mob.NextCombatTime = Core.TickCount + FarFuture; // suprime auto-swing; o golpe e manual (1 por turno)
            // Orçamento de perseguição: sempre fecha a distância que o alvo pode criar (+2 de margem) -> sem kite.
            _mobBudget = target != null ? Math.Max(MoveBudget(_mob), MoveBudget(target) + 2) : MoveBudget(_mob);
            _mobStepsUsed = 0;
            _mobSwung = false;
            _mobActFrom = Core.TickCount + MobLeadInMs; // espera o "beat" antes de mover/golpear
            _nextMobStep = _mobActFrom;

            _phaseEndTick = Core.TickCount + EnemyTurnTotalMs;
            Log(0x25, ">> Turno do inimigo");
            ShowBanner(false);
            TurnFx.Windup(_mob); // telegrafia: durante o "beat" o mob carrega o golpe (jogador vê vindo)
        }

        private void PursueStep()
        {
            var target = ClosestPlayer();
            if (target == null)
            {
                return;
            }

            if (_mob.Combatant != target)
            {
                _mob.Combatant = target;
            }

            _mob.Direction = _mob.GetDirectionTo(target);

            // "Beat" inicial: encara o alvo e espera 1,2s antes de mover/golpear (visual).
            if (Core.TickCount < _mobActFrom)
            {
                return;
            }

            // Adjacente: golpe normal do turno só se o mob ainda NÃO atacou nesta rodada (ex.: via oportunidade).
            if (_mob.InRange(target.Location, 1))
            {
                if (!_mobSwung && !_mobAttackedThisRound)
                {
                    if (_mob.Weapon is BaseWeapon w)
                    {
                        var before = target.Hits;
                        w.OnSwing(_mob, target);
                        if (target.Hits < before)
                        {
                            TurnFx.Impact(target);
                        }
                    }

                    _mobSwung = true;
                    _mobAttackedThisRound = true;
                }

                _phaseEndTick = Math.Min(_phaseEndTick, Core.TickCount + PostSwingMs);
                return;
            }

            // Gastou o orçamento de passos e não alcançou: passa o turno (não fica esperando o teto).
            if (_mobStepsUsed >= _mobBudget)
            {
                _phaseEndTick = Math.Min(_phaseEndTick, Core.TickCount + 600);
                return;
            }

            if (Core.TickCount < _nextMobStep)
            {
                return;
            }

            if (_mob.Move(_mob.Direction))
            {
                _mobStepsUsed++;
            }

            _nextMobStep = Core.TickCount + MobStepDelayMs;
        }

        private void FreezeFinishedCasters()
        {
            foreach (var p in _players)
            {
                if (p is { Deleted: false, Frozen: false } && _acted.Contains(p) && p.Spell == null)
                {
                    p.Frozen = true;
                }
            }
        }

        private bool AnyoneCasting()
        {
            foreach (var p in _players)
            {
                if (p is { Deleted: false } && _acted.Contains(p) && p.Spell != null)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AllActed()
        {
            foreach (var p in _players)
            {
                if (p is { Deleted: false, Alive: true } && !_acted.Contains(p))
                {
                    return false;
                }
            }

            return true;
        }

        private PlayerMobile ClosestPlayer()
        {
            PlayerMobile best = null;
            var bestDist = int.MaxValue;

            foreach (var p in _players)
            {
                if (p is not { Deleted: false, Alive: true } || p.Map != _mob.Map)
                {
                    continue;
                }

                var d = (int)_mob.GetDistanceToSqrt(p.Location);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = p;
                }
            }

            return best;
        }

        private void Prune()
        {
            for (var i = _players.Count - 1; i >= 0; i--)
            {
                var p = _players[i];
                if (p is not { Deleted: false, Alive: true }
                    || p.Map != _mob.Map
                    || !_mob.InRange(p.Location, LeashTiles))
                {
                    RemovePlayer(p);
                }
            }
        }

        private void RemovePlayer(PlayerMobile p)
        {
            ClearDefense(p);
            _players.Remove(p);
            _acted.Remove(p);
            _stepsUsed.Remove(p);
            _budget.Remove(p);
            _warnedNoMove.Remove(p);
            _pickerItems.Remove(p);
            _registry.Remove(p);

            if (p is { Deleted: false })
            {
                p.Frozen = false;
                p.NextCombatTime = Core.TickCount; // restaura combate normal
                p.CloseGump<TurnHudGump>();
                p.CloseGump<TurnActionPickerGump>();
                p.CloseGump<TurnBannerGump>();
            }
        }

        // ---- HUD ------------------------------------------------------------------------------------------------

        private int SecondsLeft()
        {
            var ms = _phaseEndTick - Core.TickCount;
            if (ms < 0)
            {
                ms = 0;
            }

            return (int)Math.Ceiling(ms / 1000.0);
        }

        private void Log(int hue, string text)
        {
            _log.Add(new TurnHudGump.LogLine(text, hue));
            while (_log.Count > MaxLog)
            {
                _log.RemoveAt(0);
            }

            ForceRefresh();
        }

        private void ForceRefresh()
        {
            _lastKey = null;
            RefreshHud();
        }

        // Banner de turno (splash) + som de virada. Fica ATIVO durante todo o turno; é trocado só na virada
        // (Singleton: o CloseGump+SendGump substitui o anterior). Fechado ao sair do combate / encerrar.
        private void ShowBanner(bool playersTurn)
        {
            foreach (var p in _players)
            {
                if (p is not { Deleted: false })
                {
                    continue;
                }

                p.CloseGump<TurnBannerGump>();
                p.SendGump(new TurnBannerGump(playersTurn));
            }
        }

        private void RefreshHud()
        {
            if (!_active)
            {
                return;
            }

            var secs = SecondsLeft();

            // Sem 'secs' na chave: o HUD não re-envia a cada segundo (tela fixa, sem piscar). Atualiza só em eventos.
            var key = $"{_playersTurn}|{_mob.Hits}|{_players.Count}|{_acted.Count}|{_log.Count}";
            foreach (var p in _players)
            {
                key += $"|{p.Serial}:{p.Hits}:{_stepsUsed.GetValueOrDefault(p)}:{_acted.Contains(p)}";
            }

            if (key == _lastKey)
            {
                return;
            }

            _lastKey = key;

            var entries = new List<TurnHudGump.HpEntry>
            {
                new(_mob.Name ?? "Inimigo", _mob.Hits, _mob.HitsMax, true)
            };

            foreach (var p in _players)
            {
                entries.Add(new TurnHudGump.HpEntry(p.Name ?? "Heroi", p.Hits, p.HitsMax, false));
            }

            foreach (var p in _players)
            {
                if (p is not { Deleted: false })
                {
                    continue;
                }

                var canAct = _playersTurn && !_acted.Contains(p);
                var used = _stepsUsed.GetValueOrDefault(p);
                var budget = _budget.GetValueOrDefault(p, MoveBudget(p));

                p.CloseGump<TurnHudGump>();
                p.SendGump(new TurnHudGump(this, p, _playersTurn, secs, used, budget, entries, canAct, _log));
            }
        }

        public void Stop()
        {
            _active = false;
            _tickToken.Cancel();

            if (_mob is { Deleted: false })
            {
                _mob.Frozen = false;
                _mob.NextCombatTime = Core.TickCount;
                _mob.Skills[SkillName.Wrestling].Base = _mobWrestlingBase;
                _mob.SetCurrentSpeedToActive();
            }

            for (var i = _players.Count - 1; i >= 0; i--)
            {
                var p = _players[i];
                ClearDefense(p);
                _registry.Remove(p);

                if (p is { Deleted: false })
                {
                    p.Frozen = false;
                    p.NextCombatTime = Core.TickCount; // restaura combate normal
                    p.CloseGump<TurnHudGump>();
                    p.CloseGump<TurnActionPickerGump>();
                    p.CloseGump<TurnBannerGump>();
                    p.SendMessage(0x40, "*** Combate por turnos encerrado ***");
                }
            }

            _players.Clear();
            _acted.Clear();
            _stepsUsed.Clear();
            _budget.Clear();
            _warnedNoMove.Clear();
            _defenseMods.Clear();
            _pickerItems.Clear();
        }
    }
}
