using Kurisu.AkiAI;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class FetchWater : DescriptiveTask
    {
        [SerializeField]
        private float fetchTime = 5;
        private Transform well;
        private SequenceTask sequence;
        protected override void OnSetup()
        {
            well = GlobalVariables.Instance.GetObject<Transform>(Variables.Well);
        }
        protected sealed override void SetupDerived()
        {
            worldState.RegisterNodeTarget(this, well);
            Preconditions[States.HasWater] = false;
            Preconditions[States.HasEnergy] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.HasWater] = true;
        }
        protected override void OnActivateDerived()
        {
            sequence = new SequenceTask(GetWater);
            sequence.Append(new MoveTask(Host.TContext.NavMeshAgent, well))
                    .AppendCallBack(() =>
                    {
                        Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
                        Host.TContext.NavMeshAgent.isStopped = true;
                        Host.TContext.Animator.CrossFade("Fetch", 0.2f);
                    })
                    .Append(new WaitTask(fetchTime))
                    .Run();
        }
        private void GetWater()
        {
            Host.BlackBoard.GetSharedVariable<int>(Variables.Water).Value = 5;
            worldState.SetState(States.HasWater, true);
            CompleteTask();
        }
        protected override void OnDeactivateDerived()
        {
            sequence?.Abort();
            sequence = null;
            Host.TContext.NavMeshAgent.isStopped = false;
            Host.TContext.Animator.CrossFade("Idle", 0.2f);
        }
        public override float GetCost()
        {
            return 5;
        }
    }
}
