using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public abstract class RestAtPlace : DescriptiveTask
    {
        [SerializeField]
        private float waitTime = 5;
        private SequenceTask sequence;
        protected Transform RestEntrance { get; set; }
        protected Transform RestPoint { get; set; }
        private bool inRest;
        protected sealed override void SetupEffects()
        {
            Effects[States.HasEnergy] = true;
        }
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasEnergy] = false;
            worldState.RegisterNodeTarget(this, RestEntrance);
        }
        protected sealed override void OnActivateDerived()
        {
            sequence = new SequenceTask(RestoreEnergy);
            sequence.Append(new MoveTask(Host.TContext.NavMeshAgent, RestEntrance))
                    .AppendCallBack(() =>
                    {
                        inRest = true;
                        Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
                        Host.TContext.NavMeshAgent.enabled = false;
                        Host.Transform.position = RestPoint.position;
                        Host.TContext.Animator.Play("Rest");
                    })
                    .Append(new WaitTask(waitTime))
                    .Run();
        }
        private void RestoreEnergy()
        {
            Host.BlackBoard.SetInt(Variables.Energy, 100);
            worldState.SetState(States.HasEnergy, true);
            CompleteTask();
        }
        protected sealed override void OnDeactivateDerived()
        {
            sequence?.Abort();
            sequence = null;
            if (inRest)
            {
                inRest = false;
                Host.TContext.Animator.Play("Idle");
                Host.Transform.position = RestEntrance.position;
                Host.TContext.NavMeshAgent.enabled = true;
            }
        }
        public sealed override float GetCost()
        {
            return 10;
        }
    }
    [GOAPGroup("Rest")]
    public class RestAtHome : RestAtPlace
    {
        protected override void OnSetup()
        {
            //Agent scope variables
            RestPoint = Host.BlackBoard.GetObject<Transform>(Variables.Home);
            RestEntrance = Host.BlackBoard.GetObject<Transform>(Variables.HomeEntrance);
        }
    }
    // [GOAPGroup("Rest")]
    // public class RestAtTent : RestAtPlace
    // {
    //     protected override void OnSetup()
    //     {
    //         RestPoint = Host.BlackBoard.GetObject<Transform>(Variables.Tent);
    //         RestEntrance = Host.BlackBoard.GetObject<Transform>(Variables.TentEntrance);
    //     }
    // }
}
