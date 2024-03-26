using System.Collections.Generic;
namespace Kurisu.RealAgents
{
    public class History
    {
        public string description = "This file contains plan examples based on input goal and states.";
        public Dictionary<string, List<Raw>> dataMap = new();
        public class Raw
        {
            public Dictionary<string, bool> states = new();
            public string plan;
            public int Score(IEnumerable<KeyValuePair<string, bool>> dictionary)
            {
                int score = 0;
                foreach (var pair in dictionary)
                {
                    if (!states.TryGetValue(pair.Key, out bool value))
                    {
                        if (!pair.Value)
                            score += 1;
                        else
                            score += 2;
                    }
                    else
                    {
                        if (value != pair.Value) score += 2;
                    }
                }
                return score;
            }
            public Raw() { }
            public Raw(Dictionary<string, bool> states, string plan)
            {
                this.states = states;
                this.plan = plan;
            }
        }
    }
}