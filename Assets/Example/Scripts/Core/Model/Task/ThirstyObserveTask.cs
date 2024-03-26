using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class ThirstyObserveTask : StatusTask, IAITask
    {
        public string TaskID { get; } = nameof(ThirstyObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private SharedVariable<int> thirsty;
        private float timer;
        public void Init(IAIHost host)
        {
            worldState = host.WorldState;
            thirsty = host.BlackBoard.GetInt(Variables.Thirst, 0);
            thirsty.ObserveT().OnValueChange += (x) => worldState.SetState(States.IsThirsty, x >= 100);
        }
        public void Tick()
        {
            if (thirsty.Value < 100)
            {
                timer += Time.deltaTime;
                if (timer >= 0.6f)
                {
                    timer = 0;
                    ++thirsty.Value;
                }
            }
        }
    }
}