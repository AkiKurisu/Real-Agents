using System;
using Kurisu.AkiAI;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    /// <summary>
    /// Task controlled NPCs' work cycle
    /// </summary>
    public class WorkObserveTask : StatusTask, IAITask, IDisposable
    {
        public string TaskID { get; } = nameof(WorkObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private readonly float maxWorkTime;
        private readonly float maxSpareTime;
        private bool isWorking;
        private float timer;
        public WorkObserveTask(float maxWorkTime, float maxSpareTime, bool randomizeInitialValue)
        {
            this.maxWorkTime = maxWorkTime;
            this.maxSpareTime = maxSpareTime;
            if (randomizeInitialValue)
            {
                float time = UnityEngine.Random.Range(0, maxWorkTime + maxSpareTime);
                if (time < maxWorkTime)
                {
                    timer = time;
                    isWorking = true;
                }
                else
                {
                    timer = time - maxWorkTime;
                    isWorking = false;
                }
            }
        }
        public void Init(IAIHost host)
        {
            worldState = host.WorldState;
            worldState.OnStateUpdate += OnUpdateState;
            worldState.SetState(States.CompleteWork, !isWorking);
        }
        private void OnUpdateState(string state, bool isOn)
        {
            if (state != States.IsWorking) return;
            isWorking = isOn;
            timer = 0;
        }
        public void Tick()
        {
            timer += Time.deltaTime;
            if (!isWorking) return;
            if (timer > (isWorking ? maxWorkTime : maxSpareTime))
            {
                timer = 0;
                worldState.SetState(States.CompleteWork, isWorking);
            }
        }
        public void Dispose()
        {
            worldState.OnStateUpdate -= OnUpdateState;
        }
    }
}