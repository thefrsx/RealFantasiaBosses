using System;
using Server;
using Server.Custom.Mobiles;

namespace Server.Custom.Effect
{
    // Boss mechanic: scatter <count> spider eggs over random tiles within <radius> of an origin.
    // Each egg has its own HP and hatches into spiders after <hatchSeconds> unless destroyed first.
    public static class SpiderEggEffect
    {
        public static void Scatter(
            Map map,
            Point3D origin,
            int count,
            int radius = 4,
            double hatchSeconds = 8.0,
            int spidersPerEgg = 3,
            int team = 0
        )
        {
            if (map == null || map == Map.Internal)
            {
                return;
            }

            count = Math.Clamp(count, 1, 30);

            if (radius < 0)
            {
                radius = 0;
            }

            for (var i = 0; i < count; i++)
            {
                var x = origin.X + Utility.RandomMinMax(-radius, radius);
                var y = origin.Y + Utility.RandomMinMax(-radius, radius);
                var loc = new Point3D(x, y, origin.Z);

                var egg = new SpiderEgg(hatchSeconds, spidersPerEgg) { Team = team };
                egg.MoveToWorld(loc, map);

                Effects.SendLocationEffect(loc, map, 0x3728, 10);
            }
        }
    }
}
