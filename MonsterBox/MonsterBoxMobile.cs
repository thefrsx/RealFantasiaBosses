using ModernUO.Serialization;
using Server.Custom.Items;
using Server.Items;

namespace Server.Mobiles
{
    // Criatura genérica configurada pelo MonsterBoxItem. Os stats persistem via a serialização nativa de
    // BaseCreature/Mobile (RawStr, resistências, AI, FightMode...), então ela sobrevive a restart sem campos extras.
    [SerializationGenerator(0, false)]
    public partial class MonsterBoxMobile : BaseCreature
    {
        [Constructible]
        public MonsterBoxMobile() : base(AIType.AI_Melee, FightMode.Closest, 10, 1)
        {
        }

        public void Configure(MonsterBoxItem b)
        {
            ChangeAIType(b.Ai);
            FightMode = b.FightMode;
            RangePerception = b.RangePerception;
            RangeFight = b.RangeFight;
            ActiveSpeed = b.ActiveSpeed;
            PassiveSpeed = b.PassiveSpeed;

            Body = b.MobBody;
            Hue = b.MobHue;
            Name = b.MobName;

            SetStr(b.StrMin, b.StrMax);
            SetDex(b.DexMin, b.DexMax);
            SetInt(b.IntMin, b.IntMax);

            SetHits(b.Hits);
            SetStam(b.Stam);
            SetMana(b.Mana);

            Fame = b.Fame;
            Karma = b.Karma;

            SetDamage(b.DamageMin, b.DamageMax);

            SetDamageType(ResistanceType.Physical, b.DmgPhys);
            SetDamageType(ResistanceType.Fire, b.DmgFire);
            SetDamageType(ResistanceType.Cold, b.DmgCold);
            SetDamageType(ResistanceType.Poison, b.DmgPois);
            SetDamageType(ResistanceType.Energy, b.DmgNrgy);

            SetResistance(ResistanceType.Physical, b.ResPhys);
            SetResistance(ResistanceType.Fire, b.ResFire);
            SetResistance(ResistanceType.Cold, b.ResCold);
            SetResistance(ResistanceType.Poison, b.ResPois);
            SetResistance(ResistanceType.Energy, b.ResNrgy);

            VirtualArmor = b.VirtualArmor;
        }

        public override string CorpseName => $"o corpo de {Name ?? "uma criatura"}";

        public override void GenerateLoot() => AddLoot(LootPack.Meager);
    }
}
