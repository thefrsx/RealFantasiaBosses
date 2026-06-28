using Server;
using Server.Commands;
using Server.Custom.Effect;

namespace Server.Custom.Commands
{
    // Dev tool to test FireNovaEffect.
    // Usage: [firenova <radius> [hurt]
    //   [firenova 5       -> ring of fire expanding outward to radius 5 (no targeting).
    //   [firenova 5 hurt  -> same, dealing fire damage.
    public static class FireNovaCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("firenova", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("firenova <radius> [hurt]")]
        [Description("Dispara um nova de fogo expandindo a partir de voce, sem clicar. Ex: [firenova 5")]
        public static void OnCommand(CommandEventArgs e)
        {
            var radius = e.Length >= 1 ? e.GetInt32(0) : 5;
            var damage = e.Length >= 2 && e.GetString(1).ToLowerInvariant() == "hurt" ? 20 : 0;

            if (radius < 1)
            {
                radius = 1;
            }

            var from = e.Mobile;

            FireNovaEffect.Burst(
                from.Map,
                from.Location,
                radius,
                default,
                damage > 0 ? from : null,
                damage
            );

            e.Mobile.SendMessage(0x35, $"FireNova: raio {radius}{(damage > 0 ? ", com dano" : "")}.");
        }
    }
}
