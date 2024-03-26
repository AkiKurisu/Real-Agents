using Kurisu.AkiAI;
using UnityEngine;
using Kurisu.AkiBT;
using Kurisu.GOAP;
namespace Kurisu.RealAgents.Example
{
    [GOAPGroup("Career")]
    public class SaleIngredient : DescriptiveAction
    {
        private Transform market;
        private SequenceTask sequence;
        protected sealed override void OnSetup()
        {
            market = GlobalVariables.Instance.GetObject<Transform>(Variables.Market);
        }
        protected sealed override void SetupDerived()
        {
            Preconditions[States.HasEnergy] = true;
            worldState.RegisterNodeTarget(this, market);
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.CompleteWork] = true;
        }
        protected sealed override void OnActivateDerived()
        {
            worldState.SetState(States.MerchantAtWork, true, true);
            worldState.SetState(States.IsWorking, true);
            Host.TContext.NavMeshAgent.enabled = true;
            Host.TContext.NavMeshAgent.isStopped = false;
            sequence = new SequenceTask();
            sequence.Append(new MoveTask(Host.TContext.NavMeshAgent, market))
                    .AppendCallBack(() =>
                    {
                        Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
                        Host.TContext.NavMeshAgent.enabled = false;
                        Host.Transform.SetPositionAndRotation(market.position, market.rotation);
                    })
                    .Run();
        }
        protected sealed override void OnDeactivateDerived()
        {
            worldState.SetState(States.MerchantAtWork, false, true);
            worldState.SetState(States.IsWorking, false);
            sequence?.Abort();
            sequence = null;
            Host.TContext.NavMeshAgent.enabled = true;
        }
        public sealed override float GetCost()
        {
            return 10;
        }
    }
}
