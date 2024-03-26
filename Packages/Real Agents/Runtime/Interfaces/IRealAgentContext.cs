using Kurisu.AkiAI;
using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.RealAgents
{
    public interface IRealAgentContext : IAIContext
    {
        NavMeshAgent NavMeshAgent { get; }
        Animator Animator { get; }
        string Goal { get; set; }
        string Plan { get; set; }
        string Action { get; set; }
        IPriorityLookup PriorityLookup { get; }

    }
    public interface IPriorityLookup
    {
        bool TryGetPriority(string goalName, out float priority);
    }
}
