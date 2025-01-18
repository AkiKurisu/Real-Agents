using System;
using Kurisu.AkiBT;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Kurisu.AkiAI
{
    /// <summary>
    /// Task controlled with external state machine
    /// </summary>
    public abstract class StatusTask
    {
        public TaskStatus Status { get; private set; }
        #region  Status controlled by agent
        public void Stop()
        {
            Status = TaskStatus.Disabled;
        }
        public void Start()
        {
            Status = TaskStatus.Enabled;
        }
        public void Pause()
        {
            Status = TaskStatus.Pending;
        }
        #endregion
    }
    /// <summary>
    /// Task to run a behavior tree inside a agent-authority state machine.
    /// Whether behavior tree is failed or succeed will not affect task status. 
    /// </summary>
    [Serializable]
    public class BehaviorTask : StatusTask, IAITask
    {
        [SerializeField, TaskID]
        private string taskID;
        public string TaskID => taskID;
        [SerializeField]
        private bool isPersistent;
        public bool IsPersistent => isPersistent;
#if AKIAI_TASK_JSON_SERIALIZATION
        [SerializeField]
        private TextAsset behaviorTreeSerializeData;
#else
        [SerializeField]
        private BehaviorTreeSO behaviorTree;
#endif
        private BehaviorTreeSO instanceTree;
        public BehaviorTreeSO InstanceTree => instanceTree;
        public void Init(IAIHost host)
        {
#if AKIAI_TASK_JSON_SERIALIZATION
            instanceTree = ScriptableObject.CreateInstance<BehaviorTreeSO>();
            instanceTree.Deserialize(behaviorTreeSerializeData.text);
#else
            instanceTree = Object.Instantiate(behaviorTree);
#endif
            foreach (var variable in instanceTree.SharedVariables)
                variable.MapTo(host.BlackBoard);
            instanceTree.Init(host.Object);
        }
        public void Tick()
        {
            instanceTree.Update();
        }
    }
}