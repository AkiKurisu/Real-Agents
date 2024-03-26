using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
namespace Kurisu.RealAgents.Example
{
    public class WaterObserveTask : StatusTask, IAITask
    {
        public string TaskID { get; } = nameof(WaterObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private SharedVariable<int> water;
        public void Init(IAIHost host)
        {
            worldState = host.WorldState;
            water = host.BlackBoard.GetInt(Variables.Water, 0);
            water.ObserveT().OnValueChange += (x) => worldState.SetState(States.HasWater, x > 0);
            worldState.SetState(States.HasWater, false);
        }
        public void Tick() { }
    }
}