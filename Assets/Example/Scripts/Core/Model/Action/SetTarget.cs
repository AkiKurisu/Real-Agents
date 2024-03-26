using Kurisu.AkiBT;
namespace Kurisu.RealAgents.Example
{
    #region Set move target
    public class SetDancerAsTarget : DescriptiveTask
    {
        protected override void SetupDerived()
        {
            Preconditions[States.TargetIsDancer] = false;
        }
        protected override void OnActivateDerived()
        {
            if (CharaManager.Instance.TryGetChara(Career.Dancer, out var define))
                Host.BlackBoard.GetSharedObject(Variables.Target).SetValue(define.transform);
            worldState.SetState(States.TargetIsDancer, true);
            CompleteTask();
        }
        protected override void SetupEffects()
        {
            Effects[States.TargetIsDancer] = true;
        }
    }
    #endregion
}