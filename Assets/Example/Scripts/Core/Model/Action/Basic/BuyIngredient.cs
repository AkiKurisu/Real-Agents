using Kurisu.AkiAI;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class BuyIngredient : DescriptiveTask
    {
        [SerializeField]
        private int buyCount = 5;
        private Transform market;
        private SequenceTask sequence;
        private SharedVariable<int> ingredient;
        protected sealed override void OnSetup()
        {
            market = GlobalVariables.Instance.GetObject<Transform>(Variables.Market);
            ingredient = Host.BlackBoard.GetInt(Variables.Ingredient);
        }
        protected sealed override void SetupDerived()
        {
            Preconditions[States.MerchantAtWork] = true;
            Preconditions[States.HasIngredients] = false;
            worldState.RegisterNodeTarget(this, market);
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.HasIngredients] = true;
        }
        protected sealed override void OnActivateDerived()
        {
            Host.TContext.NavMeshAgent.enabled = true;
            Host.TContext.NavMeshAgent.isStopped = false;
            sequence = new SequenceTask(GetIngredient);
            sequence.Append(new MoveTask(Host.TContext.NavMeshAgent, market))
                    .AppendCallBack(() =>
                    {
                        Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
                        Host.TContext.NavMeshAgent.isStopped = true;
                        Host.TContext.Animator.CrossFade("Talk", 0.2f);
                    })
                    .Append(new WaitTask(3))
                    .Run();
        }
        private void GetIngredient()
        {
            Host.BlackBoard.SetInt(Variables.Ingredient, ingredient.Value + buyCount);
            CompleteTask();
        }
        protected sealed override void OnDeactivateDerived()
        {
            sequence?.Abort();
            sequence = null;
            Host.TContext.NavMeshAgent.isStopped = false;
            Host.TContext.Animator.CrossFade("Idle", 0.2f);
        }
        public sealed override float GetCost()
        {
            return 10;
        }
    }
}
