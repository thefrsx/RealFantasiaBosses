using System;
using ModernUO.Serialization;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Boss de teste que prova a base RFBoss: usa as mecânicas de FX como ataques reais, desbloqueando-as
    // conforme cai de HP (fases). Spawn: [add RFBossTest
    [SerializationGenerator(0, false)]
    public partial class RFBossTest : RFBoss
    {
        [Constructible]
        public RFBossTest() : base(AIType.AI_Melee, FightMode.Closest, 18, 1)
        {
            Name = "RF Boss Test";

            Body = 0x190;       // humano
            Hue = 0x21;         // vermelho
            HairItemID = 0x203C;
            HairHue = 0x21;
            AddItem(new Robe(0x1));
            AddItem(new Boots(0x1));

            SetStr(600, 800);
            SetDex(150, 180);
            SetInt(300, 400);

            SetHits(3000);

            SetDamage(12, 18);
            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Tactics, 100.0, 110.0);
            SetSkill(SkillName.Wrestling, 100.0, 110.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Anatomy, 100.0, 110.0);

            Fame = 15000;
            Karma = -15000;
            VirtualArmor = 50;
        }

        public override string CorpseName => "a RF boss corpse";
        public override bool ShowFameTitle => false;

        // Enrage na fase final: ataca mais rápido.
        public override TimeSpan AbilityInterval =>
            CurrentPhase >= 3 ? TimeSpan.FromSeconds(3.0) : TimeSpan.FromSeconds(5.0);

        protected override void BuildAbilities()
        {
            // Fase 0+: sopro em cone (básico).
            AddAbility(new FireConeAbility { Length = 6, Damage = 16, Cooldown = TimeSpan.FromSeconds(8) });

            // <=75% HP: paredes de fogo.
            AddAbility(
                new FireLineAbility { Tiles = 6, Dirs = 4, Damage = 18, UnlockAtPercent = 75, Cooldown = TimeSpan.FromSeconds(10) }
            );

            // <=50% HP: nova + ninhada de aranhas.
            AddAbility(new FireNovaAbility { Radius = 6, Damage = 20, UnlockAtPercent = 50, Cooldown = TimeSpan.FromSeconds(12) });
            AddAbility(new SpiderEggAbility { Count = 4, UnlockAtPercent = 50, Cooldown = TimeSpan.FromSeconds(16) });

            // <=25% HP (enrage): chuva de meteoros.
            AddAbility(new MeteorAbility { Count = 6, Damage = 26, UnlockAtPercent = 25, Cooldown = TimeSpan.FromSeconds(8) });
        }

        protected override void OnPhaseChanged(int phase)
        {
            switch (phase)
            {
                case 1:
                    Say("Vocês vão arder!");
                    break;
                case 2:
                    Say("Sintam o inferno!");
                    break;
                case 3:
                    Say("CHEGA! MORRAM TODOS!");
                    break;
            }

            FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
            PlaySound(0x208);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
        }
    }
}
