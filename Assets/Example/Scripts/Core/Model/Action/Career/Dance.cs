using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    [GOAPGroup("Career")]
    public class Dance : DescriptiveAction
    {
        private Transform center;
        private SequenceTask sequence;
        protected override void OnSetup()
        {
            center = GlobalVariables.Instance.GetObject<Transform>(Variables.Center);
        }
        protected override void SetupDerived()
        {
            Preconditions[States.HasEnergy] = true;
            worldState.RegisterNodeTarget(this, center);
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.CompleteWork] = true;
        }
        protected override void OnActivateDerived()
        {
            sequence = new SequenceTask(StartDance);
            sequence.Append(new MoveTask(Host.TContext.NavMeshAgent, center))
                    .AppendCallBack(() => Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position))
                    .Run();
        }
        private void StartDance()
        {
            worldState.SetState(States.DancerAtWork, true, true);
            worldState.SetState(States.IsWorking, true);
            Host.TContext.NavMeshAgent.isStopped = true;
            Host.TContext.Animator.CrossFade("Dance", 0.2f);
        }
        protected override void OnDeactivateDerived()
        {
            worldState.SetState(States.DancerAtWork, false, true);
            worldState.SetState(States.IsWorking, false);
            sequence?.Abort();
            sequence = null;
            Host.TContext.NavMeshAgent.isStopped = false;
            Host.TContext.Animator.Play("Idle");
        }
    }
}
