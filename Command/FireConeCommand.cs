using System;
using Server;
using Server.Commands;
using Server.Custom.Effect;

namespace Server.Custom.Commands
{
    // Dev tool to test FireConeEffect.
    // Usage: [firecone <length> [hurt]
    //   Breathes a widening fan of fire in the direction you are FACING (no targeting).
    //   [firecone 6      -> 6-tile cone toward your facing.
    //   [firecone 6 hurt -> same, dealing fire damage.
    public static class FireConeCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("firecone", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("firecone <length> [hurt]")]
        [Description("Dispara um cone/sopro de fogo na direcao em que voce esta virado, sem clicar. Ex: [firecone 6")]
        public static void OnCommand(CommandEventArgs e)
        {
            if (e.Length < 1)
            {
                e.Mobile.SendMessage(0x22, "Uso: [firecone <comprimento> [hurt]   (ex: [firecone 6)");
                return;
            }

            var length = e.GetInt32(0);
            var damage = e.Length >= 2 && e.GetString(1).ToLowerInvariant() == "hurt" ? 20 : 0;

            if (length < 1)
            {
                length = 1;
            }

            var from = e.Mobile;

            FireConeEffect.Breath(
                from.Map,
                from.Location,
                from.Direction,
                length,
                default,
                damage > 0 ? from : null,
                damage
            );

            e.Mobile.SendMessage(
                0x35,
                $"FireCone: sopro de {length} tiles na direcao {FireConeEffect.SnapToCardinal(from.Direction)}{(damage > 0 ? ", com dano" : "")}."
            );
        }
    }
}
