using Kurisu.AkiAI;
using Kurisu.AkiBT;
using Kurisu.GOAP;
namespace Kurisu.RealAgents.Example
{
    public class FoodObserveTask : StatusTask, IAITask
    {
        public string TaskID { get; } = nameof(FoodObserveTask);
        public bool IsPersistent => true;
        private WorldState worldState;
        private SharedVariable<int> food;
        private SharedVariable<int> ingredient;
        public void Init(IAIHost host)
        {
            worldState = host.WorldState;
            //Ingredient
            if (host.Object.GetComponent<CharaDefine>().AlwaysHaveIngredient)
            {
                worldState.SetState(States.HasIngredients, true);
                ingredient = host.BlackBoard.SetInt(Variables.Ingredient, int.MaxValue);
            }
            else
            {
                ingredient = host.BlackBoard.SetInt(Variables.Ingredient, 0);
                worldState.SetState(States.HasIngredients, false);
                ingredient.ObserveT().OnValueChange += (x) => worldState.SetState(States.HasIngredients, x > 0);
            }
            //Food
            if (host.Object.GetComponent<CharaDefine>().AlwaysHaveFood)
            {
                worldState.SetState(States.HasFood, true);
                food = host.BlackBoard.SetInt(Variables.Food, int.MaxValue);
            }
            else
            {
                food = host.BlackBoard.SetInt(Variables.Food, 0);
                worldState.SetState(States.HasFood, false);
                food.ObserveT().OnValueChange += (x) => worldState.SetState(States.HasFood, x > 0);
            }
        }
        public void Tick() { }
    }
}