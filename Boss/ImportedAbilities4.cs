using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Lote 4 — tempestades/AoE: Thunderstorm, FlameBurstAoe, RagingTempest, Blizzard.
    // Reimplementados com APIs do ModernUO (storms persistentes -> dano em área por ticks).

    // Thunderstorm: relâmpagos em todos ao redor do boss (dano de energia).
    public sealed class ThunderstormAbility : RFBossAbility
    {
        public int Range { get; init; } = 8;
        protected override int TeleHue => AbilityFx.HueEnergy;

        public override void Use(Mobile boss, Mobile target)
        {
            var map = boss.Map;
            if (map == null)
            {
                return;
            }

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            foreach (var m in AbilityFx.Targets(boss, boss.Location, map, Range))
            {
                b.DoHarmful(m);
                Effects.PlaySound(m.Location, map, 0x5CE);
                Effects.SendBoltEffect(m, true, 0);
                AOS.Damage(m, b, dmg, 0, 0, 0, 0, 100);
            }
        }
    }

    // FlameBurstAoe: explosão de fogo num raio em volta do boss.
    public sealed class FlameBurstAoeAbility : RFBossAbility
    {
        public int Radius { get; init; } = 5;
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target)
        {
            var map = boss.Map;
            if (map == null)
            {
                return;
            }

            var c = boss.Location;
            Effects.PlaySound(c, map, 0x307);
            AbilityFx.Disc(c, map, Radius, 0x36BD, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(0.75), () =>
            {
                foreach (var m in AbilityFx.Targets(b, c, map, Radius))
                {
                    b.DoHarmful(m);
                    AOS.Damage(m, b, dmg, 0, 100, 0, 0, 0);
                }
            });
        }
    }

    // RagingTempest: tempestade prolongada no local do alvo, atingindo a área repetidamente (energia).
    public sealed class RagingTempestAbility : RFBossAbility
    {
        public int Radius { get; init; } = 6;
        protected override int TeleHue => AbilityFx.HueEnergy;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true })
            {
                return;
            }

            var map = boss.Map;
            if (map == null)
            {
                return;
            }

            var spot = target.Location;
            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            const int ticks = 10; // ~20s @ 2s

            for (var i = 1; i <= ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 2.0), () =>
                {
                    if (b is not { Deleted: false })
                    {
                        return;
                    }

                    AbilityFx.Disc(spot, map, Radius, 0x36BD, 1169);
                    Effects.PlaySound(spot, map, 0x5CE);
                    foreach (var m in AbilityFx.Targets(b, spot, map, Radius))
                    {
                        b.DoHarmful(m);
                        Effects.SendBoltEffect(m, false, 0);
                        AOS.Damage(m, b, dmg, 0, 0, 0, 0, 100);
                    }
                });
            }
        }
    }

    // Blizzard: nevasca num raio em volta do boss, atingindo a área por alguns segundos (gelo).
    public sealed class BlizzardAbility : RFBossAbility
    {
        public int Radius { get; init; } = 5;
        protected override int TeleHue => AbilityFx.HueIce;

        public override void Use(Mobile boss, Mobile target)
        {
            var map = boss.Map;
            if (map == null)
            {
                return;
            }

            var c = boss.Location;
            Effects.PlaySound(c, map, 0x64F);
            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            const int ticks = 5;

            for (var i = 1; i <= ticks; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i * 1.0), () =>
                {
                    if (b is not { Deleted: false })
                    {
                        return;
                    }

                    AbilityFx.Disc(c, map, Radius, 0x3779, 1152);
                    foreach (var m in AbilityFx.Targets(b, c, map, Radius))
                    {
                        b.DoHarmful(m);
                        AOS.Damage(m, b, dmg, 50, 0, 50, 0, 0);
                    }
                });
            }
        }
    }
}
