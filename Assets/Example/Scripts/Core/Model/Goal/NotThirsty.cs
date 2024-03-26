namespace Kurisu.RealAgents.Example
{
    public class NotThirsty : DescriptiveGoal
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.IsThirsty] = true;
            Conditions[States.IsThirsty] = false;
        }
        protected override void OnSetup()
        {
            Host.AddTask(new WaterObserveTask());
            Host.AddTask(new ThirstyObserveTask());
        }
        protected sealed override float OnSetupPriority()
        {
            return 10f;
        }
    }
}
