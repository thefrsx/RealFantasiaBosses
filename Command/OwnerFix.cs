using System;
using Server;
using Server.Accounting;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom
{
    // Conveniencia de dev: garante que a conta "make" esteja sempre em Owner.
    // Usa um timer (roda no servidor a cada 2s) em vez de evento de login, pra
    // funcionar mesmo sem relogar e independente de timing de evento.
    public static class OwnerFix
    {
        private const string OwnerAccount = "make";

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0), 0, Check);
        }

        private static void Check()
        {
            foreach (var ns in NetState.Instances)
            {
                if (ns.Mobile is not PlayerMobile pm || pm.Account is not Account acct)
                {
                    continue;
                }

                if (!string.Equals(acct.Username, OwnerAccount, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (acct.AccessLevel < AccessLevel.Owner || pm.AccessLevel < AccessLevel.Owner)
                {
                    acct.AccessLevel = AccessLevel.Owner;
                    pm.AccessLevel = AccessLevel.Owner;
                    pm.SendMessage(0x40, "Acesso Owner restaurado.");
                }
            }
        }
    }
}
