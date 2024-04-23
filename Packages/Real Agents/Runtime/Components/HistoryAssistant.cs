using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
namespace Kurisu.RealAgents
{
    /// <summary>
    /// Helper component to collect succeed generated plan for few-shot samples
    /// </summary>
    [RequireComponent(typeof(RealAgent))]
    public class HistoryAssistant : MonoBehaviour
    {
        private History history;
        private RealAgent realAgent;
        [field: SerializeField]
        public bool AllowOverwrite { get; set; }
        private void Awake()
        {
            realAgent = GetComponent<RealAgent>();
            realAgent.OnGeneratePlan += Append;
            LoadHistory();
        }
        private void OnDestroy()
        {
            realAgent.OnGeneratePlan -= Append;
        }
        public void Append(string input, RealAgent.InferenceContext inferenceContext)
        {
            IEnumerable<KeyValuePair<string, bool>> states = inferenceContext.StateCache;
            string result = inferenceContext.optimalPlanStream;
            if (!history.dataMap.TryGetValue(input, out var list))
            {
                list = new();
                history.dataMap.Add(input, list);
            }
            var min = list.FirstOrDefault(x => x.Score(states) == 0);
            if (min != null)
            {
                if (AllowOverwrite)
                {
                    min.plan = result;
                    SaveHistory();
                }
            }
            else
            {
                list.Add(new(states.ToDictionary(x => x.Key, x => x.Value), result));
                SaveHistory();
            }
        }
        private string GetHistoryPath() => Path.Combine(PathUtil.GetOrCreateAgentFolder(realAgent.Guid), "History.json");
        public void SaveHistory()
        {
            File.WriteAllText(GetHistoryPath(), JsonConvert.SerializeObject(history, Formatting.Indented));
        }
        public void LoadHistory()
        {
            string filePath = GetHistoryPath();
            if (!File.Exists(filePath))
            {
                history = new();
                return;
            }
            history = JsonConvert.DeserializeObject<History>(File.ReadAllText(filePath));
        }
    }
}
