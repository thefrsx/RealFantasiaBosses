using System;
using Server;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Lote 3 — puxar/arremesso garimpados do Custom Abilities 3.0 (Grapple, ThrowBoulder, ThrowExplosives,
    // ThrowTimedExplosives, BarrageOfBolts). Reimplementados com APIs do ModernUO.

    // Grapple: paralisa o alvo, puxa-o até o boss e golpeia.
    public sealed class GrappleAbility : RFBossAbility
    {
        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            target.Paralyzed = true;
            boss.Say("*puxa voce para si!*");

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            Timer.DelayCall(TimeSpan.FromSeconds(0.75), () =>
            {
                if (t is not { Deleted: false } || b is not { Deleted: false } || b.Map == null)
                {
                    if (t is { Deleted: false })
                    {
                        t.Paralyzed = false;
                    }

                    return;
                }

                t.MoveToWorld(b.Location, b.Map);
                t.Paralyzed = false;

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

    // ThrowBoulder: arremessa uma rocha no local do alvo; no impacto causa dano e derruba (freeze 2s) em volta.
    public sealed class ThrowBoulderAbility : RFBossAbility
    {
        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            var spot = target.Location;
            var map = boss.Map;
            boss.MovingEffect(target, 0x1363, 1, 0, true, false, 0, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(b.GetDistanceToSqrt(spot) / 8.0), () =>
            {
                Effects.SendLocationParticles(new Entity(Serial.Zero, spot, map), 0x3728, 10, 20, 0, 0, 5052, 0);
                foreach (var m in AbilityFx.Targets(b, spot, map, 1))
                {
                    b.DoHarmful(m);
                    m.SendMessage("Voce foi atingido por uma rocha enorme!");
                    Effects.PlaySound(spot, map, 0x308);
                    AOS.Damage(m, b, dmg, 100, 0, 0, 0, 0);

                    var v = m;
                    v.Frozen = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                    {
                        if (v is { Deleted: false })
                        {
                            v.Frozen = false;
                        }
                    });
                }
            });
        }
    }

    // ThrowExplosives: arremessa um explosivo no alvo; no impacto, dano de fogo na área.
    public sealed class ThrowExplosivesAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            var map = boss.Map;
            var spot = target.Location;
            target.PlaySound(0x15E);
            boss.MovingParticles(target, 0x1C19, 1, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(b.GetDistanceToSqrt(spot) / 5.0), () =>
            {
                Effects.SendLocationEffect(spot, map, 0x36BD, 20, 10, 0, 0);
                foreach (var m in AbilityFx.Targets(b, spot, map, 1))
                {
                    b.DoHarmful(m);
                    AOS.Damage(m, b, dmg, 0, 100, 0, 0, 0);
                }
            });
        }
    }

    // ThrowTimedExplosives: arremessa um barril que cai e detona após 3s (dá pra correr) em área.
    public sealed class ThrowTimedExplosivesAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            var spot = target.Location;
            var map = boss.Map;
            boss.MovingEffect(target, 0x0FAE, 5, 0, false, false, 0, 0);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(b.GetDistanceToSqrt(spot) / 10.0), () =>
            {
                Effects.SendLocationEffect(spot, map, 0x0FAE, 30, 10, 0, 0);
                Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
                {
                    Effects.PlaySound(spot, map, 0x307);
                    AbilityFx.Disc(spot, map, 2, 0x36BD, 0);
                    foreach (var m in AbilityFx.Targets(b, spot, map, 2))
                    {
                        b.DoHarmful(m);
                        AOS.Damage(m, b, dmg, 0, 100, 0, 0, 0);
                    }
                });
            });
        }
    }

    // BarrageOfBolts: rajada de projéteis convergindo na área do alvo, com dano em volta.
    public sealed class BarrageOfBoltsAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            var spot = target.Location;
            var map = boss.Map;
            Effects.PlaySound(boss.Location, map, 0x349);

            for (var i = 0; i < 4; i++)
            {
                var b2 = boss;
                Timer.DelayCall(TimeSpan.FromMilliseconds(i * 150),
                    () => b2.MovingEffect(new Entity(Serial.Zero, spot, map), 0x36D4, 7, 0, false, true, 0, 0));
            }

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            Timer.DelayCall(TimeSpan.FromSeconds(0.75), () =>
            {
                foreach (var m in AbilityFx.Targets(b, spot, map, 3))
                {
                    b.DoHarmful(m);
                    AOS.Damage(m, b, dmg, 100, 0, 0, 0, 0);
                }
            });
        }
    }
}
