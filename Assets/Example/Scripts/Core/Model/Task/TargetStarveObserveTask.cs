using System;
using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Kurisu.RealAgents.Example
{
    public class TargetStarveObserveTask : StatusTask, IAITask, IDisposable
    {
        public string TaskID { get; } = nameof(TargetStarveObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private WorldState hostWS;
        private ObserveProxyVariable<Object> target;
        public void Init(IAIHost host)
        {
            hostWS = host.WorldState;
            target = host.BlackBoard.GetSharedObject(Variables.Target).ObserveT();
            target.OnValueChange += OnTargetChanged;
        }
        private void OnTargetChanged(Object target)
        {
            if (worldState)
                worldState.OnStateUpdate -= OnUpdateState;
            worldState = null;
            if (target == null)
            {
                hostWS.SetState(States.TargetIsHungry, false);
                return;
            }
            worldState = (target as Transform).GetComponent<WorldState>();
            worldState.OnStateUpdate += OnUpdateState;
            hostWS.SetState(States.TargetIsHungry, worldState.GetState(States.IsHungry));
        }
        private void OnUpdateState(string state, bool isOn)
        {
            if (state != States.IsHungry) return;
            hostWS.SetState(States.TargetIsHungry, isOn);
        }

        public void Tick() { }

        public void Dispose()
        {
            if (worldState)
                worldState.OnStateUpdate -= OnUpdateState;
            target.OnValueChange -= OnTargetChanged;
        }
    }
}