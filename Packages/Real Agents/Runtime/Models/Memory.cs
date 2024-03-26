using System;
using System.Collections.Generic;
namespace Kurisu.RealAgents
{
    public readonly struct MemoryMessage
    {
        public readonly string summary;
        public readonly string[] comments;
        public MemoryMessage(string summary, string[] comments)
        {
            this.summary = summary;
            this.comments = comments;
        }
    }
    [Serializable]
    public class ActionMemory
    {
        public string Name;
        /// <summary>
        /// Memory template
        /// </summary>
        /// <value></value>
        public string InitialImpression;
        /// <summary>
        /// Long term memory
        /// </summary>
        /// <value></value>
        public string Summary;
        /// <summary>
        /// Short term memory
        /// </summary>
        /// <value></value>
        public string[] Comments = new string[5] { "", "", "", "", "" };
        public ActionMemory() { }
        public ActionMemory(DescriptiveAction descriptiveAction)
        {
            Name = descriptiveAction.Name;
            Summary = InitialImpression = descriptiveAction.SelfDescription;
            for (int i = 0; i < 5; ++i)
            {
                Comments[i] = string.Empty;
            }
        }
        public MemoryMessage ToMessage()
        {
            return new MemoryMessage(Summary, Comments);
        }
        public void AddComment(string comment)
        {
            for (int i = 4; i > 0; --i)
            {
                Comments[i] = Comments[i - 1];
            }
            Comments[0] = comment;
        }
        public void Overwrite(string summary)
        {
            for (int i = 0; i < 5; ++i)
            {
                Comments[i] = string.Empty;
            }
            Summary = summary;
        }
        public bool CanReflect()
        {
            return !string.IsNullOrEmpty(Comments[4]);
        }
    }
    public class AgentMemory
    {
        public List<ActionMemory> actionMemories = new();
        public AgentMemory() { }
        public AgentMemory(IEnumerable<DescriptiveAction> actions)
        {
            foreach (var action in actions)
            {
                actionMemories.Add(new ActionMemory(action));
            }
        }
        public void Append(IEnumerable<DescriptiveAction> actions)
        {
            foreach (var action in actions)
            {
                if (!TryGetActionMemory(action.Name, out _))
                    actionMemories.Add(new ActionMemory(action));
            }
        }
        public bool TryGetActionMemory(string actionName, out ActionMemory actionMemory)
        {
            foreach (var memory in actionMemories)
            {
                if (memory.Name == actionName)
                {
                    actionMemory = memory;
                    return true;
                }
            }
            actionMemory = null;
            return false;
        }
    }
}
