using Server.Gumps;
using Server.Network;

namespace Server.Custom.Gumps
{
    // Banner transitório de início de turno ("SUA VEZ" / "TURNO DO INIMIGO").
    // Aparece por ~1.4s sobre a tela (o TurnEncounter agenda o fechamento). Não-interativo.
    public sealed class TurnBannerGump : Gump
    {
        public override bool Singleton => true;

        public TurnBannerGump(bool playersTurn) : base(360, 92)
        {
            Closable = false;
            Draggable = false;

            const int width = 250;
            AddBackground(0, 0, width, 50, 9270);
            AddAlphaRegion(8, 8, width - 16, 34);

            if (playersTurn)
            {
                AddLabel(72, 16, 0x3F, ">>>  SUA VEZ  <<<");
            }
            else
            {
                AddLabel(50, 16, 0x25, ">>  TURNO DO INIMIGO  <<");
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
        }
    }
}
