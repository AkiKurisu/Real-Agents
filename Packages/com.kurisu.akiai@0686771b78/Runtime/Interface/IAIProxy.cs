using System;
using System.Collections.Generic;
namespace Kurisu.AkiAI
{
    public interface IAIProxy
    {
        /// <summary>
        /// Get current plan to append new task or traverse the sequence
        /// </summary>
        /// <returns></returns>
        SequenceTask GetPlan();
        /// <summary>
        /// Abort plan
        /// </summary>
        void Abort();
    }
    public interface IAIProxy<T> : IAIProxy where T : IAIContext
    {
        /// <summary>
        /// Bind host
        /// </summary>
        /// <value></value>
        IAIHost<T> Host { get; }
        /// <summary>
        /// Start a proxy plan
        /// </summary>
        /// <param name="host"></param>
        /// <param name="tasks"></param>
        /// <param name="callBack"></param>
        void StartProxy(IAIHost<T> host, IReadOnlyList<ITask> tasks, Action callBack);
    }
}
