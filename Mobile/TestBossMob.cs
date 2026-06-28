using ModernUO.Serialization;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    [SerializationGenerator(0, false)]
    public partial class TestBossMob : BaseCreature
    {
        [Constructible]
        public TestBossMob() : base(AIType.AI_Melee, FightMode.Closest, 10, 1)
        {
            Name = "Test Boss Mob";

            // Human appearance
            Body = 0x190;        // male human
            Hue = 0x83EA;        // a human skin tone
            HairItemID = 0x203B; // short hair
            HairHue = 0x47;

            AddItem(new Robe(0x4F7));
            AddItem(new Boots(0x901));

            // Placeholder combat stats (test only - real boss logic comes later)
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
        }

        public override string CorpseName => "a test boss corpse";

        public override bool ShowFameTitle => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }
    }
}
