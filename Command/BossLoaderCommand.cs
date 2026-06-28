using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Custom.Bosses;
using Server.Custom.Gumps;
using Server.Gumps;

namespace Server.Custom.Commands
{
    // Abre o gump carregador de boss. Uso: [bossload
    public static class BossLoaderCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("bossload", AccessLevel.GameMaster, OnCommand);
            CommandSystem.Register("bossloadtest", AccessLevel.GameMaster, OnCommandTest);
        }

        [Usage("bossload")]
        [Description("Abre o carregador de mecanicas de boss: marque e mire num mobile para transforma-lo em boss.")]
        public static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new BossLoaderGump());
        }

        [Usage("bossloadtest")]
        [Description("Teste rapido: abre um gump para escolher QUAIS mecanicas carregar (dano 10, CD 10s, sem trava de HP).")]
        public static void OnCommandTest(CommandEventArgs e)
        {
            e.Mobile.SendGump(new BossLoadTestGump());
        }
    }
}
