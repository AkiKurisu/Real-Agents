using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Kurisu.AkiAI;
using Kurisu.GOAP;
using Kurisu.UniChat;
using Newtonsoft.Json;
using UnityEngine;
namespace Kurisu.RealAgents
{
    public abstract class DescriptiveGoal : AIGoal<IRealAgentContext>, ISelfDescriptive
    {
        public string SelfDescription { set => selfDescription = value; get => selfDescription; }
        [SerializeField, Multiline]
        private string selfDescription;
        public static void CleanUp()
        {
            template = null;
        }
        private static GenerateGoalSummaryTemplate template;
        public async UniTask DoSelfDescription(string actions, ILargeLanguageModel llm, CancellationToken ct)
        {
            template ??= new();
            selfDescription = (await llm.GenerateAsync(template.Get(actions, Conditions), ct)).Response;
        }

        public void VirtualInitialize(WorldState worldState)
        {
            this.worldState = worldState;
            SetupDerived();
        }
        protected sealed override float SetupPriority()
        {
            if (Host.TContext.PriorityLookup.TryGetPriority(Name, out float priority))
            {
                return OnSetupPriority() + priority;
            }
            return OnSetupPriority();
        }
        protected virtual float OnSetupPriority()
        {
            return 0;
        }
        public void Write(StreamWriter sw)
        {
            sw.Write("Name:");
            sw.WriteLine(Name);
            sw.WriteLine("Type:Goal");
            sw.Write("Preconditions:");
            sw.Write("Description:");
            sw.WriteLine(SelfDescription);
            sw.WriteLine(JsonConvert.SerializeObject(Preconditions));
            sw.Write("Conditions:");
            sw.WriteLine(JsonConvert.SerializeObject(Conditions));
        }
        #region   Since generative agent cannot access to goal, we should not modify states in goal
        public sealed override void OnActivate() { }
        public sealed override void OnDeactivate() { }
        public sealed override void OnTick() { }
        #endregion
    }
}
