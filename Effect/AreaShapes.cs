using System;
using System.Collections.Generic;
using Server;

namespace Server.Custom.Effect
{
    // Tile-pattern geometry for area mechanics. Each method returns a list of "waves"
    // (groups of tiles that activate together) ready to feed AreaEffectEngine.Run.
    // Z uses the origin's Z (fine on flat ground; refine later for slopes if needed).
    public static class AreaShapes
    {
        public static (int dx, int dy) Offset(Direction dir) =>
            (dir & Direction.Mask) switch
            {
                Direction.North => (0, -1),
                Direction.Right => (1, -1),
                Direction.East => (1, 0),
                Direction.Down => (1, 1),
                Direction.South => (0, 1),
                Direction.Left => (-1, 1),
                Direction.West => (-1, 0),
                Direction.Up => (-1, -1),
                _ => (0, 0)
            };

        // A straight line; each tile is its own wave, so the effect travels outward.
        public static List<IReadOnlyList<Point3D>> Line(Point3D origin, Direction dir, int length, bool startAhead = true)
        {
            if (length < 1)
            {
                length = 1;
            }

            var (dx, dy) = Offset(dir);
            var start = startAhead ? 1 : 0;

            var waves = new List<IReadOnlyList<Point3D>>(length);
            for (var i = 0; i < length; i++)
            {
                var step = start + i;
                waves.Add(new[] { new Point3D(origin.X + dx * step, origin.Y + dy * step, origin.Z) });
            }

            return waves;
        }

        // Several lines from one origin, fired together (wave i = the i-th tile of every line).
        public static List<IReadOnlyList<Point3D>> Star(
            Point3D origin, IReadOnlyList<Direction> dirs, int length, bool startAhead = true
        )
        {
            if (length < 1)
            {
                length = 1;
            }

            var start = startAhead ? 1 : 0;

            var waves = new List<IReadOnlyList<Point3D>>(length);
            for (var i = 0; i < length; i++)
            {
                var step = start + i;
                var tiles = new Point3D[dirs.Count];
                for (var d = 0; d < dirs.Count; d++)
                {
                    var (dx, dy) = Offset(dirs[d]);
                    tiles[d] = new Point3D(origin.X + dx * step, origin.Y + dy * step, origin.Z);
                }

                waves.Add(tiles);
            }

            return waves;
        }

        // A widening fan ("cone" / dragon breath) from origin toward dir. Each step is its own wave,
        // so the breath travels outward; the perpendicular half-width grows with distance (1 -> 3 -> 5 ...).
        public static List<IReadOnlyList<Point3D>> Cone(Point3D origin, Direction dir, int length, bool startAhead = true)
        {
            if (length < 1)
            {
                length = 1;
            }

            var (dx, dy) = Offset(dir);
            var (px, py) = (-dy, dx); // perpendicular axis (rotate 90 degrees)
            var start = startAhead ? 1 : 0;

            var waves = new List<IReadOnlyList<Point3D>>(length);
            for (var i = 0; i < length; i++)
            {
                var step = start + i;
                var halfWidth = step / 2; // 1 tile at the mouth, widening every other step
                var cx = origin.X + dx * step;
                var cy = origin.Y + dy * step;

                var tiles = new Point3D[halfWidth * 2 + 1];
                var idx = 0;
                for (var w = -halfWidth; w <= halfWidth; w++)
                {
                    tiles[idx++] = new Point3D(cx + px * w, cy + py * w, origin.Z);
                }

                waves.Add(tiles);
            }

            return waves;
        }

        // A hollow ring at a fixed radius (single wave).
        public static List<IReadOnlyList<Point3D>> Ring(Point3D origin, int radius)
        {
            return new List<IReadOnlyList<Point3D>> { RingTiles(origin, radius) };
        }

        // Expanding rings from radius 1..maxRadius, each its own wave (a nova travelling outward).
        public static List<IReadOnlyList<Point3D>> Nova(Point3D origin, int maxRadius)
        {
            if (maxRadius < 1)
            {
                maxRadius = 1;
            }

            var waves = new List<IReadOnlyList<Point3D>>(maxRadius);
            for (var r = 1; r <= maxRadius; r++)
            {
                waves.Add(RingTiles(origin, r));
            }

            return waves;
        }

        private static Point3D[] RingTiles(Point3D origin, int radius)
        {
            if (radius < 1)
            {
                return new[] { origin };
            }

            // Square (Chebyshev) ring edge.
            var tiles = new List<Point3D>(radius * 8);
            for (var x = -radius; x <= radius; x++)
            {
                for (var y = -radius; y <= radius; y++)
                {
                    if (Math.Max(Math.Abs(x), Math.Abs(y)) == radius)
                    {
                        tiles.Add(new Point3D(origin.X + x, origin.Y + y, origin.Z));
                    }
                }
            }

            return tiles.ToArray();
        }
    }
}
