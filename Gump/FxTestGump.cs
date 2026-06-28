using System;
using Server;
using Server.Custom.Effect;
using Server.Gumps;
using Server.Network;

namespace Server.Custom.Gumps
{
    // Click-driven tester for the boss FX mechanics. Open with [fxtest.
    // One row per mechanic: adjust values with +/- and press GO. Fires from your own location.
    public class FxTestGump : Gump
    {
        private const int White = 1153;
        private const int Yellow = 1271;

        // button art
        private const int Minus = 0x15E3, MinusP = 0x15E7;
        private const int Plus = 0x15E1, PlusP = 0x15E5;
        private const int Go = 0xFA5, GoP = 0xFA7;

        private readonly int _tiles;
        private readonly int _dirs;
        private readonly int _radius;
        private readonly int _meteors;
        private readonly int _eggs;
        private readonly int _cone;
        private readonly bool _hurt;

        public FxTestGump(
            int tiles = 5, int dirs = 4, int radius = 5, int meteors = 6, int eggs = 6, int cone = 6, bool hurt = false
        )
            : base(100, 100)
        {
            _tiles = Math.Clamp(tiles, 1, 20);
            _dirs = Math.Clamp(dirs, 1, 8);
            _radius = Math.Clamp(radius, 1, 15);
            _meteors = Math.Clamp(meteors, 1, 30);
            _eggs = Math.Clamp(eggs, 1, 20);
            _cone = Math.Clamp(cone, 1, 20);
            _hurt = hurt;

            Closable = true;
            Draggable = true;

            AddPage(0);
            AddBackground(0, 0, 470, 290, 9270);
            AddAlphaRegion(10, 10, 450, 270);

            AddLabel(20, 16, Yellow, "Teste de Mecanicas de Boss - Real Fantasia");

            // ---- Fire Line (tiles + directions) ----
            AddLabel(20, 52, Yellow, "Fire Line");
            Stepper(110, 50, "Tiles", _tiles, 1, 2);
            Stepper(230, 50, "Dir", _dirs, 3, 4);
            GoButton(395, 50, 5);

            // ---- Fire Nova ----
            AddLabel(20, 90, Yellow, "Fire Nova");
            Stepper(110, 88, "Raio", _radius, 6, 7);
            GoButton(395, 88, 8);

            // ---- Fire Meteor ----
            AddLabel(20, 128, Yellow, "Meteoro");
            Stepper(110, 126, "Qtd", _meteors, 10, 11);
            GoButton(395, 126, 12);

            // ---- Spider Eggs ----
            AddLabel(20, 166, Yellow, "Ovos de Aranha");
            Stepper(130, 164, "Qtd", _eggs, 13, 14);
            GoButton(395, 164, 15);

            // ---- Fire Cone (sopro na direcao que voce esta virado) ----
            AddLabel(20, 204, Yellow, "Cone / Sopro");
            Stepper(130, 202, "Tiles", _cone, 16, 17);
            GoButton(395, 202, 18);

            // ---- Damage toggle + close ----
            AddButton(20, 244, _hurt ? 0xD3 : 0xD2, _hurt ? 0xD2 : 0xD3, 9);
            AddLabel(48, 244, _hurt ? 0x26 : White, _hurt ? "Dano: LIGADO" : "Dano: desligado");

            AddButton(360, 244, 0xFB1, 0xFB3, 0);
            AddLabel(395, 245, White, "Fechar");
        }

        private void Stepper(int x, int y, string label, int value, int minusId, int plusId)
        {
            AddLabel(x, y + 2, White, label + ":");
            var bx = x + 50;
            AddButton(bx, y, Minus, MinusP, minusId);
            AddLabel(bx + 26, y + 2, Yellow, value.ToString());
            AddButton(bx + 48, y, Plus, PlusP, plusId);
        }

        private void GoButton(int x, int y, int id)
        {
            AddButton(x, y, Go, GoP, id);
            AddLabel(x + 35, y + 2, Yellow, "GO");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var m = sender.Mobile;

            if (m == null)
            {
                return;
            }

            int tiles = _tiles, dirs = _dirs, radius = _radius, meteors = _meteors, eggs = _eggs, cone = _cone;
            var hurt = _hurt;
            var dmg = hurt ? 20 : 0;
            var src = hurt ? m : null;

            switch (info.ButtonID)
            {
                case 0: return; // close
                case 1: tiles--; break;
                case 2: tiles++; break;
                case 3: dirs--; break;
                case 4: dirs++; break;
                case 5: FireLineEffect.ThrowRandom(m.Map, m.Location, tiles, dirs, default, src, dmg); break;
                case 6: radius--; break;
                case 7: radius++; break;
                case 8: FireNovaEffect.Burst(m.Map, m.Location, radius, default, src, dmg); break;
                case 10: meteors--; break;
                case 11: meteors++; break;
                case 12:
                    MeteorEffect.Shower(m.Map, m.Location, meteors, 4, 1.5, 5, hurt ? m : null, hurt ? 30 : 0, hurt ? 8 : 0);
                    break;
                case 13: eggs--; break;
                case 14: eggs++; break;
                case 15: SpiderEggEffect.Scatter(m.Map, m.Location, eggs, 4, 8.0, 3); break;
                case 16: cone--; break;
                case 17: cone++; break;
                case 18: FireConeEffect.Breath(m.Map, m.Location, m.Direction, cone, default, src, dmg); break;
                case 9: hurt = !hurt; break;
            }

            m.SendGump(new FxTestGump(tiles, dirs, radius, meteors, eggs, cone, hurt));
        }
    }
}
