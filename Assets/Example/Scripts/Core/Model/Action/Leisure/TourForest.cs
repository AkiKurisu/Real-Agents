using Kurisu.GOAP;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    [GOAPGroup("Leisure")]
    public class TourForest : DescriptiveAction
    {
        private Transform forest;
        protected sealed override void OnSetup()
        {
            forest = GlobalVariables.Instance.GetObject<Transform>(Variables.Forest);
        }
        protected sealed override void SetupDerived()
        {
            worldState.RegisterNodeTarget(this, forest);
        }
        protected sealed override void OnActivateDerived()
        {
            Host.GetTask(Tasks.TourForest).Start();
        }
        protected sealed override void OnDeactivateDerived()
        {
            Host.GetTask(Tasks.TourForest).Stop();
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.IsHappy] = true;
        }
    }
}