using System.Collections.Generic;
using Server;
using Server.Custom.Bosses;
using Server.Gumps;
using Server.Network;

namespace Server.Custom.Gumps
{
    // Teste rapido: marque QUAIS mecanicas testar; carrega so as marcadas com dano 10, CD 10s e sem trava de HP.
    // Abrir: [bossloadtest
    public sealed class BossLoadTestGump : Gump
    {
        public override bool Singleton => true;

        public BossLoadTestGump() : base(110, 70)
        {
            Closable = true;
            Draggable = true;

            var defs = BossCatalog.All;
            const int width = 330;
            const int rowStart = 52;
            const int rowH = 24;
            var height = rowStart + defs.Count * rowH + 8 + 34;

            AddPage(0);
            AddBackground(0, 0, width, height, 9270);
            AddAlphaRegion(8, 8, width - 16, height - 16);

            AddLabel(18, 14, 1271, "TESTE DE MECANICAS");
            AddLabel(18, 32, 0x3B2, "Marque quais testar (dano 10, CD 10s, sem HP).");

            for (var i = 0; i < defs.Count; i++)
            {
                var d = defs[i];
                var by = rowStart + i * rowH;
                AddCheck(18, by, 0xD2, 0xD3, false, i);
                AddLabel(44, by + 1, 1271, $"[{d.Category}] {d.Display}");
            }

            AddButton(18, height - 34, 0xFA5, 0xFA7, 1);
            AddLabel(54, height - 32, 1271, "Carregar (mirar no boss)");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var m = sender.Mobile;
            if (m == null || info.ButtonID != 1)
            {
                return;
            }

            var defs = BossCatalog.All;
            var kit = new List<RFBossAbility>();
            foreach (var sw in info.Switches)
            {
                if (sw < 0 || sw >= defs.Count)
                {
                    continue;
                }

                var d = defs[sw];
                var shape = new int[d.Shape.Length];
                for (var j = 0; j < shape.Length; j++)
                {
                    shape[j] = d.Shape[j].Default;
                }

                // dano fixo 10, cooldown 10s, unlock 100% (sempre ativa); telegrafia = default da mecanica.
                kit.Add(d.Build(shape, d.Telegraph, 10, 10, 100));
            }

            if (kit.Count == 0)
            {
                m.SendMessage(0x22, "Marque ao menos uma mecanica.");
                m.SendGump(new BossLoadTestGump());
                return;
            }

            m.SendMessage(0x40, $"TESTE: mire no boss para carregar {kit.Count} mecanica(s) (dano 10, CD 10s, sem HP).");
            m.Target = new BossApplyTarget(kit);
        }
    }
}
