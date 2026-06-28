using System;
using Server.Custom.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Gumps
{
    // Editor do Monster In A Box: edita todos os stats do template, cicla IA/FightMode, valida Dano/Resist (=100)
    // e cria a criatura. "Salvar" grava o template sem spawnar; "Criar" spawna no seu lugar.
    public sealed class MonsterBoxGump : Gump
    {
        // text entry ids
        private const int E_Name = 0, E_Hue = 1, E_Body = 2, E_RangeP = 3, E_RangeF = 4, E_ActSpd = 5, E_PasSpd = 6;
        private const int E_StrMin = 7, E_StrMax = 8, E_DexMin = 9, E_DexMax = 10, E_IntMin = 11, E_IntMax = 12;
        private const int E_Hits = 13, E_Stam = 14, E_Mana = 15, E_Fame = 16, E_Karma = 17, E_DmgMin = 18, E_DmgMax = 19;
        private const int E_DPhys = 20, E_DFire = 21, E_DCold = 22, E_DPois = 23, E_DNrgy = 24;
        private const int E_RPhys = 25, E_RFire = 26, E_RCold = 27, E_RPois = 28, E_RNrgy = 29, E_VArmor = 30;

        // button ids
        private const int B_Close = 0, B_Spawn = 1, B_Save = 2, B_AiUp = 3, B_AiDown = 4, B_FmUp = 5, B_FmDown = 6;

        private const int Gold = 1271, Grey = 0x3B2, White = 1153;

        private readonly MonsterBoxItem _box;

        public override bool Singleton => true;

        public MonsterBoxGump(MonsterBoxItem box) : base(60, 40)
        {
            _box = box;
            Closable = true;
            Draggable = true;

            const int width = 545;
            const int height = 426;

            AddPage(0);
            AddBackground(0, 0, width, height, 9270);
            AddAlphaRegion(8, 8, width - 16, height - 16);

            AddLabel(18, 12, Gold, "MONSTER IN A BOX  -  editor de criatura");

            const int lx = 18;
            const int rx = 285;
            var y = 44;
            const int step = 26;

            // ---- coluna esquerda ----
            Single(lx, y, "Nome", E_Name, _box.MobName, 150);
            Single(lx, y += step, "Hue", E_Hue, _box.MobHue.ToString());
            Single(lx, y += step, "Body", E_Body, _box.MobBody.ToString());

            y += step;
            AddLabel(lx, y, Grey, "IA:");
            AddLabel(lx + 40, y, White, _box.Ai.ToString());
            AddButton(lx + 200, y, 0x983, 0x984, B_AiUp);
            AddButton(lx + 216, y, 0x985, 0x986, B_AiDown);

            y += step;
            AddLabel(lx, y, Grey, "Luta:");
            AddLabel(lx + 40, y, White, _box.FightMode.ToString());
            AddButton(lx + 200, y, 0x983, 0x984, B_FmUp);
            AddButton(lx + 216, y, 0x985, 0x986, B_FmDown);

            Single(lx, y += step, "Percep", E_RangeP, _box.RangePerception.ToString());
            Single(lx, y += step, "AlcLuta", E_RangeF, _box.RangeFight.ToString());
            Single(lx, y += step, "VelAtiv", E_ActSpd, _box.ActiveSpeed.ToString("0.0"));
            Single(lx, y += step, "VelPass", E_PasSpd, _box.PassiveSpeed.ToString("0.0"));

            // ---- coluna direita ----
            y = 44;
            MinMax(rx, y, "For", E_StrMin, E_StrMax, _box.StrMin, _box.StrMax);
            MinMax(rx, y += step, "Des", E_DexMin, E_DexMax, _box.DexMin, _box.DexMax);
            MinMax(rx, y += step, "Int", E_IntMin, E_IntMax, _box.IntMin, _box.IntMax);
            Single(rx, y += step, "Vida", E_Hits, _box.Hits.ToString());
            Single(rx, y += step, "Stam", E_Stam, _box.Stam.ToString());
            Single(rx, y += step, "Mana", E_Mana, _box.Mana.ToString());
            Single(rx, y += step, "Fama", E_Fame, _box.Fame.ToString());
            Single(rx, y += step, "Karma", E_Karma, _box.Karma.ToString());
            MinMax(rx, y += step, "Dano", E_DmgMin, E_DmgMax, _box.DamageMin, _box.DamageMax);

            // ---- linhas de dano/resist (somam 100) ----
            var py = 300;
            Penta(py, "Dano% (soma 100)", E_DPhys, _box.DmgPhys, _box.DmgFire, _box.DmgCold, _box.DmgPois, _box.DmgNrgy);
            Penta(py + 26, "Resist% (soma 100)", E_RPhys, _box.ResPhys, _box.ResFire, _box.ResCold, _box.ResPois, _box.ResNrgy);

            Single(18, py + 54, "Arm.Virtual", E_VArmor, _box.VirtualArmor.ToString(), 56, 78);

            // ---- botões ----
            AddButton(18, height - 30, 0xFA5, 0xFA7, B_Spawn);
            AddLabel(54, height - 28, Gold, "CRIAR (spawna aqui)");

            AddButton(230, height - 30, 0xFA5, 0xFA7, B_Save);
            AddLabel(266, height - 28, White, "Salvar template");

            AddButton(420, height - 30, 0xFB1, 0xFB3, B_Close);
            AddLabel(456, height - 28, White, "Fechar");
        }

        private void Single(int x, int y, string label, int id, string val, int boxW = 56, int labelW = 56)
        {
            AddLabel(x, y, Grey, label);
            var bx = x + labelW;
            AddBackground(bx, y - 2, boxW, 22, 9350);
            AddTextEntry(bx + 4, y - 1, boxW - 8, 18, 0, id, val);
        }

        private void MinMax(int x, int y, string label, int idMin, int idMax, int vMin, int vMax)
        {
            AddLabel(x, y, Grey, label);
            AddBackground(x + 40, y - 2, 44, 22, 9350);
            AddTextEntry(x + 44, y - 1, 36, 18, 0, idMin, vMin.ToString());
            AddBackground(x + 90, y - 2, 44, 22, 9350);
            AddTextEntry(x + 94, y - 1, 36, 18, 0, idMax, vMax.ToString());
        }

        private void Penta(int y, string label, int idBase, int v0, int v1, int v2, int v3, int v4)
        {
            AddLabel(18, y, Grey, label);
            var names = new[] { "Fis", "Fog", "Fri", "Ven", "Ene" };
            var vals = new[] { v0, v1, v2, v3, v4 };
            var x = 200;
            for (var i = 0; i < 5; i++)
            {
                AddLabel(x, y, White, names[i]);
                AddBackground(x + 24, y - 2, 40, 22, 9350);
                AddTextEntry(x + 28, y - 1, 32, 18, 0, idBase + i, vals[i].ToString());
                x += 68;
            }
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            var from = sender.Mobile;
            if (from == null)
            {
                return;
            }

            if (info.ButtonID == B_Close)
            {
                return;
            }

            // Sempre lê os campos editados de volta para o template (preserva edições ao ciclar/salvar/criar).
            ReadInto(_box, info);

            switch (info.ButtonID)
            {
                case B_AiUp: _box.Ai = Cycle(_box.Ai, 1); break;
                case B_AiDown: _box.Ai = Cycle(_box.Ai, -1); break;
                case B_FmUp: _box.FightMode = Cycle(_box.FightMode, 1); break;
                case B_FmDown: _box.FightMode = Cycle(_box.FightMode, -1); break;
                case B_Save:
                    from.SendMessage(0x40, "Template salvo.");
                    break;
                case B_Spawn:
                    var dmgSum = _box.DmgPhys + _box.DmgFire + _box.DmgCold + _box.DmgPois + _box.DmgNrgy;
                    var resSum = _box.ResPhys + _box.ResFire + _box.ResCold + _box.ResPois + _box.ResNrgy;
                    if (dmgSum != 100 || resSum != 100)
                    {
                        from.SendMessage(0x22, $"Dano (={dmgSum}) e Resistencia (={resSum}) precisam somar 100.");
                        break;
                    }

                    _box.Spawn(from);
                    break;
            }

            from.SendGump(new MonsterBoxGump(_box));
        }

        private static void ReadInto(MonsterBoxItem b, in RelayInfo info)
        {
            var name = info.GetTextEntry(E_Name);
            if (!string.IsNullOrWhiteSpace(name))
            {
                b.MobName = name;
            }

            b.MobHue = Gi(info, E_Hue, b.MobHue);
            b.MobBody = Gi(info, E_Body, b.MobBody);
            b.RangePerception = Gi(info, E_RangeP, b.RangePerception);
            b.RangeFight = Gi(info, E_RangeF, b.RangeFight);
            b.ActiveSpeed = Gd(info, E_ActSpd, b.ActiveSpeed);
            b.PassiveSpeed = Gd(info, E_PasSpd, b.PassiveSpeed);

            b.StrMin = Gi(info, E_StrMin, b.StrMin);
            b.StrMax = Gi(info, E_StrMax, b.StrMax);
            b.DexMin = Gi(info, E_DexMin, b.DexMin);
            b.DexMax = Gi(info, E_DexMax, b.DexMax);
            b.IntMin = Gi(info, E_IntMin, b.IntMin);
            b.IntMax = Gi(info, E_IntMax, b.IntMax);

            b.Hits = Gi(info, E_Hits, b.Hits);
            b.Stam = Gi(info, E_Stam, b.Stam);
            b.Mana = Gi(info, E_Mana, b.Mana);
            b.Fame = Gi(info, E_Fame, b.Fame);
            b.Karma = Gi(info, E_Karma, b.Karma);
            b.DamageMin = Gi(info, E_DmgMin, b.DamageMin);
            b.DamageMax = Gi(info, E_DmgMax, b.DamageMax);

            b.DmgPhys = Gi(info, E_DPhys, b.DmgPhys);
            b.DmgFire = Gi(info, E_DFire, b.DmgFire);
            b.DmgCold = Gi(info, E_DCold, b.DmgCold);
            b.DmgPois = Gi(info, E_DPois, b.DmgPois);
            b.DmgNrgy = Gi(info, E_DNrgy, b.DmgNrgy);

            b.ResPhys = Gi(info, E_RPhys, b.ResPhys);
            b.ResFire = Gi(info, E_RFire, b.ResFire);
            b.ResCold = Gi(info, E_RCold, b.ResCold);
            b.ResPois = Gi(info, E_RPois, b.ResPois);
            b.ResNrgy = Gi(info, E_RNrgy, b.ResNrgy);

            b.VirtualArmor = Gi(info, E_VArmor, b.VirtualArmor);
        }

        private static int Gi(in RelayInfo info, int id, int dft) =>
            int.TryParse(info.GetTextEntry(id), out var v) ? v : dft;

        private static double Gd(in RelayInfo info, int id, double dft) =>
            double.TryParse(info.GetTextEntry(id), out var v) ? v : dft;

        private static T Cycle<T>(T cur, int dir) where T : struct, Enum
        {
            var vals = Enum.GetValues<T>();
            var idx = Array.IndexOf(vals, cur);
            if (idx < 0)
            {
                idx = 0;
            }

            idx = (idx + dir + vals.Length) % vals.Length;
            return vals[idx];
        }
    }
}
