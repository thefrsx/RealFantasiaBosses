using System;
using System.Collections.Generic;
using Server;
using Server.Custom.Gumps;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Motor de mecânicas de boss ANEXÁVEL a qualquer mobile (runtime/evento, sem subclassar). Dirige o "pulso"
    // de habilidade, as fases por %HP e a TELEGRAFIA: avisa a mecânica (BossWarningGump) por N segundos e então
    // dispara. Carregado pelo BossLoaderGump (GM clica mecânicas + mira no boss).
    public sealed class BossController
    {
        private static readonly Dictionary<Mobile, BossController> _active = new();
        private static readonly TimeSpan TickRate = TimeSpan.FromMilliseconds(300);
        private const int WarnRange = 18;

        public static bool IsBoss(Mobile m) => m != null && _active.ContainsKey(m);

        public static void Attach(Mobile boss, List<RFBossAbility> kit)
        {
            if (boss is not { Deleted: false } || kit == null || kit.Count == 0)
            {
                return;
            }

            Detach(boss);
            var c = new BossController(boss, kit);
            _active[boss] = c;
            c.Start();
        }

        public static void Detach(Mobile boss)
        {
            if (boss != null && _active.Remove(boss, out var c))
            {
                c.Stop();
            }
        }

        private readonly Mobile _boss;
        private readonly List<RFBossAbility> _kit;
        private int _phase;
        private long _nextAbilityTime;
        private RFBossAbility _pending;
        private Mobile _pendingTarget;
        private long _fireTime;
        private TimerExecutionToken _token;

        private readonly int _originalHue;

        // Cores por fase (espetaculo de enrage): fase 0 = hue original, depois esquenta ate vermelho intenso.
        private static readonly int[] PhaseHues = { 0, 0x2B, 0x21, 0x26 };

        private BossController(Mobile boss, List<RFBossAbility> kit)
        {
            _boss = boss;
            _kit = kit;
            _originalHue = boss.Hue;
        }

        private TimeSpan IntervalBetween => _phase >= 3 ? TimeSpan.FromSeconds(2.0) : TimeSpan.FromSeconds(4.0);

        private void Start()
        {
            _nextAbilityTime = Core.TickCount + 2000;
            Timer.StartTimer(TickRate, TickRate, 0, OnTick, out _token);
        }

        private void Stop()
        {
            _token.Cancel();
            CloseWarning();
            if (_boss != null)
            {
                _boss.Hue = _originalHue; // restaura a cor original ao remover o preset
            }
        }

        // Protótipo 2: transição de fase = espetáculo (burst + som + cor de enrage).
        private void PhaseSpectacle(int phase)
        {
            _boss.Say(phase >= 3 ? "* FURIA! *" : $"* Fase {phase + 1}! *");
            _boss.PlaySound(0x20F);
            _boss.FixedParticles(0x3709, 20, 30, 5052, EffectLayer.Head);
            _boss.FixedParticles(0x373A, 10, 30, 5036, EffectLayer.Waist);

            var idx = Math.Clamp(phase, 0, PhaseHues.Length - 1);
            if (_boss.Map != null)
            {
                AbilityFx.DiscMark(_boss.Location, _boss.Map, 3, 0x3709, PhaseHues[idx] == 0 ? 0x26 : PhaseHues[idx]);
            }

            _boss.Hue = PhaseHues[idx] == 0 ? _originalHue : PhaseHues[idx];
        }

        private void OnTick()
        {
            if (_boss is not { Deleted: false, Alive: true } || _boss.Map == null || _boss.Map == Map.Internal)
            {
                Detach(_boss);
                return;
            }

            var pct = _boss.HitsMax > 0 ? _boss.Hits * 100 / _boss.HitsMax : 100;
            var ph = pct >= 75 ? 0 : pct >= 50 ? 1 : pct >= 25 ? 2 : 3;
            if (ph > _phase)
            {
                _phase = ph;
                PhaseSpectacle(_phase);
            }

            // Telegrafia em andamento: NÃO cancela por perda momentânea de alvo; dispara ao fim do aviso.
            if (_pending != null)
            {
                if (Core.TickCount >= _fireTime)
                {
                    CloseWarning();

                    // Usa o alvo capturado no início do aviso (a IA pode tê-lo largado por ser staff); senão re-procura.
                    var tgt = IsValidTarget(_pendingTarget) ? _pendingTarget : FindTarget();
                    if (tgt != null)
                    {
                        _pending.Use(_boss, tgt);
                    }

                    _pending.MarkUsed();
                    _pending = null;
                    _pendingTarget = null;
                    _nextAbilityTime = Core.TickCount + (long)IntervalBetween.TotalMilliseconds;
                }
                else
                {
                    // Protótipo 1: pinta a área do golpe no chão a cada tick (pulsa) durante todo o aviso.
                    _pending.PaintTelegraph(_boss, IsValidTarget(_pendingTarget) ? _pendingTarget : null);
                }

                return;
            }

            if (Core.TickCount < _nextAbilityTime)
            {
                return;
            }

            // Só inicia a telegrafia se houver um alvo válido — e CAPTURA esse alvo para usar no disparo.
            var startTarget = FindTarget();
            if (startTarget == null)
            {
                return;
            }

            // Escolhe (amostragem-reservatório) uma habilidade pronta e inicia a telegrafia.
            RFBossAbility chosen = null;
            var seen = 0;
            foreach (var a in _kit)
            {
                if (a.Ready(pct) && Utility.Random(++seen) == 0)
                {
                    chosen = a;
                }
            }

            if (chosen == null)
            {
                return;
            }

            _pending = chosen;
            _pendingTarget = startTarget;
            _fireTime = Core.TickCount + (long)(chosen.TelegraphSeconds * 1000);
            ShowWarning(chosen);
        }

        private bool IsValidTarget(Mobile m) =>
            m is { Alive: true, Deleted: false } && m.Map == _boss.Map && _boss.InRange(m.Location, 25);

        private Mobile FindTarget()
        {
            var c = _boss.Combatant;
            if (IsValidTarget(c))
            {
                return c;
            }

            // Só conjura EM COMBATE: procura um agressor (quem atacou o boss) que ainda seja alvo válido.
            // (A IA larga staff/Owner como Combatant, mas o agressor persiste pela janela de combate.)
            var list = _boss.Aggressors;
            for (var i = 0; i < list.Count; i++)
            {
                var info = list[i];
                if (!info.Expired && info.Attacker is PlayerMobile && IsValidTarget(info.Attacker))
                {
                    return info.Attacker;
                }
            }

            return null;
        }

        private void ShowWarning(RFBossAbility a)
        {
            var bossName = _boss.Name ?? "O chefe";

            // Reforço no próprio boss: fala + brilho de conjuração durante o aviso.
            _boss.Say($"*conjura {a.Name}...*");
            _boss.FixedParticles(0x3779, 10, 25, 5032, EffectLayer.Head);

            foreach (var m in _boss.GetMobilesInRange(WarnRange))
            {
                if (m is PlayerMobile p)
                {
                    p.CloseGump<BossWarningGump>();
                    p.SendGump(new BossWarningGump(bossName, a.Name, a.WarningText));
                }
            }
        }

        private void CloseWarning()
        {
            foreach (var m in _boss.GetMobilesInRange(WarnRange))
            {
                if (m is PlayerMobile p)
                {
                    p.CloseGump<BossWarningGump>();
                }
            }
        }
    }
}
