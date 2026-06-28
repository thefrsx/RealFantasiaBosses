using Server;
using Server.Commands;
using Server.Custom.Gumps;
using Server.Gumps;

namespace Server.Custom.Commands
{
    // Opens the click-driven FX test gump.
    public static class FxTestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("fxtest", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("fxtest")]
        [Description("Abre um painel (gump) para testar as mecanicas de FX com cliques, sem digitar comandos.")]
        public static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseGump<FxTestGump>();
            e.Mobile.SendGump(new FxTestGump());
        }
    }
}
