using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Kurisu.AkiAI
{
    public abstract class AIProxy<T> : MonoBehaviour, IAIProxy<T> where T : IAIContext
    {
        [SerializeField]
        private UnityEvent OnProxyStart;
        public IAIHost<T> Host { get; private set; }
        private SequenceTask sequenceTask;
        private IReadOnlyList<ITask> tasks;
        private Action callBack;
        public SequenceTask GetPlan()
        {
            return sequenceTask;
        }
        public void StartProxy(IAIHost<T> host, IReadOnlyList<ITask> tasks, Action callBack)
        {
            this.callBack = callBack;
            Host = host;
            this.tasks = tasks;
#if UNITY_EDITOR
            Debug.Log($"Start proxy: {GetType().Name}");
#endif
            OnStartProxy();
            OnProxyStart?.Invoke();
        }
        protected virtual void OnStartProxy() { }
        protected void RunProxyTasks()
        {
#if UNITY_EDITOR
            Debug.Log($"Create proxy task sequence: {GetType().Name}");
#endif
            sequenceTask?.Abort();
            sequenceTask = new SequenceTask(tasks, EndProxy);
            sequenceTask.Run();
        }
        protected void EndProxy()
        {
            sequenceTask = null;
#if UNITY_EDITOR
            Debug.Log($"End proxy: {GetType().Name}");
#endif
            callBack?.Invoke();
            callBack = null;
            Host = null;
        }
        public void Abort()
        {
            sequenceTask?.Abort();
            sequenceTask = null;
            OnAbort();
        }
        protected virtual void OnAbort() { }
    }
}
