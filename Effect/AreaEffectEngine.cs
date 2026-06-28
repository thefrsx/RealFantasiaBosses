using System;
using System.Collections.Generic;
using Server;

namespace Server.Custom.Effect
{
    // Generic timed area-effect runner — the core building block for telegraphed AoE boss mechanics.
    // A mechanic = a list of "waves" (each a set of tiles that activate together) + a per-tile action.
    // Waves fire in order with waveDelay between them, which produces travelling/expanding animations.
    //
    // Compose with AreaShapes (the geometry) and TileFx (the per-tile FX/damage) to author many mechanics.
    public static class AreaEffectEngine
    {
        public delegate void TileAction(Map map, Point3D tile);

        public static void Run(
            Map map,
            IReadOnlyList<IReadOnlyList<Point3D>> waves,
            TimeSpan waveDelay,
            TileAction action
        )
        {
            if (map == null || map == Map.Internal || action == null || waves == null || waves.Count == 0)
            {
                return;
            }

            if (waveDelay < TimeSpan.FromMilliseconds(1))
            {
                waveDelay = TimeSpan.FromMilliseconds(1);
            }

            new WaveTimer(map, waves, waveDelay, action).Start();
        }

        // Damage everyone standing on a tile (fire damage). Shared helper for tile actions.
        public static void DamageTile(Map map, Point3D tile, Mobile source, int damage)
        {
            if (map == null || source == null || damage <= 0)
            {
                return;
            }

            // Collect first — don't harm while enumerating the spatial query.
            List<Mobile> targets = null;
            foreach (var m in map.GetMobilesInRange(tile, 0))
            {
                if (m == source || !source.CanBeHarmful(m, false))
                {
                    continue;
                }

                (targets ??= new List<Mobile>()).Add(m);
            }

            if (targets == null)
            {
                return;
            }

            foreach (var m in targets)
            {
                source.DoHarmful(m);
                AOS.Damage(m, source, damage, 0, 100, 0, 0, 0);
            }
        }

        private class WaveTimer : Timer
        {
            private readonly Map _map;
            private readonly IReadOnlyList<IReadOnlyList<Point3D>> _waves;
            private readonly TileAction _action;
            private int _index;

            public WaveTimer(Map map, IReadOnlyList<IReadOnlyList<Point3D>> waves, TimeSpan delay, TileAction action)
                : base(TimeSpan.Zero, delay, waves.Count)
            {
                _map = map;
                _waves = waves;
                _action = action;
            }

            protected override void OnTick()
            {
                if (_index >= _waves.Count)
                {
                    Stop();
                    return;
                }

                var wave = _waves[_index++];
                for (var i = 0; i < wave.Count; i++)
                {
                    _action(_map, wave[i]);
                }
            }
        }
    }
}
