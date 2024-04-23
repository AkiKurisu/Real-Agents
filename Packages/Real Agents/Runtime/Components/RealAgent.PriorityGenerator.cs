using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Kurisu.UniChat.Chains;
using Kurisu.UniChat.LLMs;
using Newtonsoft.Json;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public partial class RealAgent
    {
        private class PriorityGenerator : IPriorityLookup
        {
            private readonly Template generatePriorityTemplate;
            private readonly RealAgent realAgent;
            private readonly OpenAIClient openAIClient;
            private Dictionary<string, float> priorityMap;
            public bool IsRunning { get; private set; }
            public PriorityGenerator(RealAgent realAgent, OpenAIClient openAIClient)
            {
                this.realAgent = realAgent;
                this.openAIClient = openAIClient;
                generatePriorityTemplate = new("Generate_Priority");
            }
            public async UniTask<bool> StartGenerate(CancellationToken ct)
            {
                //Block when method run
                if (IsRunning)
                {
                    return true;
                }
                string input = generatePriorityTemplate.Get(new()
                {
                    {"Goals",realAgent.GetGoalsDescription()},
                    {"Purpose",realAgent.Goal}
                });
                IsRunning = true;
                var chain = Chain.Set(input) | Chain.LLM(openAIClient);
                string result = await chain.Run("text");
                Debug.Log(result);
                try
                {
                    priorityMap = JsonConvert.DeserializeObject<Dictionary<string, float>>(result);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return false;
                }
                finally
                {
                    IsRunning = false;
                }
            }
            public bool TryGetPriority(string goalName, out float priority)
            {
                if (priorityMap == null)
                {
                    priority = 0;
                    return false;
                }
                return priorityMap.TryGetValue(goalName, out priority);
            }
        }
    }
}