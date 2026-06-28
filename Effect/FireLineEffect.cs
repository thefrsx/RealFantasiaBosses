using System;
using Server;

namespace Server.Custom.Effect
{
    // Throws walls of fire (Fire Field FX) in straight lines — a travelling "line / cross" telegraph.
    // Thin wrapper over AreaEffectEngine + AreaShapes + TileFx, so it shares sequencing with other mechanics.
    public static class FireLineEffect
    {
        // One fire line from <origin> toward <dir>, <length> tiles long.
        public static void Throw(
            Map map,
            Point3D origin,
            Direction dir,
            int length = 5,
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

            AreaEffectEngine.Run(
                map,
                AreaShapes.Line(origin, dir, length),
                tileDelay,
                TileFx.Flame(origin, damageSource, damage)
            );
        }

        // <directions> fire lines fired together, each toward a distinct RANDOM compass direction
        // (cardinals first, then diagonals). Each wall's flame respects its own direction.
        public static void ThrowRandom(
            Map map,
            Point3D origin,
            int length,
            int directions,
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

            var dirs = PickRandomDirections(directions);

            AreaEffectEngine.Run(
                map,
                AreaShapes.Star(origin, dirs, length),
                tileDelay,
                TileFx.Flame(origin, damageSource, damage)
            );
        }

        private static Direction[] PickRandomDirections(int count)
        {
            count = Math.Clamp(count, 1, 8);

            // Cardinals first (clean E-W / N-S walls), then diagonals if more are requested.
            var cardinals = new[] { Direction.North, Direction.East, Direction.South, Direction.West };
            var diagonals = new[] { Direction.Right, Direction.Down, Direction.Left, Direction.Up };
            Shuffle(cardinals);
            Shuffle(diagonals);

            var result = new Direction[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = i < 4 ? cardinals[i] : diagonals[i - 4];
            }

            return result;
        }

        private static void Shuffle(Direction[] arr)
        {
            for (var i = arr.Length - 1; i > 0; i--)
            {
                var j = Utility.Random(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
    }
}
