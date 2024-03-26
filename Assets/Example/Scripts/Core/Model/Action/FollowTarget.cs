namespace Kurisu.RealAgents.Example
{
    public class FollowTarget : DescriptiveTask
    {
        protected override void OnSetup()
        {
            Host.AddTask(new DistanceObserveTask(2));
        }
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasEnergy] = true;
            Preconditions[States.InDistance] = false;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.InDistance] = true;
        }
        protected override void OnActivateDerived()
        {
            Host.GetTask(Tasks.DistanceObserve).Stop();
            //Use behavior tree to observe distance
            Host.GetTask(Tasks.FollowTarget).Start();
        }
        public override void OnTick()
        {
            if (Flag == TaskFlag.Running)
            {
                if (worldState.InSet(States.InDistance, true))
                {
                    CompleteTask();
                }
            }
        }
        protected override void OnDeactivateDerived()
        {
            //Restore observe task
            Host.GetTask(Tasks.DistanceObserve).Start();
            Host.GetTask(Tasks.FollowTarget).Stop();
        }
    }
}
