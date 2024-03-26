using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class HungerObserveTask : StatusTask, IAITask
    {
        public string TaskID { get; } = nameof(HungerObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private SharedVariable<int> hunger;
        private float timer;
        public void Init(IAIHost host)
        {
            worldState = host.WorldState;
            hunger = host.BlackBoard.GetInt(Variables.Hunger, 0);
            hunger.ObserveT().OnValueChange += (x) => worldState.SetState(States.IsHungry, x >= 100);
        }

        public void Tick()
        {
            if (hunger.Value < 100)
            {
                timer += Time.deltaTime;
                if (timer >= 1.5f)
                {
                    timer = 0;
                    ++hunger.Value;
                }
            }
        }
    }
}