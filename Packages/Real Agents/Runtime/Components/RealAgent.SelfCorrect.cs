using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public partial class RealAgent
    {
        private Template commentTemplate;
        private Template reasonTemplate;
        private Template reflectTemplate;
        private void InitializeTemplates()
        {
            reasonTemplate = new("Generate_Action_SelfCorrect_Reason");
            commentTemplate = new("Generate_Action_SelfCorrect_Comment");
            reflectTemplate = new("Generate_Action_SelfCorrect_Reflect");
        }
        #region  Plan Evaluation
        private async UniTask<bool> EvaluatePlan(InferenceContext evaluateContext)
        {
            for (int i = 0; i < Mathf.Min(evaluateContext.ProceduralPlan.Count, evaluateContext.Tasks.Count); ++i)
            {
                if (evaluateContext.ProceduralPlan[i].Name == evaluateContext.Tasks[i].Action.Name) continue;
                Log($"Generated plan is not as same as procedural plan.");
                //Reasoning chain
                await GenerateComment(await GenerateReason(i, evaluateContext), evaluateContext.Tasks[i].Action.Name, evaluateContext);
                return false;
            }
            LogSuccess($"Generated plan is as same as procedural plan!");
            //Save history
            OnGeneratePlan?.Invoke(Goal, evaluateContext);
            return true;
        }
        #endregion
        #region  Precondition Evaluation
        private async UniTask<bool> EvaluatePrecondition(InferenceContext evaluateContext)
        {
            var action = evaluateContext.Tasks[0].Action;
            if (!action.PreconditionsSatisfied(evaluateContext.StateCache))
            {
                LogWarning($"{action.Name}'s preconditions are not satisfied.");
                //Generate a procedural reason to teach agent why action can not use
                StringBuilder sb = new($"{action.Name}'s preconditions are not fully satisfied,");
                foreach (var precondition in action.Preconditions)
                {
                    if (evaluateContext.StateCache.InSet(precondition.Key, precondition.Value)) continue;
                    sb.Append($" ,precondition {precondition.Key} need to be {precondition.Value}");
                }
                sb.Append('.');
                await GenerateComment(sb.ToString(), action.Name, evaluateContext);
                stateStopWatch.StateChanged = true;
                return false;
            }
            return true;
        }
        #endregion
        /// <summary>
        /// Call gpt to generate a reason why planner select target action as comment's reason
        /// </summary>
        /// <param name="index"></param>
        /// <param name="evaluateContext"></param>
        /// <returns></returns>
        private async UniTask<string> GenerateReason(int index, InferenceContext evaluateContext)
        {
            LogStatus($"Generate a reason for while select {evaluateContext.ProceduralPlan[index].Name} instead of {evaluateContext.Tasks[index].Action.Name}");
            var response = await openAIClient.GenerateAsync(reasonTemplate.Get(new()
            {
                {"Actions",GetActionsMemoryMessage()},
                {"States",evaluateContext.StateCache.ToDictionary() },
                {"Goal",Goal},
                {"Plan",evaluateContext.ProceduralPlan.Select(x => x.Name).ToArray() },
                {"Index",index},
                {"Wrong",evaluateContext.ProceduralPlan[index].Name},
                {"Right",evaluateContext.Tasks[index].Action.Name}
            }), ct.Token);
            Log($"Get reason: {response.Response}");
            return response.Response;
        }
        /// <summary>
        /// Call gpt to generate a comment for an action as short-term self correct
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="actionName"></param>
        /// <param name="evaluateContext"></param>
        /// <returns></returns> <summary>
        private async UniTask GenerateComment(string reason, string actionName, InferenceContext evaluateContext)
        {
            LogStatus($"Generate a comment for {actionName}");
            if (!Memory.TryGetActionMemory(actionName, out var memory))
            {
                LogError($"Action {actionName} not existed in agent memory");
                return;
            }
            var result = await openAIClient.GenerateAsync(commentTemplate.Get(new()
            {
                {"Reason",reason },
                {"States",evaluateContext.StateCache.ToDictionary() },
                {"Goal",Goal},
                {"Action",actionName},
                {"Summary",memory.Summary},
                {"Comments",memory.Comments}
            }), ct.Token);
            memory.AddComment(result.Response);
            Log($"Get comment: {result.Response}");
            if (memory.CanReflect())
            {
                await GenerateReflect(memory);
            }
            //Temporarily call after generating comment which means dataBase should update
            SaveMemory();
        }
        /// <summary>
        /// Call gpt to reflect for long-term self correct
        /// </summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        private async UniTask GenerateReflect(ActionMemory memory)
        {
            var response = await openAIClient.GenerateAsync(reflectTemplate.Get(new()
            {
                {"Name",memory.Name},
                {"InitialImpression",memory.InitialImpression},
                {"Summary",memory.Summary},
                {"Comments",memory.Comments}
            }), ct.Token);
            memory.Overwrite(response.Response);
        }
    }
}