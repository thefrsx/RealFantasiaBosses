using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Lote 5 — Zap, Snare, IcePrison, Ambush, SummonMinion, HealAllies.

    // Zap: relâmpago no alvo (energia) + paralisia 3s.
    public sealed class ZapAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueEnergy;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            target.PlaySound(0x2F4);
            boss.MovingEffect(target, 0x3818, 7, 0, false, false, 0, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            Timer.DelayCall(TimeSpan.FromSeconds(b.GetDistanceToSqrt(t) / 8.0), () =>
            {
                if (t is not { Alive: true, Deleted: false } || b is not { Deleted: false } || !b.CanBeHarmful(t, false))
                {
                    return;
                }

                b.DoHarmful(t);
                AOS.Damage(t, b, dmg, 0, 0, 0, 0, 100);
                t.Paralyzed = true;
                Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
                {
                    if (t is { Deleted: false })
                    {
                        t.Paralyzed = false;
                    }
                });
            });
        }
    }

    // Snare: projétil que prende o alvo numa teia (energia + paralisia 5s).
    public sealed class SnareAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueEnergy;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            target.PlaySound(0x145);
            boss.MovingEffect(target, 0x3818, 7, 0, false, false, 0, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            Timer.DelayCall(TimeSpan.FromSeconds(b.GetDistanceToSqrt(t) / 8.0), () =>
            {
                if (t is not { Alive: true, Deleted: false } || b is not { Deleted: false } || !b.CanBeHarmful(t, false))
                {
                    return;
                }

                b.DoHarmful(t);
                AOS.Damage(t, b, dmg, 0, 0, 0, 0, 100);
                t.Paralyzed = true;
                t.FixedParticles(0xEE6, 9, 320, 5030, EffectLayer.LeftFoot);
                Timer.DelayCall(TimeSpan.FromSeconds(5.0), () =>
                {
                    if (t is { Deleted: false })
                    {
                        t.Paralyzed = false;
                    }
                });
            });
        }
    }

    // IcePrison: prende o alvo num bloco de gelo (imóvel/invulnerável) por 8s; ao quebrar, dano (se configurado).
    public sealed class IcePrisonAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueIce;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            var map = boss.Map;
            var t = target;
            var b = boss;
            t.Frozen = true;
            t.Hidden = true;
            t.Blessed = true;

            Effects.PlaySound(t.Location, map, 0x64F);
            AbilityFx.Disc(t.Location, map, 1, 0x35F7, 0xAA8);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            Timer.DelayCall(TimeSpan.FromSeconds(8.0), () =>
            {
                t.Frozen = false;
                t.Hidden = false;
                t.Blessed = false;

                if (Damage > 0 && t is { Alive: true, Deleted: false } && b is { Deleted: false } && b.CanBeHarmful(t, false))
                {
                    b.DoHarmful(t);
                    AOS.Damage(t, b, dmg, 100, 0, 0, 0, 0);
                }
            });
        }
    }

    // Ambush: o boss some e reaparece em cima do alvo após 3-6s, golpeando.
    public sealed class AmbushAbility : RFBossAbility
    {
        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true })
            {
                return;
            }

            boss.Hidden = true;
            boss.Frozen = true;
            Effects.SendLocationParticles(new Entity(Serial.Zero, boss.Location, boss.Map), 0x3728, 10, 30, 0, 0, 5052, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(3, 6)), () =>
            {
                b.Hidden = false;
                b.Frozen = false;

                if (b.Deleted || t.Deleted || t.Map != b.Map)
                {
                    return;
                }

                b.MoveToWorld(t.Location, t.Map);
                if (t.Alive && b.CanBeHarmful(t, false))
                {
                    b.DoHarmful(t);
                    AOS.Damage(t, b, dmg, 100, 0, 0, 0, 0);
                    b.Combatant = t;
                    b.Warmode = true;
                }
            });
        }
    }

    // SummonMinion: invoca Count lacaios (BossMinion) que atacam o alvo; somem após 60s.
    public sealed class SummonMinionAbility : RFBossAbility
    {
        public int Count { get; init; } = 1;
        public int MinionBody { get; init; } = 50;

        public override void Use(Mobile boss, Mobile target)
        {
            if (boss is not BaseCreature bc || boss.Map == null)
            {
                return;
            }

            var map = boss.Map;
            for (var i = 0; i < Count; i++)
            {
                var x = boss.X + Utility.RandomMinMax(-2, 2);
                var y = boss.Y + Utility.RandomMinMax(-2, 2);
                var z = map.GetAverageZ(x, y);
                var loc = map.CanFit(x, y, z, 16, false, false) ? new Point3D(x, y, z) : boss.Location;

                var min = new BossMinion();
                if (MinionBody > 0)
                {
                    min.Body = MinionBody;
                }

                min.Team = bc.Team;
                min.MoveToWorld(loc, map);
                Effects.SendLocationParticles(new Entity(Serial.Zero, loc, map), 0x3789, 10, 30, 0x3F, 0, 5052, 0);
                min.PlaySound(0x1FB);

                if (target is { Alive: true })
                {
                    min.Combatant = target;
                }

                var mm = min;
                Timer.DelayCall(TimeSpan.FromSeconds(60.0), () =>
                {
                    if (mm is { Deleted: false })
                    {
                        mm.Delete();
                    }
                });
            }
        }
    }

    // HealAllies: cura o boss e os lacaios próximos.
    public sealed class HealAlliesAbility : RFBossAbility
    {
        public int Range { get; init; } = 8;

        public override void Use(Mobile boss, Mobile target)
        {
            var heal = Math.Max(1, Damage);
            foreach (var m in boss.GetMobilesInRange(Range))
            {
                if ((m == boss || m is BossMinion) && m.Alive && m.Hits < m.HitsMax)
                {
                    m.Hits = Math.Min(m.HitsMax, m.Hits + heal);
                    m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    m.PlaySound(0x202);
                }
            }
        }
    }
}
