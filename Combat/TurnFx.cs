using Server;
using Server.Mobiles;

namespace Server.Custom.Combat
{
    // "Juice" do combate por turnos: efeitos visuais/sonoros que NÃO são nativos da engine.
    // (Números de dano e sangue já são enviados pelo ModernUO em melee — não duplicamos aqui.)
    internal static class TurnFx
    {
        // Telegrafia do inimigo: durante o "beat" antes do golpe, o mob carrega/brilha + avisa + som.
        // É o equivalente turn-based da telegrafia dos bosses: o jogador VÊ o ataque vindo.
        public static void Windup(Mobile mob)
        {
            if (mob is not { Deleted: false })
            {
                return;
            }

            mob.FixedParticles(0x3779, 10, 25, 5032, 0x21, 0, EffectLayer.Head);
            mob.PublicOverheadMessage(MessageType.Regular, 0x21, true, "* prepara o golpe *");
        }

        // Golpe acertando: faísca de impacto sobre quem apanhou (reforça o feedback do número nativo).
        public static void Impact(Mobile victim)
        {
            if (victim is not { Deleted: false })
            {
                return;
            }

            victim.FixedParticles(0x374A, 10, 15, 5028, 0x21, 0, EffectLayer.Waist);
        }

        // Ataque de oportunidade: flash vermelho distinto + som + texto, pra LER como punição por largar o corpo-a-corpo.
        public static void Opportunity(Mobile victim)
        {
            if (victim is not { Deleted: false })
            {
                return;
            }

            victim.FixedParticles(0x3728, 10, 15, 5028, 0x26, 0, EffectLayer.Head);
            victim.PublicOverheadMessage(MessageType.Regular, 0x26, true, "Oportunidade!");
        }

        // Postura de defesa: brilho de escudo no jogador.
        public static void Defend(Mobile m)
        {
            if (m is not { Deleted: false })
            {
                return;
            }

            m.FixedParticles(0x375A, 10, 15, 5027, 0x47E, 0, EffectLayer.Waist);
        }

        // Fuga bem-sucedida: pequeno "poof" + som.
        public static void FleeSuccess(Mobile m)
        {
            if (m is not { Deleted: false })
            {
                return;
            }

            Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
        }
    }
}
