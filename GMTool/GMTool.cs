using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Logging;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Gumps;

public class IceGMTool : Gump
{
    private static readonly ILogger _log = LogFactory.GetLogger(typeof(IceGMTool));

    public static void Configure()
    {
        CommandSystem.Register("GMTool", AccessLevel.Counselor, GMTool_OnCommand);
    }

    private static void GMTool_OnCommand(CommandEventArgs e)
    {
        var from = e.Mobile;
        from.CloseGump<IceGMTool>();
        from.SendGump(new IceGMTool(from));
        _log.Information("[{AccessLevel}] {Name} is using Ice's GM Tool.", from.AccessLevel, from.Name);
    }

    public IceGMTool(Mobile from) : base(0, 0)
    {
        Closable = false;
        Disposable = false;
        Draggable = true;
        Resizable = false;

        AddPage(6);
        AddBackground(154, 84, 89, 21, 9200);
        AddLabel(158, 85, 267, "Ice's GM Tool");
        AddButton(242, 86, 9766, 9767, 0, GumpButtonType.Page, 1);
        AddButton(191, 67, 22153, 22155, 0, GumpButtonType.Page, 2);

        AddPage(1);
        AddImage(98, 71, 5010);
        AddBackground(154, 84, 89, 21, 9200);
        AddLabel(158, 85, 267, "Ice's GM Tool");
        AddButton(242, 86, 9762, 9763, 0, GumpButtonType.Page, 6);
        AddButton(191, 160, 22150, 22151, 0, GumpButtonType.Reply, 0);

        if (from.AccessLevel >= AccessLevel.Administrator)
        {
            AddButton(157, 61, 2093, 2093, 31, GumpButtonType.Reply, 0);
            AddButton(186, 61, 2093, 2093, 29, GumpButtonType.Reply, 29);
            AddButton(218, 62, 2093, 2093, 0, GumpButtonType.Page, 7);
            AddLabel(189, 62, 1775, "Ad");
            AddLabel(222, 62, 1775, "Re");
            AddLabel(161, 62, 1775, "Pr");
        }

        if (from.AccessLevel >= AccessLevel.GameMaster)
        {
            AddButton(63, 213, 30083, 30089, 0, GumpButtonType.Page, 4);
            AddButton(219, 247, 2260, 2282, 14, GumpButtonType.Reply, 0);
            AddButton(176, 247, 2262, 2282, 12, GumpButtonType.Reply, 0);
            AddButton(137, 232, 2460, 2461, 15, GumpButtonType.Reply, 0);
            AddButton(287, 252, 1154, 1154, 32, GumpButtonType.Reply, 0);
            AddBackground(285, 172, 32, 108, 9200);
            AddButton(288, 223, 22253, 22259, 35, GumpButtonType.Reply, 0);
            AddButton(302, 223, 22254, 22260, 36, GumpButtonType.Reply, 0);
            AddItem(272, 249, 4011);
            AddButton(166, 206, 5537, 5539, 33, GumpButtonType.Reply, 0);
            AddButton(215, 206, 5540, 5542, 34, GumpButtonType.Reply, 0);
        }

        if (from.AccessLevel >= AccessLevel.Counselor)
        {
            AddImage(174, 105, 4500);
            AddImage(200, 116, 4501);
            AddImage(211, 142, 4502);
            AddImage(200, 169, 4503);
            AddImage(174, 179, 4504);
            AddImage(146, 169, 4505);
            AddImage(135, 142, 4506);
            AddImage(147, 115, 4507);
            AddButton(191, 120, 11410, 11402, 8, GumpButtonType.Reply, 0);
            AddButton(221, 129, 11410, 11402, 1, GumpButtonType.Reply, 0);
            AddButton(232, 159, 11410, 11402, 2, GumpButtonType.Reply, 0);
            AddButton(223, 188, 11410, 11402, 7, GumpButtonType.Reply, 0);
            AddButton(191, 200, 11410, 11402, 4, GumpButtonType.Reply, 0);
            AddButton(162, 188, 11410, 11402, 5, GumpButtonType.Reply, 0);
            AddButton(151, 159, 11410, 11402, 6, GumpButtonType.Reply, 0);
            AddButton(163, 131, 11410, 11402, 3, GumpButtonType.Reply, 0);
            AddButton(288, 101, 22153, 22155, 0, GumpButtonType.Page, 2);
            AddButton(285, 127, 22414, 22415, 9, GumpButtonType.Reply, 0);
            AddButton(284, 172, 22411, 22412, 10, GumpButtonType.Reply, 0);
            AddButton(190, 232, 2465, 2464, 11, GumpButtonType.Reply, 0);
            AddButton(133, 247, 2283, 2282, 13, GumpButtonType.Reply, 0);
            AddButton(87, 100, 30046, 30073, 16, GumpButtonType.Reply, 0);
            AddButton(87, 128, 30012, 30073, 17, GumpButtonType.Reply, 0);
            AddButton(87, 156, 30041, 30073, 18, GumpButtonType.Reply, 0);
            AddButton(64, 80, 30083, 30089, 19, GumpButtonType.Reply, 0);
            AddButton(87, 184, 30014, 30073, 20, GumpButtonType.Reply, 0);
            AddButton(176, 290, 2261, 2282, 21, GumpButtonType.Reply, 0);
            AddButton(219, 290, 2274, 2282, 25, GumpButtonType.Reply, 0);
            AddButton(133, 290, 2280, 2282, 26, GumpButtonType.Reply, 0);
            AddImage(64, 80, 30079);
            AddImage(65, 177, 30080);
        }

        AddPage(2);
        AddBackground(105, 3, 271, 430, 2520);
        AddImage(134, 176, 4500);
        AddImage(146, 365, 22150);
        AddImage(133, 343, 2465);
        AddImage(146, 288, 22414);
        AddImage(144, 234, 22411);
        AddImage(151, 189, 11410);
        AddImage(138, 126, 2262);
        AddImage(138, 83, 2283);
        AddBackground(209, 13, 88, 17, 9200);
        AddLabel(214, 11, 0, "GM Tool Info");
        AddLabel(201, 129, 0, "sets movable");
        AddLabel(196, 174, 0, "Moves the object");
        AddLabel(196, 190, 0, "one tile in pointed");
        AddLabel(195, 206, 0, "direction.");
        AddLabel(197, 248, 0, "Decreases the Z");
        AddLabel(201, 296, 0, "Increases the Z");
        AddLabel(213, 339, 0, "Deletes Objects");
        AddLabel(192, 363, 0, "Closes GM Tool");
        AddImage(331, 3, 2522);
        AddButton(186, 404, 2468, 2467, 0, GumpButtonType.Page, 1);
        AddImage(138, 40, 2260);
        AddLabel(200, 144, 0, " true/false");
        AddLabel(198, 84, 0, "sets Visible");
        AddLabel(194, 100, 0, " true/false");
        AddLabel(197, 40, 0, "Moves object to");
        AddLabel(197, 56, 0, "Target Location");
        AddButton(268, 404, 2471, 2470, 0, GumpButtonType.Page, 3);

        AddPage(3);
        AddBackground(105, 3, 271, 430, 2520);
        AddImage(146, 49, 30046);
        AddImage(146, 88, 30012);
        AddImage(146, 128, 30041);
        AddImage(146, 171, 30014);
        AddImage(140, 214, 30079);
        AddImage(142, 293, 30080);
        AddImage(146, 363, 2460);
        AddBackground(209, 13, 88, 17, 9200);
        AddLabel(214, 11, 0, "GM Tool Info");
        AddImage(331, 3, 2522);
        AddButton(186, 404, 2468, 2467, 0, GumpButtonType.Page, 2);
        AddLabel(192, 49, 0, "Nightsight");
        AddLabel(192, 88, 0, "Enables Speedboost");
        AddLabel(192, 128, 0, "Relays a few Infos");
        AddLabel(192, 171, 0, "Opens Go Gump");
        AddLabel(211, 236, 0, "-Blue Button-");
        AddLabel(195, 258, 0, "Disables SpeedBoost");
        AddLabel(215, 293, 0, "-Blue Button-");
        AddLabel(213, 311, 0, "Opens [OB Gump");
        AddLabel(216, 355, 0, "Opens Add Menu");
        AddButton(268, 404, 2471, 2470, 0, GumpButtonType.Page, 5);

        AddPage(4);
        AddBackground(62, 48, 229, 202, 9270);
        AddBackground(143, 20, 70, 49, 9270);
        AddLabel(156, 34, 37, "<URL>");
        AddLabel(104, 173, 52, "Press Ctrl+V to paste");
        AddLabel(112, 192, 52, "From your clipboard");
        AddBackground(73, 59, 208, 105, 9350);
        AddTextEntry(80, 62, 194, 99, 32, 1, "");
        AddButton(212, 215, 242, 241, 0, GumpButtonType.Page, 1);
        AddButton(77, 215, 247, 248, 24, GumpButtonType.Reply, 0);

        AddPage(5);
        AddBackground(105, 3, 271, 430, 2520);
        AddButton(186, 404, 2468, 2467, 0, GumpButtonType.Page, 3);
        AddButton(268, 404, 2471, 2470, 0, GumpButtonType.Page, 8);
        AddImage(331, 3, 2522);
        AddBackground(209, 13, 88, 17, 9200);
        AddLabel(214, 11, 0, "GM Tool Info");
        AddImage(138, 42, 2274);
        AddLabel(199, 50, 0, "[Who Command");
        AddImage(138, 85, 2280);
        AddLabel(199, 95, 0, "[Pages Command");
        AddImage(138, 128, 2261);
        AddLabel(199, 140, 0, "[m Tele");
        AddImage(148, 175, 2093);
        AddLabel(151, 176, 1775, "Ad");
        AddLabel(196, 175, 0, "Admin Menu");
        AddImage(148, 201, 2093);
        AddLabel(151, 202, 1775, "Re");
        AddLabel(197, 203, 0, "[Restart");
        AddImage(148, 227, 2093);
        AddLabel(152, 228, 1775, "Pr");
        AddLabel(197, 230, 0, "[Props");
        AddBackground(139, 262, 38, 33, 9200);
        AddItem(130, 264, 4011);
        AddLabel(198, 264, 0, "Hue # input");
        AddImage(157, 304, 5537);
        AddLabel(198, 303, 0, "[Kill");
        AddImage(157, 329, 5540);
        AddLabel(198, 331, 0, "[Res");
        AddImage(158, 364, 22253);
        AddLabel(198, 363, 0, "Random Skin Hue");

        AddPage(7);
        AddBackground(287, 79, 35, 100, 9200);
        AddBackground(192, 79, 35, 100, 9200);
        AddBackground(184, 78, 146, 49, 9270);
        AddButton(194, 150, 4014, 4014, 30, GumpButtonType.Reply, 0);
        AddButton(290, 150, 4017, 4017, 0, GumpButtonType.Page, 1);
        AddAlphaRegion(194, 88, 126, 28);
        AddLabel(204, 91, 137, " Restart Server?");

        AddPage(8);
        AddBackground(105, 3, 271, 430, 2520);
        AddButton(186, 404, 2468, 2467, 0, GumpButtonType.Page, 5);
        AddImage(331, 3, 2522);
        AddBackground(209, 13, 88, 17, 9200);
        AddImage(168, 50, 22254);
        AddLabel(199, 50, 0, "Random Neutral Hue");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        var prefix = CommandSystem.Prefix;
        var website = (info.GetTextEntry(1) ?? "").Trim();

        switch (info.ButtonID)
        {
            case 1:
                from.Target = new NT();
                from.SendMessage(2125, "What do you wish to move North? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 2:
                from.Target = new NWT();
                from.SendMessage(2125, "What do you wish to move North East? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 3:
                from.Target = new WT();
                from.SendMessage(2125, "What do you wish to move West? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 4:
                from.Target = new SWT();
                from.SendMessage(2125, "What do you wish to move South East? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 5:
                from.Target = new ST();
                from.SendMessage(2125, "What do you wish to move South? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 6:
                from.Target = new SET();
                from.SendMessage(2125, "What do you wish to move South West? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 7:
                from.Target = new ET();
                from.SendMessage(2125, "What do you wish to move East? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 8:
                from.Target = new NET();
                from.SendMessage(2125, "What do you wish to move North West? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 9:
                from.Target = new ZUT();
                from.SendMessage(2125, "What do you wish to Raise? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 10:
                from.Target = new ZDT();
                from.SendMessage(2125, "What do you wish to Lower? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 11:
                from.Target = new DT();
                from.SendMessage(2125, "What do you wish to Delete? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 12:
                from.Target = new UT();
                from.SendMessage(2125, "What do you wish to Lock/Unlock? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 13:
                from.Target = new HT();
                from.SendMessage(2125, "What do you wish to Hide/UnHide? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 14:
                from.Target = new PMT();
                from.SendMessage(2125, "What do you wish to Move? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 15:
                CommandSystem.Handle(from, $"{prefix}Add");
                from.SendMessage(2125, "Opening Add Menu");
                from.SendGump(new IceGMTool(from));
                break;
            case 16:
                CommandSystem.Handle(from, $"{prefix}Light 0");
                from.SendMessage(2125, "It is now light to you.");
                from.SendGump(new IceGMTool(from));
                break;
            case 17:
                CommandSystem.Handle(from, $"{prefix}SpeedBoost");
                from.SendGump(new IceGMTool(from));
                break;
            case 18:
                CommandSystem.Handle(from, $"{prefix}Get Hue ItemID BodyValue Location");
                from.SendMessage(2125, "What do you wish to get info from?");
                from.SendGump(new IceGMTool(from));
                break;
            case 19:
                CommandSystem.Handle(from, $"{prefix}SpeedBoost False");
                from.SendGump(new IceGMTool(from));
                break;
            case 20:
                from.SendGump(new iGO());
                from.SendMessage(2125, "Opening Travel Gump.");
                break;
            case 21:
                CommandSystem.Handle(from, $"{prefix}m Tele");
                from.SendMessage(2125, "Target Multi Tele Location.");
                from.SendGump(new IceGMTool(from));
                break;
            case 24:
                CommandSystem.Handle(from, $"{prefix}OB {website}");
                from.SendMessage(2125, "Target Player to Open Browser.");
                from.SendGump(new IceGMTool(from));
                break;
            case 25:
                CommandSystem.Handle(from, $"{prefix}Who");
                from.SendGump(new IceGMTool(from));
                break;
            case 26:
                CommandSystem.Handle(from, $"{prefix}Pages");
                from.SendGump(new IceGMTool(from));
                break;
            case 29:
                CommandSystem.Handle(from, $"{prefix}Admin");
                from.SendMessage(2125, "Opening Admin Menu.");
                from.SendGump(new IceGMTool(from));
                break;
            case 30:
                CommandSystem.Handle(from, $"{prefix}Restart");
                break;
            case 31:
                CommandSystem.Handle(from, $"{prefix}Props");
                from.SendMessage(2125, "What do you wish to get properties from?");
                from.SendGump(new IceGMTool(from));
                break;
            case 32:
                from.SendGump(new GMTHuePickerGump());
                break;
            case 33:
                CommandSystem.Handle(from, $"{prefix}m Kill");
                from.SendMessage(2125, "What do you wish to kill?");
                from.SendGump(new IceGMTool(from));
                break;
            case 34:
                CommandSystem.Handle(from, $"{prefix}m Res");
                from.SendMessage(2125, "What do you wish to resurrect?");
                from.SendGump(new IceGMTool(from));
                break;
            case 35:
                from.Target = new RSHT();
                from.SendMessage(2125, "What do you wish to dye a random skin hue? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
            case 36:
                from.Target = new RNHT();
                from.SendMessage(2125, "What do you wish to dye a random neutral hue? (ESC to cancel)");
                from.SendGump(new IceGMTool(from));
                break;
        }
    }

    // ── Direction targets ──────────────────────────────────────────────────

    private static void MoveItem(Item item, int dx, int dy, int dz = 0)
    {
        item.X += dx;
        item.Y += dy;
        item.Z += dz;
    }

    private static void MoveMobile(Mobile m, int dx, int dy, int dz = 0)
    {
        m.X += dx;
        m.Y += dy;
        m.Z += dz;
    }

    private class DirectionTarget : Target
    {
        private readonly int _dx, _dy, _dz;

        public DirectionTarget(int dx, int dy, int dz = 0) : base(-1, false, TargetFlags.None)
        {
            _dx = dx;
            _dy = dy;
            _dz = dz;
            CheckLOS = false;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new DirectionTarget(_dx, _dy, _dz);
                return;
            }
            if (targeted is Item item)
            {
                MoveItem(item, _dx, _dy, _dz);
            }
            else if (targeted is Mobile m)
            {
                MoveMobile(m, _dx, _dy, _dz);
            }
            from.Target = new DirectionTarget(_dx, _dy, _dz);
        }
    }

    // 8 direction aliases used by button IDs 1-8 and Z up/down 9-10
    private class NT  : DirectionTarget { public NT()  : base( 0, -1) {} }
    private class NWT : DirectionTarget { public NWT() : base( 1, -1) {} }
    private class WT  : DirectionTarget { public WT()  : base(-1,  0) {} }
    private class SWT : DirectionTarget { public SWT() : base( 1,  1) {} }
    private class ST  : DirectionTarget { public ST()  : base( 0,  1) {} }
    private class SET : DirectionTarget { public SET() : base(-1,  1) {} }
    private class ET  : DirectionTarget { public ET()  : base( 1,  0) {} }
    private class NET : DirectionTarget { public NET() : base(-1, -1) {} }
    private class ZUT : DirectionTarget { public ZUT() : base( 0,  0,  1) {} }
    private class ZDT : DirectionTarget { public ZDT() : base( 0,  0, -1) {} }

    // ── Delete ────────────────────────────────────────────────────────────

    private class DT : Target
    {
        public DT() : base(-1, false, TargetFlags.None) { CheckLOS = false; }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new DT();
                return;
            }
            if (targeted is Item item)
            {
                item.Delete();
                from.SendMessage(2125, string.IsNullOrEmpty(item.Name) ? "Item Deleted." : $"{item.Name} Deleted.");
            }
            else if (targeted is BaseVendor || targeted is BaseCreature)
            {
                var m = (Mobile)targeted;
                from.SendMessage(2125, $"{m.Name} Deleted.");
                m.Delete();
            }
            else
            {
                from.SendMessage(38, "Invalid Target.");
            }
            from.Target = new DT();
        }
    }

    // ── Lock / Unlock ─────────────────────────────────────────────────────

    private class UT : Target
    {
        public UT() : base(-1, false, TargetFlags.None) { CheckLOS = false; }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new UT();
                return;
            }
            if (targeted is Item item)
            {
                item.Movable = !item.Movable;
                if (item.Movable)
                {
                    from.SendMessage(2120, string.IsNullOrEmpty(item.Name) ? "Item UnFrozen." : $"{item.Name} UnFrozen.");
                }
                else
                {
                    from.SendMessage(2120, string.IsNullOrEmpty(item.Name) ? "Item frozen." : $"{item.Name} frozen.");
                }
                from.Target = new UT();
            }
            else if (targeted is Mobile m)
            {
                m.Frozen = !m.Frozen;
                from.SendMessage(2120, m.Frozen ? $"{m.Name} Frozen." : $"{m.Name} Unfrozen.");
                from.Target = new UT();
            }
            else
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new UT();
            }
        }
    }

    // ── Hide / Unhide ─────────────────────────────────────────────────────

    private class HT : Target
    {
        public HT() : base(-1, false, TargetFlags.None) { CheckLOS = false; }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new HT();
                return;
            }
            if (from.AccessLevel < AccessLevel.Counselor)
            {
                from.Target = new HT();
                return;
            }
            if (targeted is Item item)
            {
                item.Visible = !item.Visible;
                if (item.Visible)
                {
                    from.SendMessage(2120, string.IsNullOrEmpty(item.Name) ? "Item UnHidden." : $"{item.Name} UnHidden.");
                }
                else
                {
                    from.SendMessage(2120, string.IsNullOrEmpty(item.Name) ? "Item hidden." : $"{item.Name} hidden.");
                }
                from.Target = new HT();
            }
            else if (targeted is Mobile m)
            {
                m.Hidden = !m.Hidden;
                from.SendMessage(2120, m.Hidden ? $"{m.Name} Hidden." : $"{m.Name} Unhidden.");
                from.Target = new HT();
            }
            else
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new HT();
            }
        }
    }

    // ── Move to target ────────────────────────────────────────────────────

    private class PMT : Target
    {
        public PMT() : base(-1, false, TargetFlags.None) {}

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new PMT();
                return;
            }
            if (targeted is Static)
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new PMT();
                return;
            }
            if (targeted is Item item)
            {
                from.SendMessage(2120, string.IsNullOrEmpty(item.Name) ? "Item Targeted." : $"{item.Name} Targeted.");
                from.Target = new MT(item);
            }
            else if (targeted is Mobile m)
            {
                from.SendMessage(2120, $"{m.Name} Targeted.");
                from.Target = new MT(m);
            }
        }
    }

    private class MT : Target
    {
        private readonly object _obj;

        public MT(object o) : base(-1, true, TargetFlags.None) { _obj = o; }

        protected override void OnTarget(Mobile from, object o)
        {
            if (o is not IPoint3D ip) { return; }

            var dest = ip is Item destItem ? new Point3D(destItem.GetWorldTop()) : new Point3D(ip);

            if (_obj is Item item && !item.Deleted)
            {
                item.MoveToWorld(dest, from.Map);
                from.SendMessage(2120, string.IsNullOrEmpty(item.Name) ? "Item Moved." : $"{item.Name} Moved.");
                from.Target = new PMT();
            }
            else if (_obj is Mobile m && !m.Deleted)
            {
                m.MoveToWorld(dest, from.Map);
                from.SendMessage(2120, $"{m.Name} Moved.");
                from.Target = new PMT();
            }
        }
    }

    // ── Random hue targets ────────────────────────────────────────────────

    private static readonly int[] SkinHues =
    {
        0x83FE, 0x8407, 0x8401, 0x840B, 0x841D, 0x8407,
        0x840C, 0x8409, 0x8406, 0x8404, 0x841C, 0x83F4, 0x83F5, 0x83EB, 0x8421
    };

    private static readonly int[] NeutralHues =
    {
        1858, 1904, 1860, 1837, 1836, 1866, 1843, 1871, 1804, 1859, 1879, 1853,
        1885, 1847, 1803, 1813, 1895, 1813, 1808, 1889, 1899, 1870, 1867, 1847, 1801, 1829, 1816, 1906
    };

    private class RSHT : Target
    {
        public RSHT() : base(-1, false, TargetFlags.None) { CheckLOS = false; }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new RSHT();
                return;
            }
            var hue = Utility.RandomList(SkinHues);
            if (targeted is Item item) { item.Hue = hue; }
            else if (targeted is Mobile m) { m.Hue = hue; }
            from.Target = new RSHT();
        }
    }

    private class RNHT : Target
    {
        public RNHT() : base(-1, false, TargetFlags.None) { CheckLOS = false; }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendMessage(38, "Invalid Target.");
                from.Target = new RNHT();
                return;
            }
            var hue = Utility.RandomList(NeutralHues);
            if (targeted is Item item) { item.Hue = hue; }
            else if (targeted is Mobile m) { m.Hue = hue; }
            from.Target = new RNHT();
        }
    }
}
