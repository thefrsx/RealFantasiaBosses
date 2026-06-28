using Server.Commands;
using Server.Network;

namespace Server.Gumps;

public class GMTHuePickerGump : Gump
{
    public GMTHuePickerGump() : base(0, 0)
    {
        Closable = false;
        Disposable = true;
        Draggable = true;
        Resizable = false;
        AddPage(1);
        AddButton(280, 255, 55, 55, 2, GumpButtonType.Reply, 2);
        AddButton(280, 202, 55, 55, 1, GumpButtonType.Reply, 1);
        AddImage(213, 188, 35);
        AddBackground(225, 245, 53, 34, 9200);
        AddImage(222, 240, 51);
        AddAlphaRegion(225, 251, 50, 24);
        AddTextEntry(234, 253, 35, 16, 52, 1, "");
        AddLabel(233, 218, 0, "Hue #");
        AddLabel(233, 205, 0, "Enter");
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        var from = sender.Mobile;
        var prefix = CommandSystem.Prefix;

        switch (info.ButtonID)
        {
            case 1:
            {
                from.SendGump(new IceGMTool(from));
                break;
            }
            case 2:
            {
                var hueText = info.GetTextEntry(1);
                if (hueText == null)
                {
                    break;
                }
                if (!int.TryParse(hueText, out var hue))
                {
                    from.SendMessage(38, "Please make sure you write only numbers.");
                    from.SendGump(new GMTHuePickerGump());
                    break;
                }
                if (hue >= 3001)
                {
                    from.SendMessage(38, "Cannot exceed 3000.");
                    from.SendGump(new GMTHuePickerGump());
                    break;
                }
                if (hue < 0)
                {
                    from.SendMessage(38, "Must be higher than 0.");
                    from.SendGump(new GMTHuePickerGump());
                    break;
                }
                CommandSystem.Handle(from, $"{prefix}m Set Hue {hue}");
                from.SendGump(new GMTHuePickerGump());
                from.SendMessage(hue == 0 ? 2125 : hue, hue == 0 ? "Using Default hue: 0" : $"What do you wish to hue {hue}?");
                break;
            }
        }
    }
}
