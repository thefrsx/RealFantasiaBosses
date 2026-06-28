using ModernUO.Serialization;
using Server;
using Server.Custom.Gumps;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Custom.Items
{
    // "Monster In A Box" portado pro ModernUO (o original ServUO era um protótipo quebrado: spawn comentado,
    // parsing bugado, só 6 campos persistidos). Aqui o box é um TEMPLATE reutilizável que guarda TODOS os stats
    // como [SerializableField]; o gump edita e o Spawn cria a criatura de verdade. [add MonsterBoxItem -> dbl-click.
    [SerializationGenerator(0, false)]
    public partial class MonsterBoxItem : Item
    {
        [SerializableField(0)] [SerializedCommandProperty(AccessLevel.GameMaster)] private string _mobName;
        [SerializableField(1)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _mobHue;
        [SerializableField(2)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _mobBody;
        [SerializableField(3)] [SerializedCommandProperty(AccessLevel.GameMaster)] private AIType _ai;
        [SerializableField(4)] [SerializedCommandProperty(AccessLevel.GameMaster)] private FightMode _fightMode;
        [SerializableField(5)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _rangePerception;
        [SerializableField(6)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _rangeFight;
        [SerializableField(7)] [SerializedCommandProperty(AccessLevel.GameMaster)] private double _activeSpeed;
        [SerializableField(8)] [SerializedCommandProperty(AccessLevel.GameMaster)] private double _passiveSpeed;
        [SerializableField(9)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _strMin;
        [SerializableField(10)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _strMax;
        [SerializableField(11)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _dexMin;
        [SerializableField(12)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _dexMax;
        [SerializableField(13)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _intMin;
        [SerializableField(14)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _intMax;
        [SerializableField(15)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _hits;
        [SerializableField(16)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _stam;
        [SerializableField(17)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _mana;
        [SerializableField(18)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _fame;
        [SerializableField(19)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _karma;
        [SerializableField(20)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _damageMin;
        [SerializableField(21)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _damageMax;
        [SerializableField(22)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _dmgPhys;
        [SerializableField(23)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _dmgFire;
        [SerializableField(24)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _dmgCold;
        [SerializableField(25)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _dmgPois;
        [SerializableField(26)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _dmgNrgy;
        [SerializableField(27)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _resPhys;
        [SerializableField(28)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _resFire;
        [SerializableField(29)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _resCold;
        [SerializableField(30)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _resPois;
        [SerializableField(31)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _resNrgy;
        [SerializableField(32)] [SerializedCommandProperty(AccessLevel.GameMaster)] private int _virtualArmor;

        [Constructible]
        public MonsterBoxItem() : base(0xE80)
        {
            Name = "Monster In A Box";
            Hue = 0x47E;

            // defaults sensatos (um melee básico)
            _mobName = "criatura";
            _mobHue = 0;
            _mobBody = 0x190; // humano
            _ai = AIType.AI_Melee;
            _fightMode = FightMode.Closest;
            _rangePerception = 10;
            _rangeFight = 1;
            _activeSpeed = 0.2;
            _passiveSpeed = 0.4;
            _strMin = 96; _strMax = 120;
            _dexMin = 36; _dexMax = 55;
            _intMin = 16; _intMax = 30;
            _hits = 50; _stam = 50; _mana = 0;
            _fame = 1000; _karma = -1000;
            _damageMin = 5; _damageMax = 10;
            _dmgPhys = 100; _dmgFire = 0; _dmgCold = 0; _dmgPois = 0; _dmgNrgy = 0;
            _resPhys = 30; _resFire = 20; _resCold = 20; _resPois = 15; _resNrgy = 15;
            _virtualArmor = 20;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.Counselor && !from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.SendGump(new MonsterBoxGump(this));
        }

        // Cria a criatura configurada no local do jogador.
        public void Spawn(Mobile from)
        {
            if (from?.Map == null || from.Map == Map.Internal)
            {
                return;
            }

            var m = new MonsterBoxMobile();
            m.Configure(this);
            m.MoveToWorld(from.Location, from.Map);
            from.SendMessage(0x40, $"'{MobName}' criado (corpo {MobBody}).");
        }
    }
}
