using Kurisu.AkiAI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
namespace Kurisu.RealAgents.Example
{
    public class MoveTask : ITask
    {
        private readonly Transform moveTarget;
        private readonly NavMeshAgent agent;
        public float StopSqrMagnitude { get; set; } = 1f;
        public TaskStatus Status { get; private set; } = TaskStatus.Enabled;
        public MoveTask(NavMeshAgent agent, Transform moveTarget)
        {
            this.agent = agent;
            this.moveTarget = moveTarget;
            Assert.IsNotNull(agent);
            Assert.IsNotNull(moveTarget);
        }
        public void Tick()
        {
            agent.SetDestination(moveTarget.position);
            if ((agent.transform.position - moveTarget.position).sqrMagnitude < StopSqrMagnitude)
            {
                Status = TaskStatus.Disabled;
            }
        }
    }
}
