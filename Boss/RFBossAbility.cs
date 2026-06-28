using System;
using Server;

namespace Server.Custom.Bosses
{
    // Objeto de habilidade de boss (padrão "AbilityCreature 3.0" do estudo), agora agnóstico de classe:
    // funciona tanto no RFBoss (coded) quanto carregado num mobile qualquer via BossController.
    // Inclui metadados de TELEGRAFIA (nome + texto de aviso + segundos) para o BossWarningGump.
    public abstract class RFBossAbility
    {
        public string Name { get; init; } = "Habilidade";
        public string WarningText { get; init; } = "";
        public double TelegraphSeconds { get; init; } = 3.0;

        public TimeSpan Cooldown { get; init; } = TimeSpan.FromSeconds(10);
        // Desbloqueia quando a vida do boss (%) está IGUAL OU ABAIXO disto. 100 = ativa desde o início.
        public int UnlockAtPercent { get; init; } = 100;
        public int Damage { get; init; } = 20;

        private long _nextReady;

        // Telegrafia NO CHÃO: marca DELICADA (faísca pequena) pintada durante a janela de aviso.
        // Cor por efeito — override TeleHue/TeleId p/ casar com o que vai sair. Default = neutro.
        protected virtual int TeleId => AbilityFx.TeleSpark;
        protected virtual int TeleHue => AbilityFx.HueNeutral;
        public int TelegraphRadius { get; init; } = 2;

        // Pinta a area que SERA atingida (repintada a cada tick do controller = pulsa). Override p/ formato exato.
        // Default = só o CONTORNO da zona (delicado), não o disco cheio.
        public virtual void PaintTelegraph(Mobile boss, Mobile target)
        {
            var map = boss.Map;
            if (map == null)
            {
                return;
            }

            var c = target?.Location ?? boss.Location;
            if (TelegraphRadius <= 1)
            {
                AbilityFx.Mark(c, map, TeleId, TeleHue);
            }
            else
            {
                AbilityFx.RingMark(c, map, TelegraphRadius, TeleId, TeleHue);
            }
        }

        public bool Ready(int hpPercent) => hpPercent <= UnlockAtPercent && Core.TickCount >= _nextReady;
        public void MarkUsed() => _nextReady = Core.TickCount + (long)Cooldown.TotalMilliseconds;
        public void Trigger(Mobile boss, Mobile target)
        {
            Use(boss, target);
            MarkUsed();
        }

        public abstract void Use(Mobile boss, Mobile target);
    }
}
