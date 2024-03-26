using Kurisu.AkiAI;
using Kurisu.AkiBT;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class Cook : DescriptiveTask
    {
        [SerializeField]
        private float cookTime = 5;
        private Transform kitchen;
        private SequenceTask sequence;
        protected override void OnSetup()
        {
            kitchen = GlobalVariables.Instance.GetObject<Transform>(Variables.Kitchen);
        }
        protected sealed override void SetupDerived()
        {
            worldState.RegisterNodeTarget(this, kitchen);
            Preconditions[States.HasFood] = false;
            Preconditions[States.HasIngredients] = true;
        }
        protected sealed override void SetupEffects()
        {
            Effects[States.HasFood] = true;
        }
        protected override void OnActivateDerived()
        {
            sequence = new SequenceTask(GetFood);
            sequence.Append(new MoveTask(Host.TContext.NavMeshAgent, kitchen))
                    .AppendCallBack(() =>
                    {
                        Host.TContext.NavMeshAgent.SetDestination(Host.Transform.position);
                        Host.TContext.NavMeshAgent.isStopped = true;
                        Host.TContext.Animator.CrossFade("Cook", 0.2f);
                    })
                    .Append(new WaitTask(cookTime))
                    .Run();
        }
        private void GetFood()
        {
            var food = Host.BlackBoard.GetSharedVariable<int>(Variables.Food);
            var ingredient = Host.BlackBoard.GetSharedVariable<int>(Variables.Ingredient);
            food.Value = Mathf.Min(5, ingredient.Value);
            ingredient.Value -= food.Value;
            CompleteTask();
        }
        protected override void OnDeactivateDerived()
        {
            Host.TContext.Animator.Play("Idle");
            sequence?.Abort();
            sequence = null;
            Host.TContext.NavMeshAgent.isStopped = false;
        }
        public override float GetCost()
        {
            return 5;
        }
    }
}
