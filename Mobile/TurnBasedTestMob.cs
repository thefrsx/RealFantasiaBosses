using ModernUO.Serialization;
using Server.Custom.Combat;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // PROTÓTIPO 1v1 do combate "por turnos ao vivo" (Modelo A). Spawn: [add TurnBasedTestMob
    //
    // Ataque-o normalmente (double-click). O mob vira-se Frozen e espera VOCÊ; mova-se à vontade e dê UM golpe
    // (atacar passa a vez). Aí o jogador fica Frozen por ~1.75s enquanto o mob age — é aqui que se sente a
    // fricção do cliente (rubber-banding) que precisa ser validada antes de pensar em multiplayer.
    //
    // Use [props -> TurnBased = false para voltar ao combate real-time normal da UO e comparar lado a lado.
    [SerializationGenerator(0, false)]
    public partial class TurnBasedTestMob : BaseCreature
    {
        [SerializableField(0)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private bool _turnBased;

        // Transitório (não serializado): o gerenciador da luta corrente.
        private TurnEncounter _encounter;

        [Constructible]
        public TurnBasedTestMob() : base(AIType.AI_Melee, FightMode.Closest, 18, 1)
        {
            Name = "Turn Test Dummy";

            Body = 0x190;
            Hue = 0x83EA;
            HairItemID = 0x203B;
            HairHue = 0x47;

            AddItem(new Robe(0x48D));
            AddItem(new Boots(0x901));

            SetStr(300);
            SetDex(100);
            SetInt(100);

            SetHits(500);

            SetDamage(8, 12);
            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Tactics, 90.0, 100.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);
            SetSkill(SkillName.MagicResist, 90.0, 100.0);
            SetSkill(SkillName.Anatomy, 90.0, 100.0);

            Fame = 5000;
            Karma = -5000;
            VirtualArmor = 30;

            _turnBased = true; // padrão do protótipo
        }

        public override string CorpseName => "a turn test corpse";
        public override bool ShowFameTitle => false;

        public override void OnCombatantChange()
        {
            base.OnCombatantChange();

            // Inicia a luta por turnos se o mob agredir/for agredido por um jogador (não auto-para no null:
            // o encounter se gerencia via leash/morte, senão pararia durante o longo Frozen do turno do jogador).
            if (_turnBased && !Deleted && Alive && Combatant is PlayerMobile p && _encounter is not { Active: true })
            {
                _encounter = new TurnEncounter(this, p);
                _encounter.Start();
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker, int damage)
        {
            base.OnGotMeleeAttack(attacker, damage);

            if (!_turnBased || attacker is not PlayerMobile p)
            {
                return;
            }

            // Primeiro golpe inicia a luta; golpes seguintes registram a ação / entram no combate (multiplayer).
            if (_encounter is not { Active: true })
            {
                _encounter = new TurnEncounter(this, p);
                _encounter.Start();
            }
            else
            {
                _encounter.OnHitByPlayer(p);
            }
        }

        public override bool OnBeforeDeath()
        {
            _encounter?.Stop();
            _encounter = null;
            return base.OnBeforeDeath();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            _encounter?.Stop();
            _encounter = null;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }
    }
}
