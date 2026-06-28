using System;
using ModernUO.Serialization;
using Server;
using Server.Mobiles;

namespace Server.Custom.Mobiles
{
    // A destructible "spider egg": an immobile creature with its own HP.
    // If players don't destroy it in time, it hatches and summons spiders.
    // (In ModernUO only mobiles have HP / are attackable, so the egg is a passive BaseCreature.)
    [SerializationGenerator(0, false)]
    public partial class SpiderEgg : BaseCreature
    {
        [SerializableField(0)]
        private double _hatchSeconds;

        [SerializableField(1)]
        private int _spiderCount;

        private HatchTimer _timer;

        [Constructible]
        public SpiderEgg() : this(8.0, 3)
        {
        }

        public SpiderEgg(double hatchSeconds, int spiderCount) : base(AIType.AI_Animal, FightMode.None, 10, 1)
        {
            Name = "a spider egg";
            Body = 8;       // Corpser plant body — reads as a sac rooted to the ground
            Hue = 0x851;    // pale sickly green
            BaseSoundID = 0x388;

            SetStr(50);
            SetDex(0);
            SetInt(0);

            SetHits(40);    // the egg's own HP (tunable)
            SetDamage(0);

            SetResistance(ResistanceType.Physical, 10, 20);
            SetResistance(ResistanceType.Fire, 0, 10);

            Fame = 0;
            Karma = 0;
            VirtualArmor = 8;

            CantWalk = true;
            Frozen = true;  // never moves or attacks

            _hatchSeconds = hatchSeconds < 0.5 ? 0.5 : hatchSeconds;
            _spiderCount = Math.Clamp(spiderCount, 1, 10);

            StartHatch();
        }

        public override string CorpseName => "a shattered egg";
        public override bool DeleteCorpseOnDeath => true;
        public override bool AlwaysMurderer => false;
        public override void GenerateLoot()
        {
        }

        private void StartHatch()
        {
            _timer?.Stop();
            _timer = new HatchTimer(this);
            _timer.Start();
        }

        public void Hatch()
        {
            if (Deleted || !Alive)
            {
                return;
            }

            var map = Map;
            if (map != null)
            {
                Effects.SendLocationEffect(Location, map, 0x3728, 13);
                Effects.PlaySound(Location, map, 0x307);

                for (var i = 0; i < _spiderCount; i++)
                {
                    var spider = new GiantSpider { Team = Team };
                    var loc = new Point3D(
                        X + Utility.RandomMinMax(-1, 1),
                        Y + Utility.RandomMinMax(-1, 1),
                        Z
                    );
                    spider.MoveToWorld(loc, map);
                }
            }

            Delete();
        }

        public override bool OnBeforeDeath()
        {
            // Destroyed in time -> no hatch.
            _timer?.Stop();
            _timer = null;
            return base.OnBeforeDeath();
        }

        public override void OnAfterDelete()
        {
            _timer?.Stop();
            _timer = null;
            base.OnAfterDelete();
        }

        private class HatchTimer : Timer
        {
            private readonly SpiderEgg _egg;

            public HatchTimer(SpiderEgg egg) : base(TimeSpan.FromSeconds(egg._hatchSeconds))
            {
                _egg = egg;
            }

            protected override void OnTick()
            {
                _egg.Hatch();
            }
        }
    }
}
