using Server.Network;

namespace Server.Gumps;

public class iGO : Gump
{
    private static void TeleportGM(Mobile from, Map map, Point3D loc, int boltRange = 0)
    {
        foreach (var obj in from.Map.GetMobilesInRange<Mobile>(from.Location, boltRange))
        {
            obj.BoltEffect(0);
        }
        from.MoveToWorld(loc, map);
        from.CloseGump<IceGMTool>();
        from.SendGump(new IceGMTool(from));
    }

    public iGO() : base(150, 60)
    {
        Closable = true;
        Disposable = false;
        Draggable = true;
        Resizable = false;

        AddPage(1);
        AddBackground(63, 65, 228, 282, 9350);
        AddLabel(73, 75, 10, "Trammel Cities -----------------");
        AddButton(263, 80, 2361, 2361, 1, GumpButtonType.Page, 2);
        AddLabel(73, 95, 10, "Trammel Dungeons -------------");
        AddButton(263, 100, 2361, 2361, 2, GumpButtonType.Page, 3);
        AddLabel(73, 115, 10, "Trammel Points of Intrest -----");
        AddButton(263, 120, 2361, 2361, 3, GumpButtonType.Page, 4);
        AddLabel(73, 135, 81, "Felucca Cities ------------------");
        AddButton(263, 140, 2361, 2361, 4, GumpButtonType.Page, 5);
        AddLabel(73, 155, 81, "Felucca Dungeons ---------------");
        AddButton(263, 160, 2361, 2361, 5, GumpButtonType.Page, 6);
        AddLabel(73, 175, 81, "Felucca Points of Intrest ------");
        AddButton(263, 180, 2361, 2361, 6, GumpButtonType.Page, 7);
        AddLabel(73, 195, 1102, "Ilish Shrines & Cities ---------");
        AddButton(263, 200, 2361, 2361, 7, GumpButtonType.Page, 8);
        AddLabel(79, 215, 1102, "Ilish Dungeons + Forts --------");
        AddButton(263, 220, 2361, 2361, 8, GumpButtonType.Page, 9);
        AddLabel(79, 235, 1102, "Malas & Points of Intrest ----");
        AddButton(263, 240, 2361, 2361, 9, GumpButtonType.Page, 10);
        AddLabel(73, 255, 1154, "Tokuno & Points of Intrest ----");
        AddButton(263, 260, 2361, 2361, 10, GumpButtonType.Page, 11);
        AddLabel(73, 275, 10, "Trammel Shrines ---------------");
        AddButton(263, 280, 2361, 2361, 11, GumpButtonType.Page, 12);
        AddLabel(73, 295, 81, "Felucca Shrines -----------------");
        AddButton(263, 300, 2361, 2361, 12, GumpButtonType.Page, 13);
        AddLabel(73, 315, 315, "Internal ------------------------");
        AddButton(263, 320, 2361, 2361, 0, GumpButtonType.Page, 14);
        AddButton(141, 340, 1028, 1027, 169, GumpButtonType.Reply, 0);
        AddImage(13, 6, 10440);
        AddImage(257, 6, 10441);

        AddPage(2);
        AddBackground(157, 83, 297, 355, 9350);
        AddLabel(167, 93, 10, "Trammel Towns");
        AddLabel(172, 113, 10, "Britian ---------------------------------");
        AddLabel(167, 133, 10, "Cove -------------------------------------");
        AddLabel(167, 153, 10, "Delucia ----------------------------------");
        AddLabel(167, 173, 10, "Haven ----------------------------------");
        AddLabel(167, 193, 10, "Jhelom ----------------------------------");
        AddLabel(167, 213, 10, "Magincia ---------------------------------");
        AddLabel(175, 233, 10, "Minoc ----------------------------------");
        AddLabel(176, 253, 10, "Moonglow ------------------------------");
        AddLabel(167, 273, 10, "Nujel'm ---------------------------------");
        AddLabel(167, 293, 10, "Papua -----------------------------------");
        AddLabel(167, 313, 10, "Serpents Hold ---------------------------");
        AddLabel(167, 333, 10, "Skara Brae ------------------------------");
        AddLabel(167, 353, 10, "Trinsic ----------------------------------");
        AddLabel(169, 373, 10, "Vesper ----------------------------------");
        AddLabel(174, 393, 10, "Wind -----------------------------------");
        AddLabel(174, 413, 10, "Yew ------------------------------------");
        AddLabel(331, 86, 10, "Banks");
        AddLabel(395, 86, 10, "Center");
        AddButton(342, 117, 2361, 2361, 101, GumpButtonType.Reply, 0);
        AddButton(342, 137, 2361, 2361, 102, GumpButtonType.Reply, 0);
        AddButton(342, 157, 2361, 2361, 103, GumpButtonType.Reply, 0);
        AddButton(342, 177, 2361, 2361, 104, GumpButtonType.Reply, 0);
        AddButton(342, 197, 2361, 2361, 105, GumpButtonType.Reply, 0);
        AddButton(342, 217, 2361, 2361, 106, GumpButtonType.Reply, 0);
        AddButton(342, 237, 2361, 2361, 107, GumpButtonType.Reply, 0);
        AddButton(342, 257, 2361, 2361, 108, GumpButtonType.Reply, 0);
        AddButton(342, 277, 2361, 2361, 109, GumpButtonType.Reply, 0);
        AddButton(342, 297, 2361, 2361, 110, GumpButtonType.Reply, 0);
        AddButton(342, 317, 2361, 2361, 111, GumpButtonType.Reply, 0);
        AddButton(342, 337, 2361, 2361, 112, GumpButtonType.Reply, 0);
        AddButton(342, 357, 2361, 2361, 113, GumpButtonType.Reply, 0);
        AddButton(342, 377, 2361, 2361, 114, GumpButtonType.Reply, 0);
        AddButton(342, 397, 2361, 2361, 115, GumpButtonType.Reply, 0);
        AddButton(342, 417, 2361, 2361, 116, GumpButtonType.Reply, 0);
        AddButton(412, 117, 2361, 2361, 117, GumpButtonType.Reply, 0);
        AddButton(412, 137, 2361, 2361, 118, GumpButtonType.Reply, 0);
        AddButton(412, 157, 2361, 2361, 119, GumpButtonType.Reply, 0);
        AddButton(412, 177, 2361, 2361, 120, GumpButtonType.Reply, 0);
        AddButton(412, 197, 2361, 2361, 121, GumpButtonType.Reply, 0);
        AddButton(412, 217, 2361, 2361, 122, GumpButtonType.Reply, 0);
        AddButton(412, 237, 2361, 2361, 123, GumpButtonType.Reply, 0);
        AddButton(412, 257, 2361, 2361, 124, GumpButtonType.Reply, 0);
        AddButton(412, 277, 2361, 2361, 125, GumpButtonType.Reply, 0);
        AddButton(412, 297, 2361, 2361, 126, GumpButtonType.Reply, 0);
        AddButton(412, 317, 2361, 2361, 127, GumpButtonType.Reply, 0);
        AddButton(412, 337, 2361, 2361, 128, GumpButtonType.Reply, 0);
        AddButton(412, 357, 2361, 2361, 129, GumpButtonType.Reply, 0);
        AddButton(412, 377, 2361, 2361, 130, GumpButtonType.Reply, 0);
        AddButton(412, 397, 2361, 2361, 131, GumpButtonType.Reply, 0);
        AddButton(412, 417, 2361, 2361, 132, GumpButtonType.Reply, 0);
        AddImage(109, 25, 10440);
        AddImage(420, 25, 10441);

        AddPage(3);
        AddBackground(53, 64, 310, 355, 9350);
        AddLabel(63, 74, 10, "Trammel Dungeons");
        AddLabel(193, 74, 10, "Ent");
        AddLabel(68, 94, 10, "Covetus --------------------------------");
        AddLabel(60, 109, 10, "Deciet ----------------------------------");
        AddLabel(60, 124, 10, "Despise ---------------------------------");
        AddLabel(60, 139, 10, "Destard ---------------------------------");
        AddLabel(60, 154, 10, "Hythloth --------------------------------");
        AddLabel(60, 169, 10, "Shame ----------------------------------");
        AddLabel(60, 184, 10, "Wrong ----------------------------------");
        AddLabel(67, 199, 10, "Fire ------------------");
        AddLabel(71, 214, 10, "Ice -------------------");
        AddLabel(71, 229, 10, "Fire Temple ----------");
        AddLabel(66, 244, 10, "Terathan Keep ---------");
        AddButton(202, 99, 2361, 2361, 201, GumpButtonType.Reply, 0);
        AddButton(202, 115, 2361, 2361, 202, GumpButtonType.Reply, 0);
        AddButton(202, 130, 2361, 2361, 203, GumpButtonType.Reply, 0);
        AddButton(202, 145, 2361, 2361, 204, GumpButtonType.Reply, 0);
        AddButton(202, 160, 2361, 2361, 205, GumpButtonType.Reply, 0);
        AddButton(202, 175, 2361, 2361, 206, GumpButtonType.Reply, 0);
        AddButton(202, 189, 2361, 2361, 207, GumpButtonType.Reply, 0);
        AddButton(202, 205, 2361, 2361, 208, GumpButtonType.Reply, 0);
        AddButton(202, 220, 2361, 2361, 209, GumpButtonType.Reply, 0);
        AddButton(202, 235, 2361, 2361, 210, GumpButtonType.Reply, 0);
        AddButton(202, 250, 2361, 2361, 211, GumpButtonType.Reply, 0);
        AddImage(4, 4, 10440);
        AddImage(329, 4, 10441);
        AddLabel(243, 74, 10, "Lev 2");
        AddButton(253, 99, 2361, 2361, 221, GumpButtonType.Reply, 0);
        AddButton(253, 115, 2361, 2361, 222, GumpButtonType.Reply, 0);
        AddButton(253, 130, 2361, 2361, 223, GumpButtonType.Reply, 0);
        AddButton(253, 145, 2361, 2361, 224, GumpButtonType.Reply, 0);
        AddButton(253, 160, 2361, 2361, 225, GumpButtonType.Reply, 0);
        AddButton(253, 175, 2361, 2361, 226, GumpButtonType.Reply, 0);
        AddButton(253, 190, 2361, 2361, 227, GumpButtonType.Reply, 0);
        AddLabel(303, 74, 10, "Lev 3");
        AddButton(303, 99, 2361, 2361, 241, GumpButtonType.Reply, 0);
        AddButton(303, 115, 2361, 2361, 242, GumpButtonType.Reply, 0);
        AddButton(303, 130, 2361, 2361, 243, GumpButtonType.Reply, 0);
        AddButton(303, 145, 2361, 2361, 244, GumpButtonType.Reply, 0);
        AddButton(303, 160, 2361, 2361, 245, GumpButtonType.Reply, 0);
        AddButton(303, 175, 2361, 2361, 246, GumpButtonType.Reply, 0);
        AddButton(303, 190, 2361, 2361, 247, GumpButtonType.Reply, 0);
        AddLabel(60, 259, 10, "Blighted Grove ---------");
        AddButton(202, 265, 2361, 2361, 1212, GumpButtonType.Reply, 0);
        AddLabel(60, 274, 10, "Orc Caves -------------");
        AddButton(202, 279, 2361, 2361, 1213, GumpButtonType.Reply, 0);
        AddLabel(60, 289, 10, "Painted Caves ----------");
        AddButton(202, 294, 2361, 2361, 1214, GumpButtonType.Reply, 0);
        AddLabel(60, 304, 10, "Palace Of Paroxysmus -");
        AddButton(202, 309, 2361, 2361, 1215, GumpButtonType.Reply, 0);
        AddLabel(60, 319, 10, "Prism Of Light --------");
        AddButton(202, 324, 2361, 2361, 1216, GumpButtonType.Reply, 0);
        AddLabel(60, 334, 10, "Sanctuary --------------");
        AddButton(202, 339, 2361, 2361, 1217, GumpButtonType.Reply, 0);
        AddLabel(73, 380, 10, "Solen Hives --------------------------");
        AddButton(220, 384, 2361, 2361, 1218, GumpButtonType.Reply, 0);
        AddButton(180, 384, 2361, 2361, 1219, GumpButtonType.Reply, 0);
        AddButton(260, 384, 2361, 2361, 1220, GumpButtonType.Reply, 0);
        AddButton(300, 384, 2361, 2361, 1221, GumpButtonType.Reply, 0);
        AddLabel(180, 365, 10, "A");
        AddLabel(220, 365, 10, "B");
        AddLabel(260, 365, 10, "C");
        AddLabel(300, 365, 10, "D");

        AddPage(4);
        AddBackground(50, 64, 217, 355, 9350);
        AddLabel(78, 74, 10, "Trammel Points of Intrest");
        AddLabel(60, 104, 10, "Buccaneer's Den --------------");
        AddLabel(60, 124, 10, "Cove Orc Fort ---------------");
        AddLabel(60, 144, 10, "Fishermans Hut --------------");
        AddLabel(60, 164, 10, "Great Waterfall --------------");
        AddLabel(60, 184, 10, "Heart Clearing ---------------");
        AddLabel(65, 204, 10, "Hedge Maze -----------------");
        AddLabel(67, 224, 10, "Hidden Valley ----------------");
        AddLabel(60, 244, 10, "Island Temple -----------------");
        AddLabel(60, 264, 10, "Marble Island ----------------");
        AddLabel(60, 284, 10, "Ophidian Fort ----------------");
        AddLabel(60, 304, 10, "Statue & Bridge -------------");
        AddLabel(60, 324, 10, "Wind Park -------------------");
        AddLabel(60, 344, 10, "Yew Brigands ----------------");
        AddLabel(65, 364, 10, "Yew Orc Fort ---------------");
        AddLabel(65, 384, 10, "Yew Crypts -----------------");
        AddButton(240, 109, 2361, 2361, 301, GumpButtonType.Reply, 0);
        AddButton(240, 129, 2361, 2361, 302, GumpButtonType.Reply, 0);
        AddButton(240, 149, 2361, 2361, 303, GumpButtonType.Reply, 0);
        AddButton(240, 169, 2361, 2361, 304, GumpButtonType.Reply, 0);
        AddButton(240, 189, 2361, 2361, 305, GumpButtonType.Reply, 0);
        AddButton(240, 210, 2361, 2361, 306, GumpButtonType.Reply, 0);
        AddButton(240, 230, 2361, 2361, 307, GumpButtonType.Reply, 0);
        AddButton(240, 250, 2361, 2361, 308, GumpButtonType.Reply, 0);
        AddButton(240, 270, 2361, 2361, 309, GumpButtonType.Reply, 0);
        AddButton(240, 290, 2361, 2361, 310, GumpButtonType.Reply, 0);
        AddButton(240, 310, 2361, 2361, 311, GumpButtonType.Reply, 0);
        AddButton(240, 330, 2361, 2361, 312, GumpButtonType.Reply, 0);
        AddButton(240, 350, 2361, 2361, 313, GumpButtonType.Reply, 0);
        AddButton(240, 370, 2361, 2361, 314, GumpButtonType.Reply, 0);
        AddButton(240, 390, 2361, 2361, 315, GumpButtonType.Reply, 0);
        AddImage(2, 4, 10440);
        AddImage(233, 4, 10441);

        AddPage(5);
        AddBackground(157, 83, 297, 355, 9350);
        AddLabel(167, 93, 81, "Felucca Towns");
        AddLabel(172, 113, 81, "Britian ---------------------------------");
        AddLabel(167, 133, 81, "Cove -------------------------------------");
        AddLabel(167, 153, 81, "Delucia ----------------------------------");
        AddLabel(167, 173, 81, "Occlo ------------------------------------");
        AddLabel(167, 193, 81, "Jhelom ----------------------------------");
        AddLabel(167, 213, 81, "Magincia ---------------------------------");
        AddLabel(175, 233, 81, "Minoc ----------------------------------");
        AddLabel(176, 253, 81, "Moonglow ------------------------------");
        AddLabel(167, 273, 81, "Nujel'm ---------------------------------");
        AddLabel(167, 293, 81, "Papua -----------------------------------");
        AddLabel(167, 313, 81, "Serpents Hold ---------------------------");
        AddLabel(167, 333, 81, "Skara Brae ------------------------------");
        AddLabel(167, 353, 81, "Trinsic ----------------------------------");
        AddLabel(169, 373, 81, "Vesper ----------------------------------");
        AddLabel(174, 393, 81, "Wind -----------------------------------");
        AddLabel(174, 413, 81, "Yew ------------------------------------");
        AddLabel(331, 86, 81, "Banks");
        AddLabel(395, 86, 81, "Center");
        AddButton(342, 117, 2361, 2361, 401, GumpButtonType.Reply, 0);
        AddButton(342, 137, 2361, 2361, 402, GumpButtonType.Reply, 0);
        AddButton(342, 157, 2361, 2361, 403, GumpButtonType.Reply, 0);
        AddButton(342, 177, 2361, 2361, 404, GumpButtonType.Reply, 0);
        AddButton(342, 197, 2361, 2361, 405, GumpButtonType.Reply, 0);
        AddButton(342, 217, 2361, 2361, 406, GumpButtonType.Reply, 0);
        AddButton(342, 237, 2361, 2361, 407, GumpButtonType.Reply, 0);
        AddButton(342, 257, 2361, 2361, 408, GumpButtonType.Reply, 0);
        AddButton(342, 277, 2361, 2361, 409, GumpButtonType.Reply, 0);
        AddButton(342, 297, 2361, 2361, 410, GumpButtonType.Reply, 0);
        AddButton(342, 317, 2361, 2361, 411, GumpButtonType.Reply, 0);
        AddButton(342, 337, 2361, 2361, 412, GumpButtonType.Reply, 0);
        AddButton(342, 357, 2361, 2361, 413, GumpButtonType.Reply, 0);
        AddButton(342, 377, 2361, 2361, 414, GumpButtonType.Reply, 0);
        AddButton(342, 397, 2361, 2361, 415, GumpButtonType.Reply, 0);
        AddButton(342, 417, 2361, 2361, 416, GumpButtonType.Reply, 0);
        AddButton(412, 117, 2361, 2361, 417, GumpButtonType.Reply, 0);
        AddButton(412, 137, 2361, 2361, 418, GumpButtonType.Reply, 0);
        AddButton(412, 157, 2361, 2361, 419, GumpButtonType.Reply, 0);
        AddButton(412, 177, 2361, 2361, 420, GumpButtonType.Reply, 0);
        AddButton(412, 197, 2361, 2361, 421, GumpButtonType.Reply, 0);
        AddButton(412, 217, 2361, 2361, 422, GumpButtonType.Reply, 0);
        AddButton(412, 237, 2361, 2361, 423, GumpButtonType.Reply, 0);
        AddButton(412, 257, 2361, 2361, 424, GumpButtonType.Reply, 0);
        AddButton(412, 277, 2361, 2361, 425, GumpButtonType.Reply, 0);
        AddButton(412, 297, 2361, 2361, 426, GumpButtonType.Reply, 0);
        AddButton(412, 317, 2361, 2361, 427, GumpButtonType.Reply, 0);
        AddButton(412, 337, 2361, 2361, 428, GumpButtonType.Reply, 0);
        AddButton(412, 357, 2361, 2361, 429, GumpButtonType.Reply, 0);
        AddButton(412, 377, 2361, 2361, 430, GumpButtonType.Reply, 0);
        AddButton(412, 397, 2361, 2361, 431, GumpButtonType.Reply, 0);
        AddButton(412, 417, 2361, 2361, 432, GumpButtonType.Reply, 0);
        AddImage(109, 25, 10440);
        AddImage(420, 25, 10441);

        AddPage(6);
        AddBackground(53, 64, 310, 355, 9350);
        AddLabel(63, 74, 81, "Felucca Dungeons");
        AddLabel(193, 74, 81, "Ent");
        AddLabel(68, 94, 81, "Covetus --------------------------------");
        AddLabel(60, 109, 81, "Deciet ----------------------------------");
        AddLabel(60, 124, 81, "Despise ---------------------------------");
        AddLabel(60, 139, 81, "Destard ---------------------------------");
        AddLabel(60, 154, 81, "Hythloth --------------------------------");
        AddLabel(60, 169, 81, "Shame ----------------------------------");
        AddLabel(60, 184, 81, "Wrong ----------------------------------");
        AddLabel(67, 199, 81, "Fire ------------------");
        AddLabel(71, 214, 81, "Ice -------------------");
        AddLabel(71, 229, 81, "Fire Temple ----------");
        AddLabel(66, 244, 81, "Terathan Keep ---------");
        AddButton(202, 99, 2361, 2361, 501, GumpButtonType.Reply, 0);
        AddButton(202, 115, 2361, 2361, 502, GumpButtonType.Reply, 0);
        AddButton(202, 130, 2361, 2361, 503, GumpButtonType.Reply, 0);
        AddButton(202, 145, 2361, 2361, 504, GumpButtonType.Reply, 0);
        AddButton(202, 160, 2361, 2361, 505, GumpButtonType.Reply, 0);
        AddButton(202, 175, 2361, 2361, 506, GumpButtonType.Reply, 0);
        AddButton(202, 189, 2361, 2361, 507, GumpButtonType.Reply, 0);
        AddButton(202, 205, 2361, 2361, 508, GumpButtonType.Reply, 0);
        AddButton(202, 220, 2361, 2361, 509, GumpButtonType.Reply, 0);
        AddButton(202, 235, 2361, 2361, 510, GumpButtonType.Reply, 0);
        AddButton(202, 250, 2361, 2361, 511, GumpButtonType.Reply, 0);
        AddImage(4, 4, 10440);
        AddImage(329, 4, 10441);
        AddLabel(243, 74, 81, "Lev 2");
        AddButton(253, 99, 2361, 2361, 521, GumpButtonType.Reply, 0);
        AddButton(253, 115, 2361, 2361, 522, GumpButtonType.Reply, 0);
        AddButton(253, 130, 2361, 2361, 523, GumpButtonType.Reply, 0);
        AddButton(253, 145, 2361, 2361, 524, GumpButtonType.Reply, 0);
        AddButton(253, 160, 2361, 2361, 525, GumpButtonType.Reply, 0);
        AddButton(253, 175, 2361, 2361, 526, GumpButtonType.Reply, 0);
        AddButton(253, 190, 2361, 2361, 527, GumpButtonType.Reply, 0);
        AddLabel(303, 74, 81, "Lev 3");
        AddButton(303, 99, 2361, 2361, 541, GumpButtonType.Reply, 0);
        AddButton(303, 115, 2361, 2361, 542, GumpButtonType.Reply, 0);
        AddButton(303, 130, 2361, 2361, 543, GumpButtonType.Reply, 0);
        AddButton(303, 145, 2361, 2361, 544, GumpButtonType.Reply, 0);
        AddButton(303, 160, 2361, 2361, 545, GumpButtonType.Reply, 0);
        AddButton(303, 175, 2361, 2361, 546, GumpButtonType.Reply, 0);
        AddButton(303, 190, 2361, 2361, 547, GumpButtonType.Reply, 0);
        AddLabel(60, 259, 81, "Blighted Grove ---------");
        AddButton(202, 265, 2361, 2361, 2212, GumpButtonType.Reply, 0);
        AddLabel(60, 274, 81, "Orc Caves -------------");
        AddButton(202, 279, 2361, 2361, 2213, GumpButtonType.Reply, 0);
        AddLabel(60, 289, 81, "Painted Caves ----------");
        AddButton(202, 294, 2361, 2361, 2214, GumpButtonType.Reply, 0);
        AddLabel(60, 304, 81, "Palace Of Paroxysmus -");
        AddButton(202, 309, 2361, 2361, 2215, GumpButtonType.Reply, 0);
        AddLabel(60, 319, 81, "Prism Of Light --------");
        AddButton(202, 324, 2361, 2361, 2216, GumpButtonType.Reply, 0);
        AddLabel(60, 334, 81, "Sanctuary --------------");
        AddButton(202, 339, 2361, 2361, 2217, GumpButtonType.Reply, 0);
        AddLabel(73, 380, 81, "Solen Hives --------------------------");
        AddButton(220, 384, 2361, 2361, 2218, GumpButtonType.Reply, 0);
        AddButton(180, 384, 2361, 2361, 2219, GumpButtonType.Reply, 0);
        AddButton(260, 384, 2361, 2361, 2220, GumpButtonType.Reply, 0);
        AddButton(300, 384, 2361, 2361, 2221, GumpButtonType.Reply, 0);
        AddLabel(180, 365, 81, "A");
        AddLabel(220, 365, 81, "B");
        AddLabel(260, 365, 81, "C");
        AddLabel(300, 365, 81, "D");
        AddLabel(65, 349, 81, "Khaldun ---------------");
        AddButton(202, 354, 2361, 2361, 2222, GumpButtonType.Reply, 0);

        AddPage(7);
        AddBackground(50, 64, 217, 355, 9350);
        AddLabel(78, 74, 81, "Felucca Points of Intrest");
        AddLabel(60, 104, 81, "Buccaneer's Den --------------");
        AddLabel(60, 124, 81, "Cove Orc Fort ---------------");
        AddLabel(60, 144, 81, "Fishermans Hut --------------");
        AddLabel(60, 164, 81, "Great Waterfall --------------");
        AddLabel(60, 184, 81, "Heart Clearing ---------------");
        AddLabel(65, 204, 81, "Hedge Maze -----------------");
        AddLabel(67, 224, 81, "Hidden Valley ----------------");
        AddLabel(60, 244, 81, "Island Temple -----------------");
        AddLabel(60, 264, 81, "Marble Island ----------------");
        AddLabel(60, 284, 81, "Ophidian Fort ----------------");
        AddLabel(60, 304, 81, "Statue & Bridge -------------");
        AddLabel(60, 324, 81, "Wind Park -------------------");
        AddLabel(60, 344, 81, "Yew Brigands ----------------");
        AddLabel(65, 364, 81, "Yew Orc Fort ---------------");
        AddLabel(65, 384, 81, "Yew Crypts -----------------");
        AddButton(240, 109, 2361, 2361, 601, GumpButtonType.Reply, 0);
        AddButton(240, 129, 2361, 2361, 602, GumpButtonType.Reply, 0);
        AddButton(240, 149, 2361, 2361, 603, GumpButtonType.Reply, 0);
        AddButton(240, 169, 2361, 2361, 604, GumpButtonType.Reply, 0);
        AddButton(240, 189, 2361, 2361, 605, GumpButtonType.Reply, 0);
        AddButton(240, 210, 2361, 2361, 606, GumpButtonType.Reply, 0);
        AddButton(240, 230, 2361, 2361, 607, GumpButtonType.Reply, 0);
        AddButton(240, 250, 2361, 2361, 608, GumpButtonType.Reply, 0);
        AddButton(240, 270, 2361, 2361, 609, GumpButtonType.Reply, 0);
        AddButton(240, 290, 2361, 2361, 610, GumpButtonType.Reply, 0);
        AddButton(240, 310, 2361, 2361, 611, GumpButtonType.Reply, 0);
        AddButton(240, 330, 2361, 2361, 612, GumpButtonType.Reply, 0);
        AddButton(240, 350, 2361, 2361, 613, GumpButtonType.Reply, 0);
        AddButton(240, 370, 2361, 2361, 614, GumpButtonType.Reply, 0);
        AddButton(240, 390, 2361, 2361, 615, GumpButtonType.Reply, 0);
        AddImage(2, 4, 10440);
        AddImage(233, 4, 10441);

        AddPage(8);
        AddBackground(53, 64, 216, 295, 9350);
        AddLabel(91, 73, 1102, "Ilish Shrines & Cities");
        AddLabel(69, 94, 1102, "Chaos ------------------------");
        AddLabel(63, 114, 1102, "Compassion --------------------");
        AddLabel(63, 134, 1102, "Honesty -----------------------");
        AddLabel(63, 154, 1102, "Honor -------------------------");
        AddLabel(63, 174, 1102, "Humility -----------------------");
        AddLabel(63, 194, 1102, "Justice -----------------------");
        AddLabel(70, 214, 1102, "Sacrifice --------------------");
        AddLabel(69, 234, 1102, "Spirituality -------------------");
        AddLabel(63, 254, 1102, "Valor -------------------------");
        AddLabel(63, 274, 1102, "Lakeshire ---------------------");
        AddLabel(63, 294, 1102, "Gargoyle City -----------------");
        AddLabel(63, 314, 1102, "Mistas ------------------------");
        AddLabel(63, 334, 1102, "Montor -----------------------");
        AddButton(243, 98, 2361, 2361, 701, GumpButtonType.Reply, 0);
        AddButton(243, 118, 2361, 2361, 702, GumpButtonType.Reply, 0);
        AddButton(243, 138, 2361, 2361, 703, GumpButtonType.Reply, 0);
        AddButton(243, 158, 2361, 2361, 704, GumpButtonType.Reply, 0);
        AddButton(243, 178, 2361, 2361, 705, GumpButtonType.Reply, 0);
        AddButton(243, 198, 2361, 2361, 706, GumpButtonType.Reply, 0);
        AddButton(243, 218, 2361, 2361, 707, GumpButtonType.Reply, 0);
        AddButton(243, 238, 2361, 2361, 708, GumpButtonType.Reply, 0);
        AddButton(243, 258, 2361, 2361, 709, GumpButtonType.Reply, 0);
        AddButton(243, 278, 2361, 2361, 710, GumpButtonType.Reply, 0);
        AddButton(243, 298, 2361, 2361, 711, GumpButtonType.Reply, 0);
        AddButton(243, 318, 2361, 2361, 712, GumpButtonType.Reply, 0);
        AddButton(243, 338, 2361, 2361, 713, GumpButtonType.Reply, 0);
        AddImage(4, 3, 10440);
        AddImage(235, 3, 10441);

        AddPage(9);
        AddBackground(53, 63, 219, 349, 9350);
        AddLabel(91, 73, 1102, "Ilish Dungeons + Forts");
        AddLabel(63, 103, 1102, "Anchient Lair -----------------");
        AddLabel(63, 123, 1102, "Ankh -------------------------");
        AddLabel(63, 143, 1102, "Blackthorn's Castle -----------");
        AddLabel(63, 163, 1102, "Blood ------------------------");
        AddLabel(63, 183, 1102, "Cyclops ----------------------");
        AddLabel(67, 203, 1102, "Elemental Arena --------------");
        AddLabel(71, 223, 1102, "Exodus ----------------------");
        AddLabel(66, 243, 1102, "Lizardman Fort ---------------");
        AddLabel(63, 263, 1102, "Ratman Fort ------------------");
        AddLabel(63, 283, 1102, "Rock -------------------------");
        AddLabel(63, 303, 1102, "Savage Camp -----------------");
        AddLabel(63, 323, 1102, "Sorcerer ---------------------");
        AddLabel(63, 343, 1102, "Spectre ----------------------");
        AddLabel(68, 363, 1102, "Wisp ------------------------");
        AddLabel(68, 383, 1102, "Twisted Weald ---------------");
        AddButton(243, 108, 2361, 2361, 801, GumpButtonType.Reply, 0);
        AddButton(243, 128, 2361, 2361, 802, GumpButtonType.Reply, 0);
        AddButton(243, 148, 2361, 2361, 803, GumpButtonType.Reply, 0);
        AddButton(243, 168, 2361, 2361, 804, GumpButtonType.Reply, 0);
        AddButton(243, 188, 2361, 2361, 805, GumpButtonType.Reply, 0);
        AddButton(243, 208, 2361, 2361, 806, GumpButtonType.Reply, 0);
        AddButton(242, 228, 2361, 2361, 807, GumpButtonType.Reply, 0);
        AddButton(243, 248, 2361, 2361, 808, GumpButtonType.Reply, 0);
        AddButton(243, 268, 2361, 2361, 809, GumpButtonType.Reply, 0);
        AddButton(243, 288, 2361, 2361, 810, GumpButtonType.Reply, 0);
        AddButton(243, 308, 2361, 2361, 811, GumpButtonType.Reply, 0);
        AddButton(243, 328, 2361, 2361, 812, GumpButtonType.Reply, 0);
        AddButton(243, 348, 2361, 2361, 813, GumpButtonType.Reply, 0);
        AddButton(243, 368, 2361, 2361, 814, GumpButtonType.Reply, 0);
        AddButton(243, 388, 2361, 2361, 5, GumpButtonType.Reply, 0);
        AddImage(4, 3, 10440);
        AddImage(238, 3, 10441);

        AddPage(10);
        AddBackground(51, 63, 193, 364, 9350);
        AddLabel(67, 73, 1102, "Malas & Points of Intrest");
        AddLabel(61, 103, 1102, "Luna -----------------------");
        AddButton(223, 108, 2361, 2361, 901, GumpButtonType.Reply, 0);
        AddLabel(61, 123, 1102, "Umbra ---------------------");
        AddButton(223, 128, 2361, 2361, 902, GumpButtonType.Reply, 0);
        AddLabel(61, 143, 1102, "Doom ----------------------");
        AddButton(223, 148, 2361, 2361, 903, GumpButtonType.Reply, 0);
        AddLabel(61, 163, 1102, "Arena ----------------------");
        AddLabel(61, 183, 1102, "Orc Fort Desert -----------");
        AddLabel(65, 203, 1102, "Orc Fort Mountain --------");
        AddLabel(69, 223, 1102, "Corrupted Forest ---------");
        AddLabel(63, 243, 1102, "Crystal Fens --------------");
        AddLabel(61, 263, 1102, "Forgotten Pyramid ---------");
        AddLabel(61, 283, 1102, "Grimswind Ruins ------------");
        AddLabel(61, 303, 1102, "Hanse's Hostel -------------");
        AddLabel(61, 323, 1102, "Mining Mountians -----------");
        AddLabel(61, 343, 1102, "Northern Mountians --------");
        AddLabel(67, 363, 1102, "Bedlam --------------------");
        AddLabel(68, 383, 1102, "Labyrinth -----------------");
        AddLabel(62, 403, 1102, "The Citadel ----------------");
        AddButton(223, 168, 2361, 2361, 904, GumpButtonType.Reply, 0);
        AddButton(223, 188, 2361, 2361, 905, GumpButtonType.Reply, 0);
        AddButton(223, 208, 2361, 2361, 906, GumpButtonType.Reply, 0);
        AddButton(223, 228, 2361, 2361, 907, GumpButtonType.Reply, 0);
        AddButton(223, 248, 2361, 2361, 908, GumpButtonType.Reply, 0);
        AddButton(223, 268, 2361, 2361, 910, GumpButtonType.Reply, 0);
        AddButton(223, 288, 2361, 2361, 911, GumpButtonType.Reply, 0);
        AddButton(223, 308, 2361, 2361, 912, GumpButtonType.Reply, 0);
        AddButton(223, 328, 2361, 2361, 913, GumpButtonType.Reply, 0);
        AddButton(223, 348, 2361, 2361, 914, GumpButtonType.Reply, 0);
        AddButton(223, 368, 2361, 2361, 9, GumpButtonType.Reply, 0);
        AddButton(223, 388, 2361, 2361, 10, GumpButtonType.Reply, 0);
        AddButton(223, 408, 2361, 2361, 11, GumpButtonType.Reply, 0);
        AddImage(2, 3, 10440);
        AddImage(210, 3, 10441);

        AddPage(11);
        AddBackground(52, 64, 246, 326, 9350);
        AddLabel(80, 70, 1154, "Tokuno & Points of Intrest");
        AddLabel(62, 104, 1154, "Makoto - Zento City --------------");
        AddLabel(62, 124, 1154, "Makoto - The Waste --------------");
        AddLabel(62, 144, 1154, "Homarae - Bushido Dojo -----------");
        AddLabel(62, 164, 1154, "Homarae - Echo Fields -------------");
        AddLabel(62, 184, 1154, "Homarae - Crane Marsh -----------");
        AddLabel(66, 204, 1154, "Homarae - Yomotsu Mines --------");
        AddLabel(70, 224, 1154, "Homarae - Kitsune Woods --------");
        AddLabel(65, 244, 1154, "Homarae - Defiance Point ---------");
        AddLabel(62, 264, 1154, "Isamu - Winter Spur --------------");
        AddLabel(62, 284, 1154, "Isamu - Fan Dancer Dojo ----------");
        AddLabel(62, 304, 1154, "Isamu - Mount Sho ----------------");
        AddLabel(62, 324, 1154, "Isamu - Lotus Lake ----------------");
        AddLabel(62, 344, 1154, "Isamu - Storm Point --------------");
        AddLabel(67, 364, 1154, "Isamu - Sleeping Dragon Valley ----");
        AddButton(272, 108, 2361, 2361, 1001, GumpButtonType.Reply, 0);
        AddButton(272, 128, 2361, 2361, 1002, GumpButtonType.Reply, 0);
        AddButton(272, 148, 2361, 2361, 1003, GumpButtonType.Reply, 0);
        AddButton(272, 168, 2361, 2361, 1004, GumpButtonType.Reply, 0);
        AddButton(272, 188, 2361, 2361, 1005, GumpButtonType.Reply, 0);
        AddButton(272, 208, 2361, 2361, 1006, GumpButtonType.Reply, 0);
        AddButton(272, 228, 2361, 2361, 1007, GumpButtonType.Reply, 0);
        AddButton(272, 248, 2361, 2361, 1008, GumpButtonType.Reply, 0);
        AddButton(272, 268, 2361, 2361, 1009, GumpButtonType.Reply, 0);
        AddButton(272, 288, 2361, 2361, 1010, GumpButtonType.Reply, 0);
        AddButton(272, 308, 2361, 2361, 1011, GumpButtonType.Reply, 0);
        AddButton(272, 328, 2361, 2361, 1012, GumpButtonType.Reply, 0);
        AddButton(272, 348, 2361, 2361, 1013, GumpButtonType.Reply, 0);
        AddButton(272, 368, 2361, 2361, 1014, GumpButtonType.Reply, 0);
        AddImage(3, 4, 10440);
        AddImage(264, 4, 10441);

        AddPage(12);
        AddBackground(53, 63, 218, 287, 9350);
        AddLabel(107, 74, 10, "Trammel Shrines");
        AddLabel(63, 103, 10, "Chaos ------------------------");
        AddLabel(63, 123, 10, "Compassion -------------------");
        AddLabel(63, 143, 10, "Honesty ----------------------");
        AddLabel(63, 163, 10, "Honor ------------------------");
        AddLabel(63, 183, 10, "Humility ----------------------");
        AddLabel(67, 203, 10, "Justice ----------------------");
        AddLabel(72, 223, 10, "Sacrifice -------------------");
        AddLabel(67, 243, 10, "Spirituality ------------------");
        AddLabel(63, 263, 10, "Valor ------------------------");
        AddButton(243, 107, 2361, 2361, 1101, GumpButtonType.Reply, 0);
        AddButton(243, 127, 2361, 2361, 1102, GumpButtonType.Reply, 0);
        AddButton(243, 147, 2361, 2361, 1103, GumpButtonType.Reply, 0);
        AddButton(243, 167, 2361, 2361, 1104, GumpButtonType.Reply, 0);
        AddButton(243, 187, 2361, 2361, 1105, GumpButtonType.Reply, 0);
        AddButton(243, 207, 2361, 2361, 1106, GumpButtonType.Reply, 0);
        AddButton(243, 227, 2361, 2361, 1107, GumpButtonType.Reply, 0);
        AddButton(243, 247, 2361, 2361, 1108, GumpButtonType.Reply, 0);
        AddButton(243, 267, 2361, 2361, 1109, GumpButtonType.Reply, 0);
        AddImage(4, 4, 10440);
        AddImage(236, 4, 10441);

        AddPage(13);
        AddBackground(53, 63, 218, 287, 9350);
        AddLabel(107, 74, 81, "Felucca Shrines");
        AddLabel(63, 103, 81, "Chaos ------------------------");
        AddLabel(63, 123, 81, "Compassion -------------------");
        AddLabel(63, 143, 81, "Honesty ----------------------");
        AddLabel(63, 163, 81, "Honor ------------------------");
        AddLabel(63, 183, 81, "Humility ----------------------");
        AddLabel(67, 203, 81, "Justice ----------------------");
        AddLabel(72, 223, 81, "Sacrifice -------------------");
        AddLabel(67, 243, 81, "Spirituality ------------------");
        AddLabel(63, 263, 81, "Valor ------------------------");
        AddButton(243, 107, 2361, 2361, 1201, GumpButtonType.Reply, 0);
        AddButton(243, 127, 2361, 2361, 1202, GumpButtonType.Reply, 0);
        AddButton(243, 147, 2361, 2361, 1203, GumpButtonType.Reply, 0);
        AddButton(243, 167, 2361, 2361, 1204, GumpButtonType.Reply, 0);
        AddButton(243, 187, 2361, 2361, 1205, GumpButtonType.Reply, 0);
        AddButton(243, 207, 2361, 2361, 1206, GumpButtonType.Reply, 0);
        AddButton(243, 227, 2361, 2361, 1207, GumpButtonType.Reply, 0);
        AddButton(243, 247, 2361, 2361, 1208, GumpButtonType.Reply, 0);
        AddButton(243, 267, 2361, 2361, 1209, GumpButtonType.Reply, 0);
        AddImage(4, 4, 10440);
        AddImage(236, 4, 10441);

        AddPage(14);
        AddBackground(49, 65, 238, 305, 9350);
        AddLabel(147, 74, 325, "Internal");
        AddLabel(63, 103, 10, "Trammel Jail ---------------------");
        AddLabel(63, 123, 10, "Trammel Green Area ------------");
        AddLabel(63, 143, 81, "Felucca Jail ----------------------");
        AddLabel(63, 163, 81, "Felucca Green Area --------------");
        AddButton(263, 107, 2361, 2361, 1500, GumpButtonType.Reply, 0);
        AddButton(263, 127, 2361, 2361, 1501, GumpButtonType.Reply, 0);
        AddButton(263, 147, 2361, 2361, 1502, GumpButtonType.Reply, 0);
        AddButton(263, 167, 2361, 2361, 1503, GumpButtonType.Reply, 0);
        AddImage(0, 6, 10440);
        AddImage(253, 6, 10441);
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        switch (info.ButtonID)
        {
            case 0:
            {
                from.CloseGump<iGO>();
                from.SendGump(new iGO());
                break;
            }
            case 169:
            {
                from.CloseGump<IceGMTool>();
                from.SendGump(new IceGMTool(from));
                from.SendMessage(32, "You decide to stay put.");
                break;
            }
            // Malas extras (Bedlam / Labyrinth / The Citadel)
            case 5:  { TeleportGM(from, Map.Ilshenar, new Point3D(2186, 1264, 0)); break; }
            case 9:  { TeleportGM(from, Map.Malas,    new Point3D(120,  1684, 0)); break; }
            case 10: { TeleportGM(from, Map.Malas,    new Point3D(329,  1973, 2)); break; }
            case 11: { TeleportGM(from, Map.Malas,    new Point3D(102,  1876, 0)); break; }
            // Trammel Banks
            case 101: { TeleportGM(from, Map.Trammel, new Point3D(1437, 1703,  2)); break; }
            case 102: { TeleportGM(from, Map.Trammel, new Point3D(2236, 1200,  0)); break; }
            case 103: { TeleportGM(from, Map.Trammel, new Point3D(5273, 3995, 37)); break; }
            case 104: { TeleportGM(from, Map.Trammel, new Point3D(3631, 2611,  0)); break; }
            case 105: { TeleportGM(from, Map.Trammel, new Point3D(1331, 3782,  0)); break; }
            case 106: { TeleportGM(from, Map.Trammel, new Point3D(3732, 2169, 20)); break; }
            case 107: { TeleportGM(from, Map.Trammel, new Point3D(2494,  566,  0)); break; }
            case 108: { TeleportGM(from, Map.Trammel, new Point3D(4461, 1175,  0)); break; }
            case 109: { TeleportGM(from, Map.Trammel, new Point3D(3776, 1311,  0)); break; }
            case 110: { TeleportGM(from, Map.Trammel, new Point3D(5675, 3128, 15)); break; }
            case 111: { TeleportGM(from, Map.Trammel, new Point3D(2870, 3472, 35)); break; }
            case 112: { TeleportGM(from, Map.Trammel, new Point3D( 596, 2135,  0)); break; }
            case 113: { TeleportGM(from, Map.Trammel, new Point3D(1828, 2821,  0)); break; }
            case 114: { TeleportGM(from, Map.Trammel, new Point3D(2893,  685,  0)); break; }
            case 115: { TeleportGM(from, Map.Trammel, new Point3D(5348,   94, 15)); break; }
            case 116: { TeleportGM(from, Map.Trammel, new Point3D( 624,  823,  0), 10); break; }
            // Trammel Centers
            case 117: { TeleportGM(from, Map.Trammel, new Point3D(1475, 1645, 21)); break; }
            case 118: { TeleportGM(from, Map.Trammel, new Point3D(2267, 1211,  0)); break; }
            case 119: { TeleportGM(from, Map.Trammel, new Point3D(5246, 4055, 37)); break; }
            case 120: { TeleportGM(from, Map.Trammel, new Point3D(3635, 2605,  0)); break; }
            case 121: { TeleportGM(from, Map.Trammel, new Point3D(1414, 3829,  5)); break; }
            case 122: { TeleportGM(from, Map.Trammel, new Point3D(3701, 2196, 20)); break; }
            case 123: { TeleportGM(from, Map.Trammel, new Point3D(2467,  434, 15)); break; }
            case 124: { TeleportGM(from, Map.Trammel, new Point3D(4442, 1123,  5)); break; }
            case 125: { TeleportGM(from, Map.Trammel, new Point3D(3714, 1238,  0)); break; }
            case 126: { TeleportGM(from, Map.Trammel, new Point3D(5732, 3208,  0)); break; }
            case 127: { TeleportGM(from, Map.Trammel, new Point3D(2993, 3405, 15)); break; }
            case 128: { TeleportGM(from, Map.Trammel, new Point3D( 610, 2194,  0)); break; }
            case 129: { TeleportGM(from, Map.Trammel, new Point3D(1914, 2720, 20)); break; }
            case 130: { TeleportGM(from, Map.Trammel, new Point3D(2857,  866,  0)); break; }
            case 131: { TeleportGM(from, Map.Trammel, new Point3D(5222,  190,  5)); break; }
            case 132: { TeleportGM(from, Map.Trammel, new Point3D( 535,  993,  0)); break; }
            // Trammel Dungeons Lev 1
            case 201: { TeleportGM(from, Map.Trammel, new Point3D(2499,  922,   0)); break; }
            case 202: { TeleportGM(from, Map.Trammel, new Point3D(4111,  434,   5)); break; }
            case 203: { TeleportGM(from, Map.Trammel, new Point3D(1302, 1081,   0)); break; }
            case 204: { TeleportGM(from, Map.Trammel, new Point3D(1176, 2641,   3)); break; }
            case 205: { TeleportGM(from, Map.Trammel, new Point3D(4723, 3819,  41)); break; }
            case 206: { TeleportGM(from, Map.Trammel, new Point3D( 511, 1566,   0)); break; }
            case 207: { TeleportGM(from, Map.Trammel, new Point3D(2044,  239,  10)); break; }
            case 208: { TeleportGM(from, Map.Trammel, new Point3D(2921, 3409,  10)); break; }
            case 209: { TeleportGM(from, Map.Trammel, new Point3D(2001,   83,   5)); break; }
            case 210: { TeleportGM(from, Map.Trammel, new Point3D(4597, 3631,  30)); break; }
            case 211: { TeleportGM(from, Map.Trammel, new Point3D(5434, 3159, -60)); break; }
            // Trammel Dungeons Lev 2
            case 221: { TeleportGM(from, Map.Trammel, new Point3D(5613, 1998,  0)); break; }
            case 222: { TeleportGM(from, Map.Trammel, new Point3D(5309,  531,  0)); break; }
            case 223: { TeleportGM(from, Map.Trammel, new Point3D(5518,  673, 20)); break; }
            case 224: { TeleportGM(from, Map.Trammel, new Point3D(5144,  804,  0)); break; }
            case 225: { TeleportGM(from, Map.Trammel, new Point3D(5975,  171,  0)); break; }
            case 226: { TeleportGM(from, Map.Trammel, new Point3D(5517,   15,  0)); break; }
            case 227: { TeleportGM(from, Map.Trammel, new Point3D(5690,  568, 25)); break; }
            // Trammel Dungeons Lev 3
            case 241: { TeleportGM(from, Map.Trammel, new Point3D(2546,  857,   0)); break; }
            case 242: { TeleportGM(from, Map.Trammel, new Point3D(5139,  654,   0)); break; }
            case 243: { TeleportGM(from, Map.Trammel, new Point3D(5402,  869,  45)); break; }
            case 244: { TeleportGM(from, Map.Trammel, new Point3D(5139,  972,   0)); break; }
            case 245: { TeleportGM(from, Map.Trammel, new Point3D(6083,  149, -22)); break; }
            case 246: { TeleportGM(from, Map.Trammel, new Point3D(5516,  143,  20)); break; }
            case 247: { TeleportGM(from, Map.Trammel, new Point3D(5700,  662,   0)); break; }
            // Trammel Points of Interest
            case 301: { TeleportGM(from, Map.Trammel, new Point3D(2730, 2142,  0)); break; }
            case 302: { TeleportGM(from, Map.Trammel, new Point3D(2206, 1270,  0)); break; }
            case 303: { TeleportGM(from, Map.Trammel, new Point3D(2372, 3487,  5)); break; }
            case 304: { TeleportGM(from, Map.Trammel, new Point3D(1316,  550, 30)); break; }
            case 305: { TeleportGM(from, Map.Trammel, new Point3D( 782, 1457,  0)); break; }
            case 306: { TeleportGM(from, Map.Trammel, new Point3D(1150, 2236, 40)); break; }
            case 307: { TeleportGM(from, Map.Trammel, new Point3D(1687, 2986,  0)); break; }
            case 308: { TeleportGM(from, Map.Trammel, new Point3D(2494, 3597,  5)); break; }
            case 309: { TeleportGM(from, Map.Trammel, new Point3D(1904, 2100,  0)); break; }
            case 310: { TeleportGM(from, Map.Trammel, new Point3D(5758, 2692, 45)); break; }
            case 311: { TeleportGM(from, Map.Trammel, new Point3D(1012, 2677,  0)); break; }
            case 312: { TeleportGM(from, Map.Trammel, new Point3D(5212,   25, 15)); break; }
            case 313: { TeleportGM(from, Map.Trammel, new Point3D( 885, 1682,  0)); break; }
            case 314: { TeleportGM(from, Map.Trammel, new Point3D( 632, 1510,  0)); break; }
            case 315: { TeleportGM(from, Map.Trammel, new Point3D( 972,  772,  0)); break; }
            // Felucca Banks
            case 401: { TeleportGM(from, Map.Felucca, new Point3D(1437, 1703,  2)); break; }
            case 402: { TeleportGM(from, Map.Felucca, new Point3D(2236, 1200,  0)); break; }
            case 403: { TeleportGM(from, Map.Felucca, new Point3D(5273, 3995, 37)); break; }
            case 404: { TeleportGM(from, Map.Felucca, new Point3D(3686, 2525,  0)); break; } // Occlo
            case 405: { TeleportGM(from, Map.Felucca, new Point3D(1331, 3782,  0)); break; }
            case 406: { TeleportGM(from, Map.Felucca, new Point3D(3732, 2169, 20)); break; }
            case 407: { TeleportGM(from, Map.Felucca, new Point3D(2494,  566,  0)); break; }
            case 408: { TeleportGM(from, Map.Felucca, new Point3D(4461, 1175,  0)); break; }
            case 409: { TeleportGM(from, Map.Felucca, new Point3D(3776, 1311,  0)); break; }
            case 410: { TeleportGM(from, Map.Felucca, new Point3D(5675, 3128, 15)); break; }
            case 411: { TeleportGM(from, Map.Felucca, new Point3D(2870, 3472, 35)); break; }
            case 412: { TeleportGM(from, Map.Felucca, new Point3D( 596, 2135,  0)); break; }
            case 413: { TeleportGM(from, Map.Felucca, new Point3D(1828, 2821,  0)); break; }
            case 414: { TeleportGM(from, Map.Felucca, new Point3D(2893,  685,  0)); break; }
            case 415: { TeleportGM(from, Map.Felucca, new Point3D(5348,   94, 15)); break; }
            case 416: { TeleportGM(from, Map.Felucca, new Point3D( 624,  823,  0)); break; }
            // Felucca Centers
            case 417: { TeleportGM(from, Map.Felucca, new Point3D(1475, 1645, 21)); break; }
            case 418: { TeleportGM(from, Map.Felucca, new Point3D(2267, 1211,  0)); break; }
            case 419: { TeleportGM(from, Map.Felucca, new Point3D(5246, 4055, 37)); break; }
            case 420: { TeleportGM(from, Map.Felucca, new Point3D(3651, 2616,  0)); break; } // Occlo
            case 421: { TeleportGM(from, Map.Felucca, new Point3D(1414, 3829,  5)); break; }
            case 422: { TeleportGM(from, Map.Felucca, new Point3D(3701, 2196, 20)); break; }
            case 423: { TeleportGM(from, Map.Felucca, new Point3D(2467,  434, 15)); break; }
            case 424: { TeleportGM(from, Map.Felucca, new Point3D(4442, 1123,  5)); break; }
            case 425: { TeleportGM(from, Map.Felucca, new Point3D(3714, 1238,  0)); break; }
            case 426: { TeleportGM(from, Map.Felucca, new Point3D(5732, 3208,  0)); break; }
            case 427: { TeleportGM(from, Map.Felucca, new Point3D(2993, 3405, 15)); break; }
            case 428: { TeleportGM(from, Map.Felucca, new Point3D( 610, 2194,  0)); break; }
            case 429: { TeleportGM(from, Map.Felucca, new Point3D(1914, 2720, 20)); break; }
            case 430: { TeleportGM(from, Map.Felucca, new Point3D(2857,  866,  0)); break; }
            case 431: { TeleportGM(from, Map.Felucca, new Point3D(5222,  190,  5)); break; }
            case 432: { TeleportGM(from, Map.Felucca, new Point3D( 535,  993,  0)); break; }
            // Felucca Dungeons Lev 1
            case 501: { TeleportGM(from, Map.Felucca, new Point3D(2499,  922,   0)); break; }
            case 502: { TeleportGM(from, Map.Felucca, new Point3D(4111,  434,   5)); break; }
            case 503: { TeleportGM(from, Map.Felucca, new Point3D(1302, 1081,   0)); break; }
            case 504: { TeleportGM(from, Map.Felucca, new Point3D(1176, 2641,   3)); break; }
            case 505: { TeleportGM(from, Map.Felucca, new Point3D(4723, 3819,  41)); break; }
            case 506: { TeleportGM(from, Map.Felucca, new Point3D( 511, 1566,   0)); break; }
            case 507: { TeleportGM(from, Map.Felucca, new Point3D(2044,  239,  10)); break; }
            case 508: { TeleportGM(from, Map.Felucca, new Point3D(2921, 3409,  10)); break; }
            case 509: { TeleportGM(from, Map.Felucca, new Point3D(2001,   83,   5)); break; }
            case 510: { TeleportGM(from, Map.Felucca, new Point3D(4597, 3631,  30)); break; }
            case 511: { TeleportGM(from, Map.Felucca, new Point3D(5434, 3159, -60)); break; }
            // Felucca Dungeons Lev 2
            case 521: { TeleportGM(from, Map.Felucca, new Point3D(5613, 1998,  0)); break; }
            case 522: { TeleportGM(from, Map.Felucca, new Point3D(5309,  531,  0)); break; }
            case 523: { TeleportGM(from, Map.Felucca, new Point3D(5518,  673, 20)); break; }
            case 524: { TeleportGM(from, Map.Felucca, new Point3D(5144,  804,  0)); break; }
            case 525: { TeleportGM(from, Map.Felucca, new Point3D(5975,  171,  0)); break; }
            case 526: { TeleportGM(from, Map.Felucca, new Point3D(5517,   15,  0)); break; }
            case 527: { TeleportGM(from, Map.Felucca, new Point3D(5690,  568, 25)); break; }
            // Felucca Dungeons Lev 3
            case 541: { TeleportGM(from, Map.Felucca, new Point3D(2546,  857,   0)); break; }
            case 542: { TeleportGM(from, Map.Felucca, new Point3D(5139,  654,   0)); break; }
            case 543: { TeleportGM(from, Map.Felucca, new Point3D(5402,  869,  45)); break; }
            case 544: { TeleportGM(from, Map.Felucca, new Point3D(5139,  972,   0)); break; }
            case 545: { TeleportGM(from, Map.Felucca, new Point3D(6083,  149, -22)); break; }
            case 546: { TeleportGM(from, Map.Felucca, new Point3D(5516,  143,  20)); break; }
            case 547: { TeleportGM(from, Map.Felucca, new Point3D(5700,  662,   0)); break; }
            // Felucca Points of Interest
            case 601: { TeleportGM(from, Map.Felucca, new Point3D(2730, 2142,  0)); break; }
            case 602: { TeleportGM(from, Map.Felucca, new Point3D(2206, 1270,  0)); break; }
            case 603: { TeleportGM(from, Map.Felucca, new Point3D(2372, 3487,  5)); break; }
            case 604: { TeleportGM(from, Map.Felucca, new Point3D(1316,  550, 30)); break; }
            case 605: { TeleportGM(from, Map.Felucca, new Point3D( 782, 1457,  0)); break; }
            case 606: { TeleportGM(from, Map.Felucca, new Point3D(1150, 2236, 40)); break; }
            case 607: { TeleportGM(from, Map.Felucca, new Point3D(1687, 2986,  0)); break; }
            case 608: { TeleportGM(from, Map.Felucca, new Point3D(2494, 3597,  5)); break; }
            case 609: { TeleportGM(from, Map.Felucca, new Point3D(1918, 2091,  0)); break; }
            case 610: { TeleportGM(from, Map.Felucca, new Point3D(5758, 2692, 45)); break; }
            case 611: { TeleportGM(from, Map.Felucca, new Point3D(1012, 2677,  0)); break; }
            case 612: { TeleportGM(from, Map.Felucca, new Point3D(5212,   25, 15)); break; }
            case 613: { TeleportGM(from, Map.Felucca, new Point3D( 885, 1682,  0)); break; }
            case 614: { TeleportGM(from, Map.Felucca, new Point3D( 632, 1510,  0)); break; }
            case 615: { TeleportGM(from, Map.Felucca, new Point3D( 972,  772,  0)); break; }
            // Ilshenar Shrines & Cities
            case 701: { TeleportGM(from, Map.Ilshenar, new Point3D(1747,  236,  58)); break; }
            case 702: { TeleportGM(from, Map.Ilshenar, new Point3D(1217,  469, -13)); break; }
            case 703: { TeleportGM(from, Map.Ilshenar, new Point3D( 720, 1356, -59)); break; }
            case 704: { TeleportGM(from, Map.Ilshenar, new Point3D( 748,  731, -29)); break; }
            case 705: { TeleportGM(from, Map.Ilshenar, new Point3D( 287, 1019,   0)); break; }
            case 706: { TeleportGM(from, Map.Ilshenar, new Point3D( 987, 1002, -36)); break; }
            case 707: { TeleportGM(from, Map.Ilshenar, new Point3D(1172, 1288, -30)); break; }
            case 708: { TeleportGM(from, Map.Ilshenar, new Point3D(1528, 1341,  -3)); break; }
            case 709: { TeleportGM(from, Map.Ilshenar, new Point3D( 529,  212, -42)); break; }
            case 710: { TeleportGM(from, Map.Ilshenar, new Point3D(1203, 1124, -25)); break; }
            case 711: { TeleportGM(from, Map.Ilshenar, new Point3D( 836,  641, -20)); break; }
            case 712: { TeleportGM(from, Map.Ilshenar, new Point3D( 820, 1155, -30)); break; }
            case 713: { TeleportGM(from, Map.Ilshenar, new Point3D(1643,  310,  48)); break; }
            // Ilshenar Dungeons & Forts
            case 801: { TeleportGM(from, Map.Ilshenar, new Point3D( 940,  503,  -30)); break; }
            case 802: { TeleportGM(from, Map.Ilshenar, new Point3D( 576, 1145, -100)); break; }
            case 803: { TeleportGM(from, Map.Ilshenar, new Point3D(1118,  652,  -80)); break; }
            case 804: { TeleportGM(from, Map.Ilshenar, new Point3D(1747, 1225,   -1)); break; }
            case 805: { TeleportGM(from, Map.Ilshenar, new Point3D( 884, 1303,  -71)); break; }
            case 806: { TeleportGM(from, Map.Ilshenar, new Point3D( 349, 1432,   15)); break; }
            case 807: { TeleportGM(from, Map.Ilshenar, new Point3D( 852,  777,  -80)); break; }
            case 808: { TeleportGM(from, Map.Ilshenar, new Point3D( 322, 1363,  -26)); break; }
            case 809: { TeleportGM(from, Map.Ilshenar, new Point3D( 643,  860,  -59)); break; }
            case 810: { TeleportGM(from, Map.Ilshenar, new Point3D(1788,  573,   71)); break; }
            case 811: { TeleportGM(from, Map.Ilshenar, new Point3D(1188,  692,  -80)); break; }
            case 812: { TeleportGM(from, Map.Ilshenar, new Point3D( 547,  464,  -58)); break; }
            case 813: { TeleportGM(from, Map.Ilshenar, new Point3D(1363, 1041,  -13)); break; }
            case 814: { TeleportGM(from, Map.Ilshenar, new Point3D( 651, 1305,  -57)); break; }
            // Malas
            case 901: { TeleportGM(from, Map.Malas, new Point3D( 982,  519,  -50)); break; }
            case 902: { TeleportGM(from, Map.Malas, new Point3D(2029, 1343,  -90)); break; }
            case 903: { TeleportGM(from, Map.Malas, new Point3D(2368, 1268,  -85)); break; }
            case 904: { TeleportGM(from, Map.Malas, new Point3D(2368, 1160,  -90)); break; }
            case 905: { TeleportGM(from, Map.Malas, new Point3D(1597, 1843, -102)); break; }
            case 906: { TeleportGM(from, Map.Malas, new Point3D(1343, 1272,  -90)); break; }
            case 907: { TeleportGM(from, Map.Malas, new Point3D(2161, 1164,  -84)); break; }
            case 908: { TeleportGM(from, Map.Malas, new Point3D(1355,  601,  -89)); break; }
            case 910: { TeleportGM(from, Map.Malas, new Point3D(1861, 1809, -107)); break; }
            case 911: { TeleportGM(from, Map.Malas, new Point3D(2192,  351,  -90)); break; }
            case 912: { TeleportGM(from, Map.Malas, new Point3D(1072, 1435,  -90)); break; }
            case 913: { TeleportGM(from, Map.Malas, new Point3D(1257, 1416,  -95)); break; }
            case 914: { TeleportGM(from, Map.Malas, new Point3D(1530,  436,  -86)); break; }
            // Tokuno
            case 1001: { TeleportGM(from, Map.Tokuno, new Point3D( 738, 1242, 25)); break; }
            case 1002: { TeleportGM(from, Map.Tokuno, new Point3D( 729, 1034, 30)); break; }
            case 1003: { TeleportGM(from, Map.Tokuno, new Point3D( 320,  461, 32)); break; }
            case 1004: { TeleportGM(from, Map.Tokuno, new Point3D( 204,  650, 33)); break; }
            case 1005: { TeleportGM(from, Map.Tokuno, new Point3D( 204,  986, 17)); break; }
            case 1006: { TeleportGM(from, Map.Tokuno, new Point3D( 254,  787, 64)); break; }
            case 1007: { TeleportGM(from, Map.Tokuno, new Point3D( 502,  503, 32)); break; }
            case 1008: { TeleportGM(from, Map.Tokuno, new Point3D( 278, 1192, 20)); break; }
            case 1009: { TeleportGM(from, Map.Tokuno, new Point3D( 925,  155, 48)); break; }
            case 1010: { TeleportGM(from, Map.Tokuno, new Point3D( 979,  244, 21)); break; }
            case 1011: { TeleportGM(from, Map.Tokuno, new Point3D(1099,  763, 30)); break; }
            case 1012: { TeleportGM(from, Map.Tokuno, new Point3D(1068,  845, 41)); break; }
            case 1013: { TeleportGM(from, Map.Tokuno, new Point3D(1191, 1114, 17)); break; }
            case 1014: { TeleportGM(from, Map.Tokuno, new Point3D(1013,  535, 29)); break; }
            // Trammel Shrines
            case 1101: { TeleportGM(from, Map.Trammel, new Point3D(1458,  844,  5)); break; }
            case 1102: { TeleportGM(from, Map.Trammel, new Point3D(1858,  875, -1)); break; }
            case 1103: { TeleportGM(from, Map.Trammel, new Point3D(4210,  563, 42)); break; }
            case 1104: { TeleportGM(from, Map.Trammel, new Point3D(1727, 3528,  3)); break; }
            case 1105: { TeleportGM(from, Map.Trammel, new Point3D(4274, 3697,  0)); break; }
            case 1106: { TeleportGM(from, Map.Trammel, new Point3D(1301,  634, 16)); break; }
            case 1107: { TeleportGM(from, Map.Trammel, new Point3D(3355,  290,  4)); break; }
            case 1108: { TeleportGM(from, Map.Trammel, new Point3D(1595, 2490, 20)); break; }
            case 1109: { TeleportGM(from, Map.Trammel, new Point3D(2492, 3931,  5)); break; }
            // Felucca Shrines
            case 1201: { TeleportGM(from, Map.Felucca, new Point3D(1458,  844,  5)); break; }
            case 1202: { TeleportGM(from, Map.Felucca, new Point3D(1858,  875, -1)); break; }
            case 1203: { TeleportGM(from, Map.Felucca, new Point3D(4210,  563, 42)); break; }
            case 1204: { TeleportGM(from, Map.Felucca, new Point3D(1727, 3528,  3)); break; }
            case 1205: { TeleportGM(from, Map.Felucca, new Point3D(4274, 3697,  0)); break; }
            case 1206: { TeleportGM(from, Map.Felucca, new Point3D(1301,  634, 16)); break; }
            case 1207: { TeleportGM(from, Map.Felucca, new Point3D(3355,  290,  4)); break; }
            case 1208: { TeleportGM(from, Map.Felucca, new Point3D(1595, 2490, 20)); break; }
            case 1209: { TeleportGM(from, Map.Felucca, new Point3D(2492, 3931,  5)); break; }
            // Extra Trammel
            case 1212: { TeleportGM(from, Map.Trammel, new Point3D( 571, 1641, -2)); break; }
            case 1213: { TeleportGM(from, Map.Trammel, new Point3D(1016, 1433,  0)); break; }
            case 1214: { TeleportGM(from, Map.Trammel, new Point3D(1717, 2994,  0)); break; }
            case 1215: { TeleportGM(from, Map.Trammel, new Point3D(6221,  333, 60)); break; }
            case 1216: { TeleportGM(from, Map.Trammel, new Point3D(3787, 1090, 20)); break; }
            case 1217: { TeleportGM(from, Map.Trammel, new Point3D( 761, 1644,  0)); break; }
            case 1218: { TeleportGM(from, Map.Trammel, new Point3D(1728,  814,  0)); break; }
            case 1219: { TeleportGM(from, Map.Trammel, new Point3D(2613,  762,  0)); break; }
            case 1220: { TeleportGM(from, Map.Trammel, new Point3D(1693, 2786,  0)); break; }
            case 1221: { TeleportGM(from, Map.Trammel, new Point3D( 731, 1448,  0)); break; }
            // Extra Felucca
            case 2212: { TeleportGM(from, Map.Felucca, new Point3D( 571, 1641, -2)); break; }
            case 2213: { TeleportGM(from, Map.Felucca, new Point3D(1016, 1433,  0)); break; }
            case 2214: { TeleportGM(from, Map.Felucca, new Point3D(1717, 2994,  0)); break; }
            case 2215: { TeleportGM(from, Map.Felucca, new Point3D(6221,  333, 60)); break; }
            case 2216: { TeleportGM(from, Map.Felucca, new Point3D(3787, 1090, 20)); break; }
            case 2217: { TeleportGM(from, Map.Felucca, new Point3D( 761, 1644,  0)); break; }
            case 2218: { TeleportGM(from, Map.Felucca, new Point3D(1728,  814,  0)); break; }
            case 2219: { TeleportGM(from, Map.Felucca, new Point3D(2613,  762,  0)); break; }
            case 2220: { TeleportGM(from, Map.Felucca, new Point3D(1693, 2786,  0)); break; }
            case 2221: { TeleportGM(from, Map.Felucca, new Point3D( 731, 1448,  0)); break; }
            case 2222: { TeleportGM(from, Map.Felucca, new Point3D(6012, 3779, 19)); break; }
            // Internal
            case 1500: { TeleportGM(from, Map.Trammel, new Point3D(5300, 1181, 0)); break; }
            case 1501: { TeleportGM(from, Map.Trammel, new Point3D(5445, 1153, 0)); break; }
            case 1502: { TeleportGM(from, Map.Felucca, new Point3D(5281, 1181, 0)); break; }
            case 1503: { TeleportGM(from, Map.Felucca, new Point3D(5445, 1153, 0)); break; }
        }
    }
}
