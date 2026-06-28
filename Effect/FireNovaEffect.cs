using System;
using Server;

namespace Server.Custom.Effect
{
    // Expanding ring of fire bursting outward from a point — a "nova" telegraph (get out of the blast).
    // Demonstrates the framework: a different SHAPE (Nova) + the same Flame painter = a brand-new mechanic.
    public static class FireNovaEffect
    {
        public static void Burst(
            Map map,
            Point3D origin,
            int radius = 5,
            TimeSpan ringDelay = default,
            Mobile damageSource = null,
            int damage = 0
        )
        {
            if (radius < 1)
            {
                radius = 1;
            }

            if (ringDelay == default)
            {
                ringDelay = TimeSpan.FromMilliseconds(150);
            }

            AreaEffectEngine.Run(
                map,
                AreaShapes.Nova(origin, radius),
                ringDelay,
                TileFx.Flame(origin, damageSource, damage)
            );
        }
    }
}
