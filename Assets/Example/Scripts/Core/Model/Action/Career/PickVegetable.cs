using Kurisu.GOAP;
namespace Kurisu.RealAgents.Example
{
    [GOAPGroup("Career")]
    public class PickVegetable : DescriptiveAction
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasEnergy] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.CompleteWork] = true;
        }
        protected sealed override void OnActivateDerived()
        {
            Host.GetTask(Tasks.PickVegetable).Start();
            worldState.SetState(States.IsWorking, true);
        }
        protected sealed override void OnDeactivateDerived()
        {
            Host.GetTask(Tasks.PickVegetable).Stop();
            worldState.SetState(States.IsWorking, false);
            Host.TContext.Animator.SetBool("IsPicking", false);
        }
        public sealed override float GetCost()
        {
            return 10;
        }
    }
}
