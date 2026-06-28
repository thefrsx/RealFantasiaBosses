using Server;
using Server.Commands;
using Server.Custom.Effect;

namespace Server.Custom.Commands
{
    // Dev tool to test SpiderEggEffect.
    // Usage: [spidereggs <count> [hatchSeconds]
    //   [spidereggs 6      -> 6 eggs around you, hatch in 8s into spiders unless destroyed.
    //   [spidereggs 6 5    -> hatch in 5s.
    public static class SpiderEggsCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("spidereggs", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("spidereggs <count> [hatchSeconds]")]
        [Description("Espalha ovos de aranha (com HP) ao seu redor; eclodem em aranhas se nao destruidos. Ex: [spidereggs 6")]
        public static void OnCommand(CommandEventArgs e)
        {
            var count = e.Length >= 1 ? e.GetInt32(0) : 6;
            var hatch = 8.0;

            if (e.Length >= 2 &&
                double.TryParse(
                    e.GetString(1),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var h))
            {
                hatch = h;
            }

            if (count < 1)
            {
                count = 1;
            }

            if (hatch < 0.5)
            {
                hatch = 0.5;
            }

            var from = e.Mobile;

            SpiderEggEffect.Scatter(from.Map, from.Location, count, 4, hatch, 3);

            e.Mobile.SendMessage(0x35, $"SpiderEggs: {count} ovos, eclodem em {hatch}s.");
        }
    }
}
