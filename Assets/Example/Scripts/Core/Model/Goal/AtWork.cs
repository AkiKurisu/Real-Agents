namespace Kurisu.RealAgents.Example
{
    public class AtWork : DescriptiveGoal
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.CompleteWork] = false;
            Preconditions[States.HasEnergy] = true;
            Preconditions[States.IsHungry] = false;
            Preconditions[States.IsThirsty] = false;
            //Complete work was controlled by `WorkObserveTask`
            Conditions[States.CompleteWork] = true;
        }
        protected override void OnSetup()
        {
            Host.AddTask(new WorkObserveTask(maxWorkTime: 240, maxSpareTime: 120, randomizeInitialValue: true));
        }
        protected sealed override float OnSetupPriority()
        {
            return 5f;
        }
    }
}
