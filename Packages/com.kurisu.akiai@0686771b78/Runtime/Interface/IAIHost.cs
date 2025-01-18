using Kurisu.GOAP;
using UnityEngine;
namespace Kurisu.AkiAI
{
    public interface IAIContext { }
    public interface IAIHost
    {
        IAIContext Context { get; }
        IAIBlackBoard BlackBoard { get; }
        WorldState WorldState { get; }
        GameObject Object { get; }
        Transform Transform { get; }
        IAITask GetTask(string taskID);
        void AddTask(IAITask task);
        /// <summary>
        /// Whether ai agent is enabled
        /// </summary>
        /// <value></value>
        bool IsAIEnabled { get; }
        void EnableAI();
        void DisableAI();
    }
    public interface IAIHost<T> : IAIHost where T : IAIContext
    {
        /// <summary>
        /// Customized AI Context
        /// </summary>
        /// <value></value>
        T TContext { get; }
    }
}
