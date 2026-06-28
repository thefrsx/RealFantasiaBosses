using System;
using Server;

namespace Server.Custom.Effect
{
    // Breathes a widening fan of fire ("dragon breath") from origin toward a direction.
    // Thin wrapper over AreaEffectEngine + AreaShapes + TileFx, so it shares sequencing with the other mechanics.
    public static class FireConeEffect
    {
        // One fire cone from <origin> toward <dir>, <length> tiles deep.
        public static void Breath(
            Map map,
            Point3D origin,
            Direction dir,
            int length = 6,
            TimeSpan tileDelay = default,
            Mobile damageSource = null,
            int damage = 0
        )
        {
            if (length < 1)
            {
                length = 1;
            }

            if (tileDelay == default)
            {
                tileDelay = TimeSpan.FromMilliseconds(120);
            }

            // The fan only reads cleanly along the cardinals; snap any diagonal facing to the nearest one.
            AreaEffectEngine.Run(
                map,
                AreaShapes.Cone(origin, SnapToCardinal(dir), length),
                tileDelay,
                TileFx.Flame(origin, damageSource, damage)
            );
        }

        // Cone fired toward a random cardinal direction (when there is no obvious facing target).
        public static void BreathRandom(
            Map map,
            Point3D origin,
            int length = 6,
            TimeSpan tileDelay = default,
            Mobile damageSource = null,
            int damage = 0
        )
        {
            var dirs = new[] { Direction.North, Direction.East, Direction.South, Direction.West };
            Breath(map, origin, dirs[Utility.Random(dirs.Length)], length, tileDelay, damageSource, damage);
        }

        // Rounds a facing down to the nearest cardinal (NE->N, SE->E, SW->S, NW->W); cardinals pass through.
        public static Direction SnapToCardinal(Direction dir)
        {
            var d = (int)(dir & Direction.Mask);
            if ((d & 1) != 0) // diagonals are the odd values in UO's Direction enum
            {
                d -= 1;
            }

            return (Direction)d;
        }
    }
}
