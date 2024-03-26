using System;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public partial class RealAgent
    {
        private class StateStopWatch : IDisposable
        {
            private float lastChangeTime;
            public bool StateChanged { get; set; }
            private StateCache stateCache;
            private readonly WorldState worldState;
            public float MaxIgnoreTime { get; set; } = 30f;
            public StateStopWatch(WorldState worldState)
            {
                this.worldState = worldState;
                worldState.OnStateUpdate += OnStateUpdate;
            }
            public void Dispose()
            {
                worldState.OnStateUpdate -= OnStateUpdate;
            }
            private void OnStateUpdate(string arg1, bool arg2)
            {
                if (IsStateChanged())
                {
                    StateChanged = true;
                }
            }
            private bool IsStateChanged()
            {
                if (stateCache == null)
                {
                    stateCache = worldState.LocalState.GetCache();
                    return true;
                }
                stateCache.Pooled();
                stateCache = worldState.LocalState.GetCache();
                if (worldState.IsSubset(stateCache.ToDictionary()))
                {
                    //Force set dirty flag after double interval time
                    var current = Time.timeSinceLevelLoad;
                    if (current - lastChangeTime > MaxIgnoreTime)
                    {
                        lastChangeTime = current;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
    }
}