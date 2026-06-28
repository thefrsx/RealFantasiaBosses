using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Custom.Gumps
{
    // Skinned crafting menu using custom client gump art (the concept-art PNG imported into
    // gumpartLegacyMUL.uop at index BackgroundGump). v1 = render the background + a working close;
    // hotspots/dynamic text are layered on top iteratively once the art renders in-game.
    public class CraftSkinGump : Gump
    {
        // Reserved custom gump id for the imported concept art (1024x1536).
        // 40000-40155 were already in use in the client; 50000+ is free.
        public const int BackgroundGump = 50000;

        public CraftSkinGump() : base(60, 20)
        {
            Closable = true;
            Draggable = true;
            Resizable = false;

            AddPage(0);

            // The whole concept art as the backdrop. Shows blank until index 40000 is imported.
            AddImage(0, 0, BackgroundGump);

            // Temporary close button (standard UO art) until we wire the painted CLOSE hotspot.
            AddButton(950, 20, 0xFB1, 0xFB3, 0);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            // v1: only the close button. Hotspots come next once the art is confirmed in-game.
        }
    }
}
