using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class EnergyObserveTask : StatusTask, IAITask
    {
        public string TaskID { get; } = nameof(EnergyObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private SharedVariable<int> energy;
        private float timer;
        private readonly float tickTime;
        public EnergyObserveTask(float tickTime)
        {
            this.tickTime = tickTime;
        }
        public void Init(IAIHost host)
        {
            worldState = host.WorldState;
            energy = host.BlackBoard.GetInt(Variables.Energy, 100);
            energy.ObserveT().OnValueChange += (x) => worldState.SetState(States.HasEnergy, x > 0);
        }

        public void Tick()
        {
            if (energy.Value > 0)
            {
                timer += Time.deltaTime;
                if (timer >= tickTime)
                {
                    timer = 0;
                    --energy.Value;
                }
            }
        }
    }
}