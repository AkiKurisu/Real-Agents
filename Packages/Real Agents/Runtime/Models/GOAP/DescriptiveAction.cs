using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Kurisu.AkiAI;
using Kurisu.GOAP;
using Kurisu.UniChat;
using Newtonsoft.Json;
using UnityEngine;
using TaskStatus = Kurisu.AkiAI.TaskStatus;
namespace Kurisu.RealAgents
{
    public abstract class DescriptiveAction : AIAction<IRealAgentContext>, ISelfDescriptive
    {
        public string SelfDescription { set => selfDescription = value; get => selfDescription; }
        [SerializeField, Multiline]
        private string selfDescription;
        private static GenerateActionSummaryTemplate template;
        public static void CleanUp()
        {
            template = null;
        }
        public async UniTask DoSelfDescription(ILargeLanguageModel llm, CancellationToken ct)
        {
            template ??= new();
            selfDescription = (await llm.GenerateAsync(template.Get(Name, Preconditions, Effects), ct)).Response;
        }
        public string GetConstraints()
        {
            return JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { "Conditions", Preconditions },
                { "Effects", Effects }
            });
        }

        public void VirtualInitialize(WorldState worldState)
        {
            this.worldState = worldState;
            SetupDerived();
            SetupEffects();
        }
        public void Write(StreamWriter sw)
        {
            sw.Write("Name:");
            sw.WriteLine(Name);
            sw.WriteLine("Type:Action");
            sw.Write("Description:");
            sw.WriteLine(SelfDescription);
            sw.Write("Conditions:");
            sw.WriteLine(JsonConvert.SerializeObject(Preconditions));
            sw.Write("Effects:");
            sw.WriteLine(JsonConvert.SerializeObject(Effects));
        }
    }
    /// <summary>
    /// DescriptiveAction can be used as task
    /// </summary>
    public abstract class DescriptiveTask : DescriptiveAction, ITask
    {
        protected enum TaskFlag
        {
            Running, Ending, Done
        }
        protected TaskFlag Flag { get; set; } = TaskFlag.Done;
        public TaskStatus Status
        {
            get
            {
                if (Flag == TaskFlag.Running || Flag == TaskFlag.Ending) return TaskStatus.Enabled;
                return TaskStatus.Disabled;
            }
        }
        /// <summary>
        /// Implement to use task-based scope
        /// </summary>
        public virtual void Tick()
        {
            if (Flag == TaskFlag.Running)
            {
                OnTick();
            }
            else if (Flag == TaskFlag.Ending)
            {
                OnDeactivate();
                Flag = TaskFlag.Done;
            }
        }
        public void CompleteTask()
        {
            Flag = TaskFlag.Ending;
        }
        public void InitTask()
        {
            Flag = TaskFlag.Running;
            OnActivate();
        }
    }
}