using System.Collections.Generic;
using Kurisu.Framework;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    public class Config
    {
        public string baseUrl = "https://api.openai-proxy.com/v1/chat/completions";
        public string apiKey;
        private readonly Dictionary<string, string> agentPathMap = new();
        public bool TryGetPath(string agentID, out string path)
        {
            if (agentPathMap == null)
            {
                path = null;
                return false;
            }
            return agentPathMap.TryGetValue(agentID, out path);
        }
        public void SetPath(string agentID, string path)
        {
            agentPathMap[agentID] = path;
        }
    }
    public class ConfigEntry
    {
        public static Config Instance { get; private set; }

        public static void SaveConfig()
        {
            SaveUtility.Save(Instance);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadConfig()
        {
            Instance = SaveUtility.LoadOrNew<Config>();
        }
    }
}
