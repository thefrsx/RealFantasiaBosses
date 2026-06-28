using ModernUO.Serialization;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Lacaio invocado pela mecânica SummonMinion. Corpo/nome/hue ajustáveis após spawn.
    [SerializationGenerator(0, false)]
    public partial class BossMinion : BaseCreature
    {
        [Constructible]
        public BossMinion() : base(AIType.AI_Melee, FightMode.Closest, 10)
        {
            Body = 50; // esqueleto (default; a mecanica pode trocar)
            Name = "lacaio";

            SetStr(90, 110);
            SetDex(60, 80);
            SetInt(20, 40);

            SetHits(45, 65);
            SetDamage(5, 9);

            SetDamageType(ResistanceType.Physical, 100);
            SetResistance(ResistanceType.Physical, 25, 35);

            SetSkill(SkillName.Wrestling, 55, 70);
            SetSkill(SkillName.Tactics, 55, 70);
            SetSkill(SkillName.MagicResist, 40, 55);

            VirtualArmor = 24;
            Fame = 600;
            Karma = -600;
        }

        public override string CorpseName => "um corpo de lacaio";
    }
}
