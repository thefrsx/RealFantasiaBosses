using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom.Effect
{
    // "Falling fire" telegraph mechanic:
    //   1) a shadow pulses on the ground for <telegraphSeconds> (the tell — players step off),
    //   2) a fireball crashes down, exploding for impact damage,
    //   3) the tile keeps burning for <burnSeconds> (lingering hazard).
    // Built on the same AreaEffectEngine + TileFx.Flame as the other mechanics.
    public static class MeteorEffect
    {
        private const int ShadowItem = 0x374A; // swirl particle, hued dark = shadow
        private const int ShadowHue = 1;
        private const int FireballItem = 0x36D4;
        private const int ExplosionItem = 0x36BD;
        private const int ExplosionSound = 0x307;
        private const int FlameField = 0x398C; // chama de fire field = chão em chamas persistente
        private const int FireSound = 0x208;

        // Single meteor onto one tile.
        public static void Drop(
            Map map,
            Point3D target,
            double telegraphSeconds = 1.5,
            int burnSeconds = 5,
            Mobile source = null,
            int impactDamage = 0,
            int burnDamage = 0
        )
        {
            if (map == null || map == Map.Internal)
            {
                return;
            }

            if (telegraphSeconds < 0.2)
            {
                telegraphSeconds = 0.2;
            }

            // 1) Telegraph: pulse a shadow on the ground during the wind-up.
            var pulses = Math.Max(1, (int)Math.Round(telegraphSeconds / 0.4));
            var waves = new List<IReadOnlyList<Point3D>>(pulses);
            for (var i = 0; i < pulses; i++)
            {
                waves.Add(new[] { target });
            }

            AreaEffectEngine.Run(map, waves, TimeSpan.FromSeconds(telegraphSeconds / pulses), ShadowPulse);

            // 2) Impact after the telegraph.
            Timer.DelayCall(
                TimeSpan.FromSeconds(telegraphSeconds),
                () => Impact(map, target, burnSeconds, source, impactDamage, burnDamage)
            );
        }

        // Boss "meteor shower": <count> meteors on random tiles within <radius> of <origin>.
        public static void Shower(
            Map map,
            Point3D origin,
            int count,
            int radius = 4,
            double telegraphSeconds = 1.5,
            int burnSeconds = 5,
            Mobile source = null,
            int impactDamage = 0,
            int burnDamage = 0
        )
        {
            count = Math.Clamp(count, 1, 40);

            if (radius < 0)
            {
                radius = 0;
            }

            for (var i = 0; i < count; i++)
            {
                var x = origin.X + Utility.RandomMinMax(-radius, radius);
                var y = origin.Y + Utility.RandomMinMax(-radius, radius);
                Drop(
                    map,
                    new Point3D(x, y, origin.Z),
                    telegraphSeconds,
                    burnSeconds,
                    source,
                    impactDamage,
                    burnDamage
                );
            }
        }

        private static void ShadowPulse(Map map, Point3D tile)
        {
            Effects.SendLocationEffect(tile, map, ShadowItem, 14, 9, ShadowHue);
        }

        private static void Impact(Map map, Point3D target, int burnSeconds, Mobile source, int impactDamage, int burnDamage)
        {
            // Fireball crashing from the sky down onto the tile.
            var high = new Point3D(target.X, target.Y, target.Z + 50);
            var from = EffectItem.Create(high, map, EffectItem.DefaultDuration);
            var to = EffectItem.Create(target, map, EffectItem.DefaultDuration);
            Effects.SendMovingEffect(from, to, FireballItem, 7, 0, false, false, 0, 0);

            // When it lands: explosion + impact damage, then leave the tile burning.
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                Effects.SendLocationEffect(target, map, ExplosionItem, 16, 9);
                Effects.PlaySound(target, map, ExplosionSound);
                AreaEffectEngine.DamageTile(map, target, source, impactDamage);

                if (burnSeconds > 0)
                {
                    LingeringFire(map, target, burnSeconds, source, burnDamage);
                }
            });
        }

        // Chão em chamas: o tile fica pegando fogo por <durationSeconds>, com TICK DE DANO a cada 0,5s
        // (pega quem ficar OU passar por cima). A chama é repintada a cada 1s pra continuar visível.
        private static void LingeringFire(Map map, Point3D tile, int durationSeconds, Mobile source, int tickDamage)
        {
            if (map == null || map == Map.Internal || durationSeconds <= 0)
            {
                return;
            }

            var ticks = durationSeconds * 2; // cadência de 0,5s
            var n = 0;
            Timer.StartTimer(TimeSpan.Zero, TimeSpan.FromSeconds(0.5), ticks, () =>
            {
                if (n % 2 == 0)
                {
                    Effects.SendLocationEffect(tile, map, FlameField, 30, 9); // repinta a chama (1x/s)
                }

                if (n % 4 == 0)
                {
                    Effects.PlaySound(tile, map, FireSound); // som de fogo esporádico (não a cada tick)
                }

                AreaEffectEngine.DamageTile(map, tile, source, tickDamage);
                n++;
            }, out _);
        }
    }
}
