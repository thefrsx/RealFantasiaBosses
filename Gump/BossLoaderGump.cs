using System.Collections.Generic;
using Server;
using Server.Custom.Bosses;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Custom.Gumps
{
    // Gump carregador (GM): por mecânica você configura forma (tiles/raio/qtd), Avs (tempo de disparo/aviso), Dmg
    // (dano fixo), CD e Fase — como o [fxtest. Marca as desejadas, "Aplicar" e mira no mobile. Abrir: [bossload
    public sealed class BossLoaderGump : Gump
    {
        public override bool Singleton => true;

        public BossLoaderGump() : base(110, 70)
        {
            Closable = true;
            Draggable = true;

            var defs = BossCatalog.All;
            const int width = 472;
            const int blockStart = 56;
            const int blockH = 46;
            var height = blockStart + defs.Count * blockH + 8 + 34;

            AddPage(0);
            AddBackground(0, 0, width, height, 9270);
            AddAlphaRegion(8, 8, width - 16, height - 16);

            AddLabel(18, 14, 1271, "CARREGADOR DE BOSS");
            AddLabel(18, 32, 0x3B2, "Configure (Avs=aviso/disparo, Dmg=dano, CD, Vida%=desbloqueia em); marque e mire.");

            for (var i = 0; i < defs.Count; i++)
            {
                var d = defs[i];
                var by = blockStart + i * blockH;

                AddCheck(18, by, 0xD2, 0xD3, false, i);
                AddLabel(44, by + 1, 1271, $"[{d.Category}] {d.Display}");

                var fx = 30;
                var ly = by + 22;
                for (var j = 0; j < d.Shape.Length; j++)
                {
                    fx = Field(fx, ly, d.Shape[j].Label, i * 10 + j, d.Shape[j].Default);
                }

                fx = Field(fx, ly, "Avs", i * 10 + 2, d.Telegraph);
                fx = Field(fx, ly, "Dmg", i * 10 + 3, d.DamageDefault);
                fx = Field(fx, ly, "CD", i * 10 + 4, d.CooldownDefault);
                Field(fx, ly, "Vida%", i * 10 + 5, d.UnlockDefault);
            }

            AddButton(18, height - 34, 0xFA5, 0xFA7, 1);
            AddLabel(54, height - 32, 1271, "Aplicar (mirar no boss)");

            AddButton(280, height - 34, 0xFB1, 0xFB3, 2);
            AddLabel(316, height - 32, 0x25, "Remover");
        }

        private int Field(int x, int y, string label, int entryId, int def)
        {
            AddLabel(x, y + 2, 0x3B2, label);
            var bx = x + label.Length * 6 + 6;
            AddBackground(bx, y, 42, 22, 9350);
            AddTextEntry(bx + 4, y + 1, 32, 18, 0, entryId, def.ToString());
            return bx + 42 + 8;
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var m = sender.Mobile;
            if (m == null)
            {
                return;
            }

            if (info.ButtonID == 2)
            {
                m.SendMessage(0x40, "Mire no boss para remover o preset.");
                m.Target = new BossRemoveTarget();
                return;
            }

            if (info.ButtonID != 1)
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
                    shape[j] = Get(info, sw * 10 + j, d.Shape[j].Default);
                }

                var tel = Get(info, sw * 10 + 2, d.Telegraph);
                var dmg = Get(info, sw * 10 + 3, d.DamageDefault);
                var cd = Get(info, sw * 10 + 4, d.CooldownDefault);
                var unlock = Get(info, sw * 10 + 5, d.UnlockDefault);
                if (cd < 1)
                {
                    cd = 1;
                }

                kit.Add(d.Build(shape, tel, dmg, cd, unlock));
            }

            if (kit.Count == 0)
            {
                m.SendMessage(0x22, "Marque ao menos uma mecanica.");
                m.SendGump(new BossLoaderGump());
                return;
            }

            m.SendMessage(0x40, $"Mire no boss para carregar {kit.Count} mecanica(s).");
            m.Target = new BossApplyTarget(kit);
        }

        private static int Get(in RelayInfo info, int id, int dft) =>
            int.TryParse(info.GetTextEntry(id), out var v) ? v : dft;
    }

    public sealed class BossApplyTarget : Target
    {
        private readonly List<RFBossAbility> _kit;

        public BossApplyTarget(List<RFBossAbility> kit) : base(15, false, TargetFlags.None) => _kit = kit;

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is BaseCreature bc)
            {
                BossController.Attach(bc, _kit);
                from.SendMessage(0x40, $"{bc.Name}: preset de boss carregado ({_kit.Count} mecanicas).");
            }
            else
            {
                from.SendMessage(0x22, "Mire numa criatura (mobile).");
            }
        }
    }

    public sealed class BossRemoveTarget : Target
    {
        public BossRemoveTarget() : base(15, false, TargetFlags.None)
        {
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Mobile mob)
            {
                BossController.Detach(mob);
                from.SendMessage(0x40, "Preset de boss removido.");
            }
        }
    }
}
