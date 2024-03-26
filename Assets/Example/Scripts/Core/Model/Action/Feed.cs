using Kurisu.AkiAI;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    /// <summary>
    /// This action can only be called by generative agent
    /// </summary>
    public class Feed : DescriptiveTask
    {
        protected override void OnSetup()
        {
            Host.AddTask(new TargetStarveObserveTask());
        }
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasEnergy] = true;
            Preconditions[States.InDistance] = true;
            Preconditions[States.HasFood] = true;
            Preconditions[States.TargetIsHungry] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.TargetIsHungry] = false;
        }
        protected override void OnActivateDerived()
        {
            //Equal to stopping
            Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
            var food = Host.BlackBoard.GetSharedVariable<int>(Variables.Food);
            --food.Value;
            Host.BlackBoard.GetObject<Transform>(Variables.Target).GetComponent<IAIBlackBoard>().SetInt(Variables.Hunger, 0);
            CompleteTask();
        }
        public override float GetCost()
        {
            return 5;
        }
    }
}
