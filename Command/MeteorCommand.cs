using Server;
using Server.Commands;
using Server.Custom.Effect;

namespace Server.Custom.Commands
{
    // Dev tool to test MeteorEffect.
    // Usage: [meteor <count> [hurt]
    //   [meteor 1       -> one falling-fire meteor near you (shadow -> fireball -> 5s burn).
    //   [meteor 6 hurt  -> a 6-meteor shower dealing damage.
    public static class MeteorCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("meteor", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("meteor <count> [hurt]")]
        [Description("Dispara meteoro(s) de fogo (sombra -> bola de fogo -> chao em chamas) ao seu redor. Ex: [meteor 6")]
        public static void OnCommand(CommandEventArgs e)
        {
            var count = e.Length >= 1 ? e.GetInt32(0) : 1;
            var hurt = e.Length >= 2 && e.GetString(1).ToLowerInvariant() == "hurt";

            if (count < 1)
            {
                count = 1;
            }

            var from = e.Mobile;

            MeteorEffect.Shower(
                from.Map,
                from.Location,
                count,
                4,
                1.5,
                5,
                hurt ? from : null,
                hurt ? 30 : 0,
                hurt ? 8 : 0
            );

            e.Mobile.SendMessage(0x35, $"Meteor: {count} meteoro(s){(hurt ? ", com dano" : "")}.");
        }
    }
}
