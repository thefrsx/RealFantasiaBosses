using System;
using Server;

namespace Server.Custom.Effect
{
    // Reusable per-tile "painters": what visually/mechanically happens on one tile.
    // Compose these with AreaShapes + AreaEffectEngine to build many mechanics.
    // (Add Ice/Poison/Energy painters here the same way to get those elements for free.)
    public static class TileFx
    {
        private const int FlameEastWest = 0x398C; // fire field flame, line along X
        private const int FlameNorthSouth = 0x3996; // fire field flame, line along Y
        private const int FlameSpeed = 9;
        private const int FlameDuration = 50;
        private const int FireSound = 0x208;

        // Fire Field flame whose orientation respects each tile's direction from <origin>.
        // Works for lines, stars/crosses, rings and novas — the orientation is derived per tile.
        public static AreaEffectEngine.TileAction Flame(Point3D origin, Mobile source = null, int damage = 0)
        {
            return (map, tile) =>
            {
                var dx = tile.X - origin.X;
                var dy = tile.Y - origin.Y;
                var id = Math.Abs(dx) >= Math.Abs(dy) ? FlameEastWest : FlameNorthSouth;

                Effects.SendLocationEffect(tile, map, id, FlameDuration, FlameSpeed);
                Effects.PlaySound(tile, map, FireSound);
                AreaEffectEngine.DamageTile(map, tile, source, damage);
            };
        }
    }
}
