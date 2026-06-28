using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Lote 2 — telegrafias de chão garimpadas do Custom Abilities 3.0 (Geyser, WalkingBomb, ToxicRain,
    // ToxicSpores, ImpaleAoe). Reimplementadas com as APIs do ModernUO (sem os itens/buff de pool deles).

    internal static class AbilityFx
    {
        // Disco preenchido de efeitos de chão (telegrafia/explosão).
        public static void Disc(Point3D c, Map map, int radius, int itemId, int hue)
        {
            if (map == null)
            {
                return;
            }

            for (var x = -radius; x <= radius; x++)
            {
                for (var y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        Effects.SendLocationEffect(new Point3D(c.X + x, c.Y + y, c.Z), map, itemId, 30, 10, hue, 0);
                    }
                }
            }
        }

        // Gráfico delicado de telegrafia (faísca pequena de chão) + cores por elemento (tingem a faísca).
        public const int TeleSpark = 0x376A;
        public const int HueNeutral = 0;
        public const int HueFire = 0x2B;     // laranja
        public const int HuePoison = 0x42;   // verde
        public const int HueIce = 1152;      // azul gelo
        public const int HueEnergy = 1169;   // roxo/energia

        // Marca de telegrafia num tile (duração curta -> repintada a cada tick = brilho pulsante que some no fim do aviso).
        public static void Mark(Point3D p, Map map, int itemId, int hue)
        {
            if (map != null)
            {
                Effects.SendLocationEffect(p, map, itemId, 9, 10, hue, 0);
            }
        }

        // Disco preenchido de marcas (usar só quando a área toda importa; senão prefira RingMark).
        public static void DiscMark(Point3D c, Map map, int radius, int itemId, int hue)
        {
            if (map == null)
            {
                return;
            }

            for (var x = -radius; x <= radius; x++)
            {
                for (var y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        Mark(new Point3D(c.X + x, c.Y + y, c.Z), map, itemId, hue);
                    }
                }
            }
        }

        // Contorno (anel de 1 tile) — telegrafia DELICADA que mostra a borda da zona de perigo.
        public static void RingMark(Point3D c, Map map, int radius, int itemId, int hue)
        {
            if (map == null)
            {
                return;
            }

            if (radius < 1)
            {
                Mark(c, map, itemId, hue);
                return;
            }

            var inner = (radius - 1) * (radius - 1);
            var outer = radius * radius;
            for (var x = -radius; x <= radius; x++)
            {
                for (var y = -radius; y <= radius; y++)
                {
                    var d2 = x * x + y * y;
                    if (d2 <= outer && d2 > inner)
                    {
                        Mark(new Point3D(c.X + x, c.Y + y, c.Z), map, itemId, hue);
                    }
                }
            }
        }

        // Coleta alvos válidos num raio (evita mutar a query enquanto causa dano).
        public static List<Mobile> Targets(Mobile boss, Point3D c, Map map, int radius)
        {
            var list = new List<Mobile>();
            if (map == null)
            {
                return list;
            }

            foreach (var m in map.GetMobilesInRange(c, radius))
            {
                if (m != boss && boss.CanBeHarmful(m, false))
                {
                    list.Add(m);
                }
            }

            return list;
        }

        // DoT simples por N ticks (sem timer próprio nem BuffInfo).
        public static void Dot(Mobile boss, Mobile target, int ticks, double interval, int perTick,
            int phys, int fire, int cold, int pois, int nrgy, int particleId, int particleHue)
        {
            for (var i = 1; i <= ticks; i++)
            {
                var b = boss;
                var t = target;
                Timer.DelayCall(TimeSpan.FromSeconds(interval * i), () =>
                {
                    if (t is { Alive: true, Deleted: false } && b is { Deleted: false } && b.CanBeHarmful(t, false))
                    {
                        t.FixedParticles(particleId, 10, 20, 5030, particleHue, 0, EffectLayer.Waist);
                        AOS.Damage(t, b, Math.Max(1, perTick), phys, fire, cold, pois, nrgy);
                    }
                });
            }
        }
    }

    // Geyser: telegrafa um redemoinho no tile do alvo e, após o aviso, irrompe e fere quem ficar ali.
    public sealed class GeyserAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueIce; // agua/gelo

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            var spot = target.Location;
            var map = boss.Map;
            Effects.SendLocationEffect(spot, map, 0x3789, 90, 10, 0, 0);
            Effects.PlaySound(spot, map, 0x011);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
            {
                Effects.SendLocationEffect(spot, map, 0x3709, 30, 10, 0, 0);
                Effects.PlaySound(spot, map, 0x208);
                foreach (var m in AbilityFx.Targets(b, spot, map, 1))
                {
                    b.DoHarmful(m);
                    m.SendMessage("O geiser irrompe sob voce!");
                    AOS.Damage(m, b, dmg, 50, 0, 50, 0, 0);
                }
            });
        }
    }

    // WalkingBomb: marca o alvo; após 5s explode num raio em volta dele.
    public sealed class WalkingBombAbility : RFBossAbility
    {
        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true })
            {
                return;
            }

            const int hue = 0x485;
            target.FixedParticles(0x374A, 10, 90, 5052, hue, 0, EffectLayer.Waist);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            Timer.DelayCall(TimeSpan.FromSeconds(5.0), () =>
            {
                if (t.Deleted || t.Map == null)
                {
                    return;
                }

                var c = t.Location;
                var map = t.Map;
                t.FixedParticles(0x36BD, 20, 10, 5044, hue, 0, EffectLayer.Head);
                Effects.PlaySound(c, map, 0x207);
                AbilityFx.Disc(c, map, 3, 0x36BD, hue);
                foreach (var m in AbilityFx.Targets(b, c, map, 3))
                {
                    b.DoHarmful(m);
                    AOS.Damage(m, b, dmg, 20, 20, 20, 20, 20);
                }
            });
        }
    }

    // ToxicRain: nuvem ácida no alvo; após o aviso causa dano de veneno + DoT.
    public sealed class ToxicRainAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HuePoison;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || boss.Map == null)
            {
                return;
            }

            var spot = target.Location;
            var map = boss.Map;
            Effects.PlaySound(spot, map, 0x011);
            Effects.SendLocationParticles(new Entity(Serial.Zero, spot, map), 0x9F89, 10, 30, 64, 0, 5052, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(0.75), () =>
            {
                foreach (var m in AbilityFx.Targets(b, spot, map, 1))
                {
                    b.DoHarmful(m);
                    AOS.Damage(m, b, dmg, 50, 0, 0, 50, 0);
                    AbilityFx.Dot(b, m, 2, 2.0, dmg / 4, 0, 0, 0, 100, 0, 0x9F89, 64);
                }
            });
        }
    }

    // ToxicSpores: anel de esporos ao redor do alvo; após o aviso, dano de veneno em área.
    public sealed class ToxicSporesAbility : RFBossAbility
    {
        public int Radius { get; init; } = 5;
        protected override int TeleHue => AbilityFx.HuePoison;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true })
            {
                return;
            }

            var c = target.Location;
            var map = boss.Map;
            AbilityFx.Disc(c, map, Radius, 0x1126, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
            {
                AbilityFx.Disc(c, map, Radius, 0x36BD, 63);
                foreach (var m in AbilityFx.Targets(b, c, map, Radius))
                {
                    b.DoHarmful(m);
                    AOS.Damage(m, b, dmg, 0, 0, 0, 100, 0);
                }
            });
        }
    }

    // ImpaleAoe: estalagmites irrompem nos alvos ao redor; dano físico + sangramento (DoT).
    public sealed class ImpaleAoeAbility : RFBossAbility
    {
        public int Radius { get; init; } = 5;

        public override void Use(Mobile boss, Mobile target)
        {
            var map = boss.Map;
            if (map == null)
            {
                return;
            }

            Effects.PlaySound(boss.Location, map, 0x21D);
            var stalId = Utility.RandomList(0x8E0, 0x8E7, 0x8E1);
            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;

            foreach (var m in AbilityFx.Targets(boss, boss.Location, map, Radius))
            {
                Effects.SendLocationEffect(m.Location, map, stalId, 150, 10, 0, 0);
                var v = m;
                Timer.DelayCall(TimeSpan.FromSeconds(0.75), () =>
                {
                    if (v is { Alive: true, Deleted: false } && b.CanBeHarmful(v, false))
                    {
                        b.DoHarmful(v);
                        AOS.Damage(v, b, dmg, 100, 0, 0, 0, 0);
                        v.SendLocalizedMessage(1060160); // You are bleeding!
                        v.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
                        AbilityFx.Dot(b, v, 4, 2.0, dmg / 5, 100, 0, 0, 0, 0, 0x377A, 31);
                    }
                });
            }
        }
    }
}
