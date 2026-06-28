using Server;
using Server.Commands;
using Server.Custom.Gumps;
using Server.Gumps;

namespace Server.Custom.Commands
{
    // Opens the skinned crafting gump (concept-art skin). Needs gump art index 40000 imported in the client.
    public static class CraftSkinCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("craftskin", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("craftskin")]
        [Description("Abre o gump de crafting com a skin da arte conceitual (requer arte importada no cliente, indice 50000).")]
        public static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseGump<CraftSkinGump>();
            e.Mobile.SendGump(new CraftSkinGump());
        }
    }
}
