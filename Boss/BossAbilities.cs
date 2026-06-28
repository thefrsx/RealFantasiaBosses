using Server;
using Server.Custom.Effect;

namespace Server.Custom.Bosses
{
    // Habilidades concretas = embrulho fino das mecânicas de FX. Agnósticas de classe (recebem Mobile boss),
    // então podem ser carregadas em qualquer mobile via BossController.

    public sealed class FireConeAbility : RFBossAbility
    {
        public int Length { get; init; } = 6;
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target)
        {
            boss.Direction = boss.GetDirectionTo(target);
            FireConeEffect.Breath(boss.Map, boss.Location, boss.Direction, Length, default, boss, Damage);
        }

        // Telegrafia: leque que abre na direcao do alvo (formato real do sopro).
        public override void PaintTelegraph(Mobile boss, Mobile target)
        {
            var map = boss.Map;
            if (map == null || target == null)
            {
                return;
            }

            var (dx, dy) = FsHelper.Delta(boss.GetDirectionTo(target));
            var px = -dy;
            var py = dx;
            for (var k = 1; k <= Length; k++)
            {
                var hw = k / 2;
                for (var w = -hw; w <= hw; w++)
                {
                    AbilityFx.Mark(new Point3D(boss.X + dx * k + px * w, boss.Y + dy * k + py * w, boss.Z), map, TeleId, TeleHue);
                }
            }
        }
    }

    public sealed class FireLineAbility : RFBossAbility
    {
        public int Tiles { get; init; } = 6;
        public int Dirs { get; init; } = 4;
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target) =>
            FireLineEffect.ThrowRandom(boss.Map, boss.Location, Tiles, Dirs, default, boss, Damage);

        // Telegrafia: contorno da zona em volta do boss (as linhas saem dele em direcoes aleatorias).
        public override void PaintTelegraph(Mobile boss, Mobile target) =>
            AbilityFx.RingMark(boss.Location, boss.Map, Tiles, TeleId, TeleHue);
    }

    public sealed class FireNovaAbility : RFBossAbility
    {
        public int Radius { get; init; } = 6;
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target) =>
            FireNovaEffect.Burst(boss.Map, boss.Location, Radius, default, boss, Damage);

        // Telegrafia: anel (contorno) da nova em volta do boss.
        public override void PaintTelegraph(Mobile boss, Mobile target) =>
            AbilityFx.RingMark(boss.Location, boss.Map, Radius, TeleId, TeleHue);
    }

    public sealed class MeteorAbility : RFBossAbility
    {
        public int Count { get; init; } = 6;
        protected override int TeleHue => AbilityFx.HueFire;

        public override void Use(Mobile boss, Mobile target) =>
            MeteorEffect.Shower(boss.Map, target.Location, Count, 4, 1.5, 6, boss, Damage, Damage / 3);

        // Telegrafia: contorno da zona de impacto em volta do alvo (os meteoros caem ali).
        public override void PaintTelegraph(Mobile boss, Mobile target)
        {
            if (target != null)
            {
                AbilityFx.RingMark(target.Location, boss.Map, 4, TeleId, TeleHue);
            }
        }
    }

    public sealed class SpiderEggAbility : RFBossAbility
    {
        public int Count { get; init; } = 4;

        public override void Use(Mobile boss, Mobile target) =>
            SpiderEggEffect.Scatter(boss.Map, boss.Location, Count, 4, 8.0, 3);
    }
}
