using System.Collections.Generic;
using Server.Custom.Combat;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Gumps
{
    // HUD do combate por turnos: vez + countdown, movimento restante, barras de HP, barra de AÇÕES (só na sua vez),
    // e um REGISTRO de combate no rodapé (eventos ficam visíveis aqui em vez do chat do sistema).
    public sealed class TurnHudGump : Gump
    {
        public sealed class HpEntry
        {
            public readonly string Name;
            public readonly int Cur;
            public readonly int Max;
            public readonly bool IsMob;

            public HpEntry(string name, int cur, int max, bool isMob)
            {
                Name = name;
                Cur = cur;
                Max = max;
                IsMob = isMob;
            }
        }

        public sealed class LogLine
        {
            public readonly string Text;
            public readonly int Hue;

            public LogLine(string text, int hue)
            {
                Text = text;
                Hue = hue;
            }
        }

        public const int BtnFlee = 1;
        public const int BtnAttack = 2;
        public const int BtnPotion = 3;
        public const int BtnScroll = 4;
        public const int BtnPass = 5;
        public const int BtnDefend = 6;
        public const int BtnSpell = 7;

        private const int Gold = 1271;
        private const int Green = 0x3F;
        private const int Red = 0x25;
        private const int Grey = 0x3B2;
        private const int Yellow = 0x35;
        private const int White = 1153;

        private readonly TurnEncounter _enc;
        private readonly PlayerMobile _viewer;

        public override bool Singleton => true;

        public TurnHudGump(
            TurnEncounter enc, PlayerMobile viewer, bool playersTurn, int secondsLeft,
            int moveUsed, int moveBudget, List<HpEntry> parts, bool canAct, List<LogLine> log
        ) : base(170, 25)
        {
            _enc = enc;
            _viewer = viewer;

            Closable = false;
            Draggable = true;

            var rows = parts.Count;
            const int width = 320;
            const int logCapacity = 6; // reserva fixa (== MaxLog) para o gump NUNCA mudar de tamanho

            var contentBottom = 84 + rows * 22;
            var actionTop = contentBottom + 6;
            var actionBottom = actionTop + 4 * 26; // sempre reserva a área de ações
            var logTop = actionBottom + 16;
            var height = logTop + 22 + logCapacity * 17 + 20;

            AddPage(0);
            AddBackground(0, 0, width, height, 9270);
            AddAlphaRegion(8, 8, width - 16, height - 16);

            AddLabel(18, 14, Gold, "COMBATE POR TURNOS");

            if (playersTurn)
            {
                AddLabel(18, 38, Green, $">> SUA VEZ   ({secondsLeft}s)");

                var remaining = moveBudget - moveUsed;
                if (remaining < 0)
                {
                    remaining = 0;
                }

                AddLabel(18, 58, remaining > 0 ? Green : Red, $"Movimento: {remaining} de {moveBudget} tiles");
            }
            else
            {
                AddLabel(18, 38, Red, $">> TURNO DO INIMIGO   ({secondsLeft}s)");
                AddLabel(18, 58, Grey, "Aguarde sua vez...");
            }

            var y = 84;
            foreach (var e in parts)
            {
                var nameHue = e.IsMob ? Red : Green;
                var name = e.Name.Length > 14 ? e.Name.Substring(0, 14) : e.Name;
                AddLabel(18, y, nameHue, name);

                const int barX = 140;
                const int barW = 100;
                var pct = e.Max > 0 ? e.Cur * 100 / e.Max : 0;
                if (pct < 0)
                {
                    pct = 0;
                }
                else if (pct > 100)
                {
                    pct = 100;
                }

                var fillW = barW * pct / 100;

                AddImageTiled(barX, y + 2, barW, 12, 2624);
                if (fillW > 0)
                {
                    AddImageTiled(barX, y + 2, fillW, 12, 0xBBC);
                }

                var pctHue = pct <= 20 ? Red : pct <= 50 ? Yellow : Green;
                AddLabel(barX + barW + 8, y, pctHue, $"{pct}%");

                y += 22;
            }

            if (canAct)
            {
                var ay = actionTop;
                ActionButton(18, ay, BtnAttack, "Atacar");
                ActionButton(170, ay, BtnPotion, "Pocao");
                ActionButton(18, ay + 26, BtnScroll, "Pergaminho");
                ActionButton(170, ay + 26, BtnSpell, "Magia");
                ActionButton(18, ay + 52, BtnDefend, "Defender");
                ActionButton(170, ay + 52, BtnPass, "Passar");
                ActionButton(18, ay + 78, BtnFlee, "Fugir");
            }

            // ---- Registro de combate ----
            AddImageTiled(14, logTop, width - 28, 14, 2624);
            AddLabel(18, logTop, Gold, "Registro");

            var ly = logTop + 22;
            foreach (var line in log)
            {
                var t = line.Text.Length > 52 ? line.Text.Substring(0, 52) : line.Text;
                AddLabel(18, ly, line.Hue, t);
                ly += 17;
            }
        }

        private void ActionButton(int x, int y, int id, string label)
        {
            AddButton(x, y, 0xFA5, 0xFA7, id);
            AddLabel(x + 32, y + 2, White, label);
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case BtnFlee: _enc.TryFlee(_viewer); break;
                case BtnAttack: _enc.DoAttack(_viewer); break;
                case BtnPotion: _enc.OpenPotions(_viewer); break;
                case BtnScroll: _enc.OpenScrolls(_viewer); break;
                case BtnPass: _enc.DoPass(_viewer); break;
                case BtnDefend: _enc.DoDefend(_viewer); break;
                case BtnSpell: _enc.OpenSpells(_viewer); break;
            }
        }
    }
}
