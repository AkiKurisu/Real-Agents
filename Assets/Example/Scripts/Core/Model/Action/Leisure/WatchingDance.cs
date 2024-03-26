using Kurisu.GOAP;
namespace Kurisu.RealAgents.Example
{
    #region  This action acts as secondary goal and will not actually be run
    [GOAPGroup("Leisure")]
    public class WatchingDance : DescriptiveAction
    {
        protected sealed override void SetupDerived()
        {
            Preconditions[States.DancerAtWork] = true;
            Preconditions[States.TargetIsDancer] = true;
            Preconditions[States.Idle] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.IsHappy] = true;
        }
    }
    #endregion
}