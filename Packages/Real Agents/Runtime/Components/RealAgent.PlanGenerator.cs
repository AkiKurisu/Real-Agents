using System.Threading;
using Cysharp.Threading.Tasks;
using Kurisu.UniChat.Chains;
using Kurisu.UniChat.LLMs;
using Newtonsoft.Json;
namespace Kurisu.RealAgents
{
    public partial class RealAgent
    {
        #region  Plan Generation
        /// <summary>
        /// Call gpt to generate plan
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> GeneratePlan(InferenceContext evaluateContext)
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
            private readonly OpenAIClient openAIClient;
            public PlanGenerator(RealAgent realAgent, OpenAIClient openAIClient)
            {
                this.realAgent = realAgent;
                this.openAIClient = openAIClient;
                openAIClient.Temperature = 0.2f;
                openAIClient.Top_p = 0.1f;
                generatePlanTemplate = new("Generate_Plan");
            }
            public async UniTask<string> Generate(InferenceContext evaluateContext, CancellationToken ct)
            {
                string input = generatePlanTemplate.Get(new()
                    {
                        {"Actions",realAgent.GetActionsMemoryMessage()},
                        {"States",evaluateContext.StateCache.ToDictionary()},
                        {"Goal",realAgent.Goal}
                    });
                var chain = Chain.Set(input) | Chain.LLM(openAIClient);
                string result = await chain.Trace(true, true).Run("text");
                result = FindArray(result);
                return result;
            }
            private string FindArray(string input)
            {
                int startIndex = input.IndexOf('[');
                int endIndex = input.LastIndexOf(']') + 1;
                if (startIndex >= 0 && endIndex > 0 && endIndex > startIndex)
                    return input[startIndex..endIndex];
                return input;
            }
        }
    }
}