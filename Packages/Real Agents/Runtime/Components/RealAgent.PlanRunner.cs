using System;
using System.Collections.Generic;
using Kurisu.AkiAI;
namespace Kurisu.RealAgents
{
    public partial class RealAgent
    {
        /// <summary>
        /// Simple task sequence runner
        /// </summary>
        public class PlanRunner
        {
            public event Action<TaskWrapper> OnTaskStart;
            public event Action<TaskWrapper> OnTaskComplete;
            public event Action OnEnd;
            private SequenceTask plan;
            /// <summary>
            /// Abortable means this plan can be aborted and to regenerate one, else it should be running until being abortable or completed.
            /// This aims to prevent agent destroying last created plan.
            /// </summary>
            /// <value></value>
            public bool IsAbortable { get; private set; }
            public bool IsRunning { get; private set; }
            public TaskWrapper Current { get; private set; }
            public void Run(IReadOnlyList<TaskWrapper> taskWrappers)
            {
                if (IsRunning) Abort();
                plan = new SequenceTask(OnPlanEnd);
                foreach (var wrapper in taskWrappers)
                {
                    plan.AppendCallBack(() =>
                    {
                        Current = wrapper;
                        OnTaskStart?.Invoke(wrapper);
                        wrapper.Init();
                        IsAbortable = wrapper.IsAbortable;
                    });
                    plan.Append(wrapper);
                    plan.AppendCallBack(() => OnTaskComplete?.Invoke(wrapper));
                }
                plan.Run();
                IsRunning = true;
            }
            public void Abort()
            {
                plan?.Abort();
                Current?.Action.OnDeactivate();
                Current = null;
                IsRunning = false;
                IsAbortable = true;
            }
            private void OnPlanEnd()
            {
                OnEnd?.Invoke();
                Current = null;
                IsRunning = false;
                IsAbortable = true;
            }
        }
        /// <summary>
        /// Wrapper to reuse action if agent generate duplicated actions
        /// </summary>
        public class TaskWrapper : ITask
        {
            public DescriptiveAction Action { get; }
            public DescriptiveTask Task { get; }
            public TaskWrapper(DescriptiveAction action)
            {
                Action = action;
                Task = action as DescriptiveTask;
            }
            public bool IsAbortable => Task == null;

            public TaskStatus Status => Task?.Status ?? TaskStatus.Enabled;
            public void Init()
            {
                if (Task != null)
                    Task.InitTask();
                else
                    Action.OnActivate();
            }
            public void Tick()
            {
                Task?.Tick();
            }
        }
    }
}