using Kurisu.AkiAI;
using Kurisu.AkiBT;
namespace Kurisu.RealAgents.Example
{
    public class Drink : DescriptiveTask
    {
        private SequenceTask sequence;
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasWater] = true;
            Preconditions[States.IsThirsty] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.IsThirsty] = false;
        }
        protected override void OnActivateDerived()
        {
            Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
            Host.TContext.Animator.CrossFade("Drink", 0.2f);
            sequence = new SequenceTask(ClearThirst);
            sequence.Append(new WaitTask(2))
                    .Run();
        }
        private void ClearThirst()
        {
            var water = Host.BlackBoard.GetSharedVariable<int>(Variables.Water);
            --water.Value;
            Host.BlackBoard.SetInt(Variables.Thirst, 0);
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
