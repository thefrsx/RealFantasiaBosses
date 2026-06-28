using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Habilidades GARIMPADAS do Custom Abilities 3.0 (Massapequa, ServUO) e portadas pro NOSSO framework
    // (RFBossAbility). Cada uma vira entrada no BossCatalog -> aparece no [bossload com config + telegrafia.
    // Lote 1.

    // Firebolt: projétil elemental (fogo) que viaja até o alvo e causa dano.
    public sealed class FireboltAbility : RFBossAbility
    {
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true } || !boss.CanBeHarmful(target))
            {
                return;
            }

            boss.DoHarmful(target);
            boss.MovingEffect(target, 0x36D4, 7, 0, false, true, 0, 0);
            target.PlaySound(0x15E);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            Timer.DelayCall(TimeSpan.FromSeconds(b.GetDistanceToSqrt(t) / 5.0), () =>
            {
                if (t is { Alive: true, Deleted: false } && b.CanBeHarmful(t))
                {
                    AOS.Damage(t, b, dmg, 0, 100, 0, 0, 0);
                }
            });
        }
    }

    // Investida (Charge): o boss salta até o alvo e golpeia.
    public sealed class ChargeAbility : RFBossAbility
    {
        public override void Use(Mobile boss, Mobile target)
        {
            if (target is not { Alive: true })
            {
                return;
            }

            Effects.SendLocationEffect(boss.Location, boss.Map, 0x3728, 10, 10);
            boss.PlaySound(0x2F3);

            var dmg = Utility.RandomMinMax(Math.Max(1, Damage * 3 / 4), Math.Max(1, Damage));
            var b = boss;
            var t = target;
            Timer.DelayCall(TimeSpan.FromSeconds(0.6), () =>
            {
                if (b.Deleted || t.Deleted || t.Map != b.Map)
                {
                    return;
                }

                b.MoveToWorld(t.Location, t.Map);
                Effects.SendLocationEffect(t.Location, t.Map, 0x3728, 10, 10);

                if (t.Alive && b.CanBeHarmful(t))
                {
                    b.DoHarmful(t);
                    AOS.Damage(t, b, dmg, 100, 0, 0, 0, 0);
                }
            });
        }
    }

    // Medo (Fear): paralisa de medo todos em volta por alguns segundos.
    public sealed class FearAbility : RFBossAbility
    {
        public int Range { get; init; } = 8;

        public override void Use(Mobile boss, Mobile target)
        {
            var victims = new List<Mobile>();
            foreach (var m in boss.GetMobilesInRange(Range))
            {
                if (m != boss && (m is PlayerMobile || m is BaseCreature) && boss.CanBeHarmful(m))
                {
                    victims.Add(m);
                }
            }

            foreach (var m in victims)
            {
                m.FixedParticles(0x3779, 9, 30, 5052, EffectLayer.Waist);
                m.PlaySound(0x19C);
                boss.DoHarmful(m);
                m.Paralyzed = true;
                m.SendMessage(0x22, "Voce e paralisado de medo!");

                var v = m;
                Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
                {
                    if (v is { Deleted: false })
                    {
                        v.Paralyzed = false;
                    }
                });
            }
        }
    }
}
