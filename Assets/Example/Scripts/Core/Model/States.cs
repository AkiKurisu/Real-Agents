using Kurisu.AkiAI;
namespace Kurisu.RealAgents.Example
{
    [TaskIDHost]
    public class Tasks
    {
        #region  Behavior Tasks
        public const string FollowFrontOfTarget = "FollowFrontOfTarget";
        public const string FollowTarget = "FollowTarget";
        public const string PickVegetable = "PickVegetable";
        public const string TourForest = "TourForest";
        #endregion
        #region  Hidden Tasks
        internal const string DistanceObserve = "DistanceObserve";
        #endregion
    }
    public class States
    {
        [Desc("When this state is true means you have energy")]
        public const string HasEnergy = "HasEnergy";
        [Desc("When this state is true means you are nearby the Target")]
        public const string InDistance = "InDistance";
        [Desc("When this state is true means you are in idle")]
        public const string Idle = "Idle";
        [Desc("When this state is true means you are very happy")]
        public const string IsHappy = "IsHappy";
        [Desc("When this state is true means Target is not null and Target's career is Dancer")]
        public const string TargetIsDancer = "TargetIsDancer";
        [Desc("When this state is true means Target is hungry")]
        public const string TargetIsHungry = "TargetIsHungry";
        [Desc("When this state is true means Target is working")]
        public const string TargetIsWorking = "TargetIsWorking";
        [Desc("When this state is true means you are thirsty")]
        public const string IsThirsty = "IsThirsty";
        [Desc("When this state is true means you are hungry")]
        public const string IsHungry = "IsHungry";
        [Desc("When this state is true means you have at least one food, food can make you not be hungry")]
        public const string HasFood = "HasFood";
        [Desc("When this state is true means you have at least one cup of water, water can make you not be thirsty")]
        public const string HasWater = "HasWater";
        [Desc("When this state is true means you are working")]
        public const string IsWorking = "IsWorking";
        [Desc("When this state is true means you have completed your work today")]
        public const string CompleteWork = "CompleteWork";
        [Desc("When this state is true means you have at least one ingredient, ingredient is used to make food")]
        public const string HasIngredients = "HasIngredients";
        [Desc("When this state is true means Merchant is working and you can buy ingredient from him")]
        public const string MerchantAtWork = "MerchantAtWork";
        [Desc("When this state is true means Dancer is working and you can watch dance")]
        public const string DancerAtWork = "DancerAtWork";
    }
    public class Variables
    {
        #region Global
        public const string Forest = "Forest";
        public const string Market = "Market";
        public const string Kitchen = "Kitchen";
        public const string Center = "Center";
        public const string Well = "Well";
        public const string Home = "Home";
        public const string HomeEntrance = "HomeEntrance";
        // public const string Tent = "Tent";
        // public const string TentEntrance = "TentEntrance";
        public const string Target = "Target";
        #endregion
        public const string Food = "Food";
        public const string Water = "Water";
        public const string Ingredient = "Ingredient";
        public const string Energy = "Energy";
        public const string Hunger = "Hunger";
        public const string Thirst = "Thirst";
    }
}