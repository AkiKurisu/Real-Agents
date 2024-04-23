using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        public async UniTask DoSelfDescription(IClientService service, CancellationToken ct)
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
        private async UniTask GenerateActionDescription(WorldState worldState, IClientService service, CancellationToken ct)
        {
            List<UniTask> tasks = new();
            foreach (var behavior in GetDescriptiveActions())
            {
                behavior.VirtualInitialize(worldState);
                try
                {
                    var agent = service.CreateOpenAIClient();
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
                    await UniTask.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            if (tasks.Count > 0)
                await UniTask.WhenAll(tasks);
        }
        private async UniTask GenerateGoalDescription(WorldState worldState, IClientService service, CancellationToken ct)
        {
            List<UniTask> tasks = new();
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
                    var agent = service.CreateOpenAIClient();
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
                    await UniTask.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            if (tasks.Count > 0)
                await UniTask.WhenAll(tasks);
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
