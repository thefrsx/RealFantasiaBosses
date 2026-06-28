using System;
using System.Collections.Generic;
using ModernUO.Serialization;
using Server;
using Server.Mobiles;

namespace Server.Custom.Bosses
{
    // Base de boss do Real Fantasia (do passo 1 da seção 5.5 do estudo): "pulso" de habilidade no OnThink +
    // máquina de fases por % de HP. As três técnicas universais do estudo (gate OnThink, fases por HP, kit por
    // fase) ficam aqui; bosses concretos só montam o kit em BuildAbilities() e ajustam falas/stats.
    [SerializationGenerator(0, false)]
    public abstract partial class RFBoss : BaseCreature
    {
        // Persistido: sobrevive a restart no meio da luta. A LISTA de habilidades NÃO é serializada — é
        // reconstruída lazy no OnThink (o estudo avisou: kit no construtor "some" após restart).
        [SerializableField(0)]
        [SerializedCommandProperty(AccessLevel.GameMaster)]
        private int _currentPhase;

        private List<RFBossAbility> _abilities; // transitório
        private long _nextAbilityTime;

        public RFBoss(AIType ai, FightMode mode, int rangePerception, int rangeFight)
            : base(ai, mode, rangePerception, rangeFight)
        {
        }

        // CurrentPhase é gerado a partir de _currentPhase pelo [SerializableField].

        // Intervalo global entre habilidades (o boss concreto pode encurtar em enrage).
        public virtual TimeSpan AbilityInterval => TimeSpan.FromSeconds(5.0);

        protected void AddAbility(RFBossAbility a) => (_abilities ??= new List<RFBossAbility>()).Add(a);

        // O boss concreto monta o kit aqui (chamado lazy => sobrevive a restart).
        protected abstract void BuildAbilities();

        // Gancho de transição de fase (falas/sinatura). 'phase' = 0..3.
        protected virtual void OnPhaseChanged(int phase)
        {
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Deleted || Map == null || Map == Map.Internal)
            {
                return;
            }

            if (_abilities == null)
            {
                _abilities = new List<RFBossAbility>();
                BuildAbilities();
            }

            CheckPhase();

            var target = Combatant;
            if (target is not { Alive: true } || !InRange(target.Location, RangePerception))
            {
                return;
            }

            if (Core.TickCount < _nextAbilityTime)
            {
                return;
            }

            // Escolhe (amostragem-reservatório) uma habilidade pronta entre as elegíveis por vida%/cooldown.
            var pct = HitsMax > 0 ? Hits * 100 / HitsMax : 100;
            RFBossAbility chosen = null;
            var seen = 0;
            foreach (var a in _abilities)
            {
                if (a.Ready(pct) && Utility.Random(++seen) == 0)
                {
                    chosen = a;
                }
            }

            if (chosen == null)
            {
                return;
            }

            chosen.Trigger(this, target);
            _nextAbilityTime = Core.TickCount + (long)AbilityInterval.TotalMilliseconds;
        }

        private void CheckPhase()
        {
            var pct = HitsMax > 0 ? Hits * 100 / HitsMax : 100;
            var phase = pct >= 75 ? 0 : pct >= 50 ? 1 : pct >= 25 ? 2 : 3;

            if (phase > _currentPhase)
            {
                _currentPhase = phase;
                OnPhaseChanged(phase);
            }
        }
    }
}
