using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class HasEnergy : DescriptiveGoal
    {
        [SerializeField]
        private float energyLossTickTime = 0.5f;
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasEnergy] = false;
            Conditions[States.HasEnergy] = true;
        }
        protected override void OnSetup()
        {
            Host.AddTask(new EnergyObserveTask(energyLossTickTime));
        }
        protected sealed override float OnSetupPriority()
        {
            return 10f;
        }
    }
}
