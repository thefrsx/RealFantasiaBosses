using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Lote 6 (final) — variações elementais de FlameStrike (Targeted, Aoe, Line, Cone, Forked),
    // com escolha de ELEMENTO (1 Fogo, 2 Gelo, 3 Veneno, 4 Energia, 5 Agua, 6 Vapor, 7 Necrotico, 8 Sagrado, 9 Sangue).

    internal static class Element
    {
        public static int Hue(int idx) => idx switch
        {
            2 => 1151, 3 => 1366, 4 => 1169, 5 => 1365, 6 => 2103, 7 => 1174, 8 => 1280, 9 => 1156, _ => 0
        };

        public static void Damage(Mobile target, Mobile from, int dmg, int idx)
        {
            switch (idx)
            {
                case 2: AOS.Damage(target, from, dmg, 0, 0, 100, 0, 0); break;   // Gelo
                case 3: AOS.Damage(target, from, dmg, 0, 0, 0, 100, 0); break;   // Veneno
                case 4: AOS.Damage(target, from, dmg, 0, 0, 0, 0, 100); break;   // Energia
                case 5: AOS.Damage(target, from, dmg, 50, 0, 50, 0, 0); break;   // Agua
                case 6: AOS.Damage(target, from, dmg, 50, 50, 0, 0, 0); break;   // Vapor
                case 7: AOS.Damage(target, from, dmg, 50, 0, 0, 50, 0); break;   // Necrotico
                case 8: AOS.Damage(target, from, dmg, 50, 0, 0, 0, 50); break;   // Sagrado
                case 9: AOS.Damage(target, from, dmg, 100, 0, 0, 0, 0); break;   // Sangue (fisico)
                default: AOS.Damage(target, from, dmg, 0, 100, 0, 0, 0); break;  // Fogo
            }
        }
    }

    internal static class FsHelper
    {
        public static (int dx, int dy) Delta(Direction d) => (d & Direction.Mask) switch
        {
            Direction.North => (0, -1),
            Direction.Right => (1, -1),
            Direction.East => (1, 0),
            Direction.Down => (1, 1),
            Direction.South => (0, 1),
            Direction.Left => (-1, 1),
            Direction.West => (-1, 0),
            Direction.Up => (-1, -1),
            _ => (0, 1)
        };

        public static void Strike(Mobile boss, int x, int y, Map map, int idx, int dmg)
        {
            var z = map.GetAverageZ(x, y);
            var p = new Point3D(x, y, z);
            Effects.SendLocationEffect(p, map, 0x3709, 30, 10, Element.Hue(idx), 0);
            foreach (var m in AbilityFx.Targets(boss, p, map, 0))
            {
                boss.DoHarmful(m);
                Element.Damage(m, boss, dmg, idx);
            }
        }

        // Cascata de fogo em linha a partir de (ox,oy) na direção (dx,dy), 1 tile por 200ms.
        public static void CascadeLine(Mobile boss, int ox, int oy, int dx, int dy, int len, Map map, int idx, int dmg)
        {
            for (var k = 1; k <= len; k++)
            {
                var kk = k;
                Timer.DelayCall(TimeSpan.FromMilliseconds(k * 200), () =>
                {
                    if (boss is { Deleted: false })
                    {
                        Strike(boss, ox + dx * kk, oy + dy * kk, map, idx, dmg);
                    }
                });
            }
        }
    }

    // Base elemental: adiciona a escolha de elemento.
    public abstract class ElementalAbility : RFBossAbility
    {
        public int Elem { get; init; } = 1;
        protected override int TeleHue => Element.Hue(Elem); // telegrafia na cor do elemento escolhido
    }

    // FlameStrikeTargeted: golpe único no alvo.
    public sealed class FlameStrikeTargetedAbility : ElementalAbility
    {
        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            target.FixedParticles(0x3709, 10, 30, 5052, Element.Hue(Elem), 0, EffectLayer.LeftFoot);
            target.PlaySound(0x208);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            var idx = Elem;
            Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
            {
                if (t is { Alive: true, Deleted: false } && b is { Deleted: false } && b.CanBeHarmful(t, false))
                {
                    b.DoHarmful(t);
                    Element.Damage(t, b, dmg, idx);
                }
            });
        }
    }

    // FlameStrikeAoe: golpe em círculo ao redor do boss.
    public sealed class FlameStrikeAoeAbility : ElementalAbility
    {
        public int Radius { get; init; } = 5;

        public override void Use(Mobile boss, Mobile target)
        {
            var map = boss.Map;
            if (map == null)
            {
                return;
            }

            var c = boss.Location;
            Effects.PlaySound(c, map, 0x349);
            AbilityFx.Disc(c, map, Radius, 0x3709, Element.Hue(Elem));

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var idx = Elem;
            var r = Radius;
            Timer.DelayCall(TimeSpan.FromSeconds(0.75), () =>
            {
                foreach (var m in AbilityFx.Targets(b, c, map, r))
                {
                    b.DoHarmful(m);
                    Element.Damage(m, b, dmg, idx);
                }
            });
        }
    }

    // FlameStrikeLine: linha cascateante do boss em direção ao alvo.
    public sealed class FlameStrikeLineAbility : ElementalAbility
    {
        public int Length { get; init; } = 5;

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

            Effects.PlaySound(boss.Location, map, 0x349);
            var (dx, dy) = FsHelper.Delta(boss.GetDirectionTo(target));
            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            FsHelper.CascadeLine(boss, boss.X, boss.Y, dx, dy, Length, map, Elem, dmg);
        }
    }

    // FlameStrikeCone: cone que se abre na frente do boss em direção ao alvo.
    public sealed class FlameStrikeConeAbility : ElementalAbility
    {
        public int Length { get; init; } = 5;

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

            Effects.PlaySound(boss.Location, map, 0x349);
            var (dx, dy) = FsHelper.Delta(boss.GetDirectionTo(target));
            var px = -dy;
            var py = dx;
            var ox = boss.X;
            var oy = boss.Y;
            var idx = Elem;
            var b = boss;
            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));

            for (var k = 1; k <= Length; k++)
            {
                var kk = k;
                var hw = k / 2;
                Timer.DelayCall(TimeSpan.FromMilliseconds(k * 200), () =>
                {
                    if (b is not { Deleted: false })
                    {
                        return;
                    }

                    for (var w = -hw; w <= hw; w++)
                    {
                        FsHelper.Strike(b, ox + dx * kk + px * w, oy + dy * kk + py * w, map, idx, dmg);
                    }
                });
            }
        }
    }

    // ForkedFlameStrike: três jatos divergentes (centro + 45° para cada lado) após um pequeno aviso.
    public sealed class ForkedFlameStrikeAbility : ElementalAbility
    {
        public int Length { get; init; } = 6;

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

            boss.Say("*respira fundo...*");
            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            var len = Length;
            var idx = Elem;
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
            {
                if (b is not { Deleted: false } || t is not { Alive: true })
                {
                    return;
                }

                Effects.PlaySound(b.Location, map, 0x349);
                var d = b.GetDirectionTo(t) & Direction.Mask;
                var dl = (Direction)(((int)d + 7) & 7);
                var dr = (Direction)(((int)d + 1) & 7);

                var (cx, cy) = FsHelper.Delta(d);
                var (lx, ly) = FsHelper.Delta(dl);
                var (rx, ry) = FsHelper.Delta(dr);

                FsHelper.CascadeLine(b, b.X, b.Y, cx, cy, len, map, idx, dmg);
                FsHelper.CascadeLine(b, b.X, b.Y, lx, ly, len, map, idx, dmg);
                FsHelper.CascadeLine(b, b.X, b.Y, rx, ry, len, map, idx, dmg);
            });
        }
    }
}
