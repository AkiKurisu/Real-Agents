using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
namespace Kurisu.RealAgents
{
    /// <summary>
    /// Styles from Generative Agents (https://github.com/joonspk-research/generative_agents)
    /// </summary>
    public class Template
    {
        private readonly string templateText;
        public Template(string path)
        {
            string externalPath = Path.Combine(PathUtil.TemplatePath, path);
            if (File.Exists(externalPath))
            {
                templateText = File.ReadAllText(externalPath);
            }
            else
            {
                templateText = Resources.Load<TextAsset>($"Real Agents/Template/{path}").text;
            }
        }
        public string Get(Dictionary<string, object> inputs)
        {
            string output = templateText;
            foreach (var pair in inputs)
            {
                if (pair.Value is not string)
                {
                    output = output.Replace($"!<{pair.Key}>!", JsonConvert.SerializeObject(pair.Value));
                }
                else
                {
                    output = output.Replace($"!<{pair.Key}>!", (string)pair.Value);
                }
            }
            return output;
        }
        public string Get()
        {
            return templateText;
        }
    }
    public class GenerateActionSummaryTemplate : Template
    {
        public GenerateActionSummaryTemplate() : base("Generate_Action_Summary") { }
        public string Get(string name, Dictionary<string, bool> conditions, Dictionary<string, bool> effects)
        {
            var dict = new Dictionary<string, object>
            {
                { "Name", name },
                { "Conditions", conditions },
                { "Effects", effects }
            };
            return Get(dict);
        }
    }
    public class GenerateGoalSummaryTemplate : Template
    {
        public GenerateGoalSummaryTemplate() : base("Generate_Goal_Summary") { }
        public string Get(string actions, Dictionary<string, bool> conditions)
        {
            var dict = new Dictionary<string, object>
            {
                { "Actions", actions },

                { "Conditions", conditions }
            };
            return Get(dict);
        }
    }
}