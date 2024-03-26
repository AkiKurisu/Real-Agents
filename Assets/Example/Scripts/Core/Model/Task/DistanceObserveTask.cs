using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class DistanceObserveTask : StatusTask, IAITask
    {
        public DistanceObserveTask(float maxSqrMagnitude)
        {
            this.maxSqrMagnitude = maxSqrMagnitude;
        }
        private readonly float maxSqrMagnitude;
        public string TaskID => Tasks.DistanceObserve;
        public bool IsPersistent => false;
        private WorldState worldState;
        private SharedVariable<Object> target;
        private IAIHost host;
        public void Init(IAIHost host)
        {
            target = host.BlackBoard.GetSharedObject(Variables.Target);
            this.host = host;
            worldState = host.WorldState;
        }
        public void Tick()
        {
            if (target.Value != null)
            {
                var targetTransform = target.Value as Transform;
                worldState.SetState(States.InDistance, Vector3.SqrMagnitude(host.Transform.position - targetTransform.position) < maxSqrMagnitude);
            }
            else
            {
                worldState.SetState(States.InDistance, false);
            }
        }
    }
}