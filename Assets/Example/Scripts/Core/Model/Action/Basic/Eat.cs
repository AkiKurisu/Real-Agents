using Kurisu.AkiAI;
using Kurisu.AkiBT;
namespace Kurisu.RealAgents.Example
{
    public class Eat : DescriptiveTask
    {
        private SequenceTask sequence;
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasFood] = true;
            Preconditions[States.IsHungry] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.IsHungry] = false;
        }
        protected override void OnActivateDerived()
        {
            Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
            Host.TContext.Animator.CrossFade("Eat", 0.2f);
            sequence = new SequenceTask(ClearHunger);
            sequence.Append(new WaitTask(2))
                    .Run();
        }
        private void ClearHunger()
        {
            var food = Host.BlackBoard.GetSharedVariable<int>(Variables.Food);
            --food.Value;
            Host.BlackBoard.SetInt(Variables.Hunger, 0);
            CompleteTask();
        }
        protected override void OnDeactivateDerived()
        {
            sequence?.Abort();
            sequence = null;
        }
        public override float GetCost()
        {
            return 5;
        }
    }
}
