using Server.Gumps;
using Server.Network;

namespace Server.Custom.Gumps
{
    // Aviso (telegrafia) de mecânica de boss, mostrado aos jogadores próximos enquanto a habilidade "carrega".
    // Some quando a mecânica dispara. Ex.: "!! [Boss] esta conjurando: Muralha de Fogo !!" + "Todos cairao!".
    public sealed class BossWarningGump : Gump
    {
        public override bool Singleton => true;

        public BossWarningGump(string boss, string ability, string warn) : base(360, 70)
        {
            Closable = false;
            Draggable = true;

            const int width = 380;
            AddBackground(0, 0, width, 76, 9270);
            AddAlphaRegion(8, 8, width - 16, 60);

            AddLabel(18, 14, 0x22, $"!! {boss} esta conjurando: {ability} !!");

            if (!string.IsNullOrEmpty(warn))
            {
                AddLabel(18, 38, 0x35, warn);
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
        }
    }
}
