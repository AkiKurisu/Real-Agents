namespace Kurisu.RealAgents.Example
{
    #region Two kinds of `idle` action
    public class Idle : DescriptiveAction
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.Idle] = false;
            Preconditions[States.InDistance] = true;
            Preconditions[States.HasEnergy] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.Idle] = true;
        }
        public sealed override float GetCost()
        {
            return 1;
        }
        protected override void OnActivateDerived()
        {
            //Equal to stop
            Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
        }
    }
    public class IdleFrontOfTarget : DescriptiveAction
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.TargetIsWorking] = true;
            Preconditions[States.Idle] = false;
            Preconditions[States.InDistance] = true;
            Preconditions[States.HasEnergy] = true;
        }
        protected override void OnSetup()
        {
            Host.AddTask(new TargetAtWorkObserveTask());
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.Idle] = true;
        }
        public sealed override float GetCost()
        {
            return 0;
        }
        protected override void OnActivateDerived()
        {
            Host.GetTask(Tasks.DistanceObserve).Stop();
            Host.GetTask(Tasks.FollowFrontOfTarget).Start();
        }
        protected override void OnDeactivateDerived()
        {
            Host.GetTask(Tasks.DistanceObserve).Start();
            Host.GetTask(Tasks.FollowFrontOfTarget).Stop();
        }
    }
    #endregion
}
