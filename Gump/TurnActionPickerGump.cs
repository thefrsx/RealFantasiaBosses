using System.Collections.Generic;
using Server.Custom.Combat;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Gumps
{
    // Seletor de itens para uma ação de turno (poções / pergaminhos). Lista os itens reais da mochila;
    // clicar usa o item via o encontro. Gump separado do HUD para não ser resetado pelo countdown.
    public sealed class TurnActionPickerGump : Gump
    {
        private const int Gold = 1271;
        private const int White = 1153;

        private readonly TurnEncounter _enc;
        private readonly PlayerMobile _viewer;

        public override bool Singleton => true;

        public TurnActionPickerGump(TurnEncounter enc, PlayerMobile viewer, string title, List<string> labels)
            : base(330, 120)
        {
            _enc = enc;
            _viewer = viewer;

            Closable = true;
            Draggable = true;

            var rows = labels.Count;
            var height = 60 + rows * 26 + 16;

            AddPage(0);
            AddBackground(0, 0, 260, height, 9270);
            AddAlphaRegion(8, 8, 244, height - 16);

            AddLabel(18, 14, Gold, title);

            var y = 44;
            for (var i = 0; i < rows; i++)
            {
                AddButton(18, y, 0xFA5, 0xFA7, i + 1);
                AddLabel(54, y + 2, White, labels[i]);
                y += 26;
            }

            AddButton(18, height - 26, 0xFB1, 0xFB3, 0);
            AddLabel(54, height - 24, White, "Cancelar");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            if (info.ButtonID >= 1)
            {
                _enc.UsePickedIndex(_viewer, info.ButtonID - 1);
            }
        }
    }
}
