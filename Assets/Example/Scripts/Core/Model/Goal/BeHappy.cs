namespace Kurisu.RealAgents.Example
{
    public class BeHappy : DescriptiveGoal
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.CompleteWork] = true;
            Preconditions[States.HasEnergy] = true;
            Preconditions[States.IsHungry] = false;
            Preconditions[States.IsThirsty] = false;
            Conditions[States.IsHappy] = true;
        }
        protected sealed override float OnSetupPriority()
        {
            return 1f;
        }
    }
}