using System;
using Kurisu.AkiAI;
using Kurisu.GOAP;
using Object = UnityEngine.Object;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class TargetAtWorkObserveTask : StatusTask, IAITask, IDisposable
    {
        public string TaskID { get; } = nameof(TargetAtWorkObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private WorldState hostWS;
        private IAIHost host;
        private ObserveProxyVariable<Object> target;
        public void Init(IAIHost host)
        {
            this.host = host;
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
                hostWS.SetState(States.TargetIsWorking, false);
                return;
            }
            worldState = (target as Transform).GetComponent<WorldState>();
            worldState.OnStateUpdate += OnUpdateState;
            hostWS.SetState(States.TargetIsWorking, worldState.GetState(States.IsWorking));
        }
        private void OnUpdateState(string state, bool isOn)
        {
            if (state != States.IsWorking) return;
            hostWS.SetState(States.TargetIsWorking, isOn);
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