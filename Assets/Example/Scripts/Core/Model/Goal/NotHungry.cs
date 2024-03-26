namespace Kurisu.RealAgents.Example
{
    public class NotHungry : DescriptiveGoal
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.IsHungry] = true;
            Conditions[States.IsHungry] = false;
        }
        protected override void OnSetup()
        {
            Host.AddTask(new FoodObserveTask());
            Host.AddTask(new HungerObserveTask());
        }
        protected sealed override float OnSetupPriority()
        {
            return 10f;
        }
    }
}
