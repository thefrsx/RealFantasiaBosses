using System;
using Server;
using Server.Commands;
using Server.Custom.Effect;

namespace Server.Custom.Commands
{
    // Dev tool to test FireLineEffect.
    // Usage: [fireline <tiles> <directions> [hurt]
    //   [fireline 5 2      -> 5-tile fire walls in 2 random directions, fired instantly (no targeting).
    //   [fireline 5 4      -> 5-tile fire walls in 4 random directions.
    //   [fireline 5 4 hurt -> same, dealing fire damage.
    // Directions are random (cardinals first, then diagonals); each wall's FX respects its direction.
    public static class FireLineCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("fireline", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("fireline <tiles> <directions> [hurt]")]
        [Description("Dispara paredes de fogo (FX do firewall) em N direcoes aleatorias a partir de voce, sem clicar. Ex: [fireline 5 4")]
        public static void OnCommand(CommandEventArgs e)
        {
            if (e.Length < 2)
            {
                e.Mobile.SendMessage(0x22, "Uso: [fireline <tiles> <direcoes> [hurt]   (ex: [fireline 5 4)");
                return;
            }

            var tiles = e.GetInt32(0);
            var directions = e.GetInt32(1);
            var damage = e.Length >= 3 && e.GetString(2).ToLowerInvariant() == "hurt" ? 20 : 0;

            if (tiles < 1)
            {
                tiles = 1;
            }

            var from = e.Mobile;

            FireLineEffect.ThrowRandom(
                from.Map,
                from.Location,
                tiles,
                directions,
                default,
                damage > 0 ? from : null,
                damage
            );

            e.Mobile.SendMessage(
                0x35,
                $"FireLine: {tiles} tiles em {Math.Clamp(directions, 1, 8)} direcao(oes) aleatoria(s){(damage > 0 ? ", com dano" : "")}."
            );
        }
    }
}
