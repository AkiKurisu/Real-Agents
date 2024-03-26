using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kurisu.NGDS.AI;
using Newtonsoft.Json;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public partial class RealAgent
    {
        #region  Plan Generation
        /// <summary>
        /// Call gpt to generate plan
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GeneratePlan(InferenceContext evaluateContext)
        {
            string result = await planGenerator.Generate(evaluateContext, ct.Token);
            try
            {
                Log($"Get generated plan: {result}");
                evaluateContext.optimalPlanStream = result.Trim().Replace("\n", string.Empty);
                evaluateContext.optimalPlan = JsonConvert.DeserializeObject<string[]>(evaluateContext.optimalPlanStream);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        private class PlanGenerator
        {
            private readonly RealAgent realAgent;
            private readonly Template generatePlanTemplate;
            private readonly string[] historyBuffer = new string[2];
            private readonly GPTAgent gptAgent;
            private bool firstEmbedding = true;
            private readonly LangChainAgent langChainAgent;
            public PlanGenerator(RealAgent realAgent, GPTAgent gptAgent, LangChainAgent langChainAgent)
            {
                this.realAgent = realAgent;
                this.gptAgent = gptAgent;
                gptAgent.Temperature = 0.2f;
                gptAgent.Top_p = 0.1f;
                this.langChainAgent = langChainAgent;
                generatePlanTemplate = new("Generate_Plan");
            }
            public async Task<string> Generate(InferenceContext evaluateContext, CancellationToken ct)
            {
                if (realAgent.PlanGeneratorMode == PlanGeneratorMode.Embedding)
                {
                    if (firstEmbedding)
                    {
                        //Force initialize database
                        await realAgent.PersistEmbedding();
                        firstEmbedding = false;
                    }
                    string input = generatePlanTemplate.Get(new()
                    {
                        //Decrease token when use vector embedding
                        {"Actions",realAgent.actions.Select(x=>x.Name).ToArray()},
                        {"States",evaluateContext.StateCache.ToDictionary()},
                        {"Goal",realAgent.Goal}
                    });
                    return await langChainAgent.Query(input, realAgent.Guid, ct);
                }
                else
                {
                    string input = generatePlanTemplate.Get(new()
                    {
                        {"Actions",realAgent.GetActionsMemoryMessage()},
                        {"States",evaluateContext.StateCache.ToDictionary()},
                        {"Goal",realAgent.Goal}
                    });
                    string result;
                    if (realAgent.AIGCMode == AIGCMode.Training)
                    {
                        result = await gptAgent.Inference(input, ct);
                    }
                    else
                    {
                        gptAgent.ClearHistory();
                        gptAgent.Append(historyBuffer[0], historyBuffer[1]);
                        result = await gptAgent.Continue(input, ct);
                    }
                    result = FindArray(result);
                    Debug.Log(result);
                    SetHistory(input, result);
                    return result;
                }
            }
            private string FindArray(string input)
            {
                int startIndex = input.IndexOf('[');
                int endIndex = input.LastIndexOf(']') + 1;
                if (startIndex >= 0 && endIndex > 0 && endIndex > startIndex)
                    return input[startIndex..endIndex];
                return input;
            }
            public void SetHistory(string input, string result)
            {
                historyBuffer[0] = input;
                historyBuffer[1] = result;
            }
        }
    }
}