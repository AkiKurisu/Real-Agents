using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kurisu.GOAP;
using Newtonsoft.Json;
using UnityEngine;
namespace Kurisu.RealAgents
{
    [CreateAssetMenu(fileName = "RealAgentSet", menuName = "Real Agents/RealAgentSet")]
    public class RealAgentSet : GOAPSet, ISelfDescriptiveSet
    {
        [SerializeField, Tooltip("Use description from sharedDataSet if possible")]
        private RealAgentSet sharedDataSet;
        public DescriptiveAction[] GetDescriptiveActions()
        {
            return Behaviors.OfType<DescriptiveAction>().ToArray();
        }
        public DescriptiveGoal[] GetDescriptiveGoals()
        {
            return Behaviors.OfType<DescriptiveGoal>().ToArray();
        }
        public async Task DoSelfDescription(IGPTService service, CancellationToken ct)
        {
            DescriptiveAction.CleanUp();
            DescriptiveGoal.CleanUp();
            var gameObject = new GameObject() { hideFlags = HideFlags.HideAndDontSave };
            var worldState = gameObject.AddComponent<WorldState>();
            try
            {
                await GenerateActionDescription(worldState, service, ct);
                await GenerateGoalDescription(worldState, service, ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DestroyImmediate(gameObject);
            }
        }
        private async Task GenerateActionDescription(WorldState worldState, IGPTService service, CancellationToken ct)
        {
            List<Task> tasks = new();
            foreach (var behavior in GetDescriptiveActions())
            {
                behavior.VirtualInitialize(worldState);
                try
                {
                    var agent = service.CreateGPTAgent();
                    if (string.IsNullOrEmpty(behavior.SelfDescription))
                    {
                        if (sharedDataSet && sharedDataSet.TryGetDescription(behavior.Name, out var description))
                        {
                            behavior.SelfDescription = description;
                        }
                        else
                        {
                            tasks.Add(behavior.DoSelfDescription(agent, ct));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                if (tasks.Count >= 5)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            if (tasks.Count > 0)
                await Task.WhenAll(tasks);
        }
        private async Task GenerateGoalDescription(WorldState worldState, IGPTService service, CancellationToken ct)
        {
            List<Task> tasks = new();
            var dictionary = new Dictionary<string, string>();
            foreach (var action in GetDescriptiveActions())
            {
                dictionary[action.Name] = action.GetConstraints();
            }
            var actionsJson = JsonConvert.SerializeObject(dictionary);
            foreach (var behavior in GetDescriptiveGoals())
            {
                behavior.VirtualInitialize(worldState);
                try
                {
                    var agent = service.CreateGPTAgent();
                    if (string.IsNullOrEmpty(behavior.SelfDescription))
                    {
                        if (sharedDataSet && sharedDataSet.TryGetDescription(behavior.Name, out var description))
                        {
                            behavior.SelfDescription = description;
                        }
                        else
                        {
                            tasks.Add(behavior.DoSelfDescription(actionsJson, agent, ct));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                if (tasks.Count >= 5)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            if (tasks.Count > 0)
                await Task.WhenAll(tasks);
        }
        public bool TryGetDescription(string behaviorName, out string selfDescription)
        {
            foreach (var behavior in Behaviors)
            {
                if (behavior.Name == behaviorName && behavior is ISelfDescriptive selfDescriptive && !string.IsNullOrEmpty(selfDescriptive.SelfDescription))
                {
                    selfDescription = selfDescriptive.SelfDescription;
                    return true;
                }
            }
            selfDescription = null;
            return false;
        }
    }
}
