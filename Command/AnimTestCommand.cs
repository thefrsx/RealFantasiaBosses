using System;
using System.Globalization;
using Server;
using Server.Commands;
using Server.Targeting;

namespace Server.Custom.Commands
{
    // Dev tool: preview which animation actions a given body has.
    // Usage: [animtest            -> cycles actions 0..35 every 2.5s on a targeted mobile
    //        [animtest 0 40       -> actions 0..40
    //        [animtest 0 40 1.5   -> actions 0..40 every 1.5s
    // The action number is shown over the mobile's head; the mobile is frozen during the cycle.
    public static class AnimTestCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("animtest", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("animtest [start] [end] [intervalSeconds]")]
        [Description(
            "Faz um mobile alvo percorrer IDs de acao de animacao para descobrir quais o corpo dele possui. " +
            "Padrao: acoes 0..35 a cada 2.5s.")]
        public static void OnCommand(CommandEventArgs e)
        {
            var start = 0;
            var end = 35;
            var interval = 2.5;

            if (e.Length >= 1)
            {
                start = e.GetInt32(0);
            }

            if (e.Length >= 2)
            {
                end = e.GetInt32(1);
            }

            if (e.Length >= 3 &&
                double.TryParse(e.GetString(2), NumberStyles.Float, CultureInfo.InvariantCulture, out var iv))
            {
                interval = iv;
            }

            if (end < start)
            {
                end = start;
            }

            if (interval < 0.3)
            {
                interval = 0.3;
            }

            e.Mobile.SendMessage(0x35, $"AnimTest: selecione o alvo (acoes {start}..{end} a cada {interval}s).");
            e.Mobile.Target = new AnimTarget(start, end, interval);
        }

        private class AnimTarget : Target
        {
            private readonly int _start;
            private readonly int _end;
            private readonly double _interval;

            public AnimTarget(int start, int end, double interval) : base(12, false, TargetFlags.None)
            {
                _start = start;
                _end = end;
                _interval = interval;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is not Mobile m)
                {
                    from.SendMessage(0x22, "Alvo invalido: selecione um mobile.");
                    return;
                }

                from.SendMessage(0x35, $"AnimTest em '{m.Name ?? "mobile"}' (Body {(int)m.Body}): acoes {_start}..{_end}.");
                new AnimCycleTimer(from, m, _start, _end, _interval).Start();
            }
        }

        private class AnimCycleTimer : Timer
        {
            private readonly Mobile _gm;
            private readonly Mobile _target;
            private readonly int _end;
            private readonly bool _wasFrozen;
            private int _action;

            public AnimCycleTimer(Mobile gm, Mobile target, int start, int end, double interval)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(interval), end - start + 1)
            {
                _gm = gm;
                _target = target;
                _action = start;
                _end = end;
                _wasFrozen = target.Frozen;
                target.Frozen = true;
            }

            protected override void OnTick()
            {
                if (_target.Deleted || !_target.Alive)
                {
                    _target.Frozen = _wasFrozen;
                    Stop();
                    return;
                }

                var label = $"Action {_action}";
                _target.PublicOverheadMessage(MessageType.Regular, 0x35, true, label);
                _target.Animate(_action, 7, 1, true, false, 0);
                _gm.SendMessage($"  -> action {_action}");

                _action++;

                if (_action > _end)
                {
                    _target.Frozen = _wasFrozen;
                }
            }
        }
    }
}
