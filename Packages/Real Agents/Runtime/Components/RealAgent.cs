using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Kurisu.AkiAI;
using Kurisu.GOAP;
using Kurisu.UniChat.LLMs;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.RealAgents
{
    public partial class RealAgent : BehaviorBasedAIAgent<IRealAgentContext>, IRealAgentContext
    {
        public class InferenceContext
        {
            private readonly RealAgent agent;
            public string optimalPlanStream;
            public string OptimalAction => optimalPlan[0];
            public string[] optimalPlan;
            public StateCache StateCache { get; private set; }
            public List<TaskWrapper> Tasks { get; } = new();
            public List<IAction> ProceduralPlan { get; } = new();
            public InferenceContext(RealAgent agent)
            {
                this.agent = agent;
            }
            public void RecordPlan()
            {
                ProceduralPlan.Clear();
                ProceduralPlan.AddRange(agent.Planner.ActivatePlan);
                Log($"Get procedural plan: {JsonConvert.SerializeObject(agent.Planner.ActivatePlan.Select(x => x.Name).ToArray())}");
            }
            /// <summary>
            /// Record current world states
            /// </summary>
            public void RecordStates()
            {
                StateCache?.Pooled();
                StateCache = agent.WorldState.LocalState.GetCache();
                StateCache.TryJoin(agent.WorldState.GlobalState.GetStates());
            }
            /// <summary>
            /// Try get tasks from context
            /// </summary>
            /// <returns></returns>
            public bool TryGetTasks()
            {
                Tasks.Clear();
                for (int i = 0; i < optimalPlan.Length; ++i)
                {
                    if (!agent.TryGetAction(optimalPlan[i], out DescriptiveAction descriptiveAction))
                    {
                        return false;
                    }
                    Tasks.Add(new TaskWrapper(descriptiveAction));
                }
                return true;
            }
        }
        public NavMeshAgent NavMeshAgent { get; private set; }
        public Animator Animator { get; private set; }
        public override IRealAgentContext TContext => this;
        private List<DescriptiveGoal> goals;
        private List<DescriptiveAction> actions;
        [SerializeField]
        private string guid = System.Guid.NewGuid().ToString();
        public string Guid => guid;
        [SerializeField]
        private AIGCMode aigcMode;
        public AIGCMode AIGCMode
        {
            get
            {
                return aigcMode;
            }
            set
            {
                if (Application.isPlaying)
                {
                    UpdateAIGCMode(value);
                }
                else
                {
                    aigcMode = value;
                }
            }
        }
        [SerializeField, TextArea]
        private string goal;
        [SerializeField, Range(0, 30), Tooltip("Interval of discovering plan")]
        private float discoverInterval = 5f;
        public string Goal
        {
            get => goal;
            set
            {
                goal = value;
                if (Application.isPlaying)
                {
                    if (AIGCMode == AIGCMode.Auxiliary)
                    {
                        GeneratePriority();
                    }
                    else if (AIGCMode == AIGCMode.Discovering)
                    {
                        StartDiscovering();
                    }
                }
            }
        }
        public string Plan { get; set; }
        public string Action { get; set; }
        private StateStopWatch stateStopWatch;
        private OpenAIClient openAIClient;
        private CancellationTokenSource ct = new();
        public event Action OnAgentUpdate;
        public event Action<string, InferenceContext> OnGeneratePlan;
        private InferenceContext inferenceContext;
        public AgentMemory Memory { get; private set; }
        private PlanGenerator planGenerator;
        private PriorityGenerator priorityGenerator;
        public IPriorityLookup PriorityLookup => priorityGenerator;
        private PlanRunner planRunner;
        private bool waitPlanner;
        protected override void OnAwake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponentInChildren<Animator>();
            InitializeTemplates();
            inferenceContext = new(this);
            stateStopWatch = new(WorldState) { MaxIgnoreTime = discoverInterval * 2 };
            planRunner = new();
            planRunner.OnTaskStart += OnTaskStart;
            planRunner.OnEnd += OnPlanEnd;
            Planner.OnUpdate += OnPlannerUpdate;
        }
        protected override void OnStart()
        {
            openAIClient = ClientService.Instance.CreateOpenAIClient();
            planGenerator = new(this, ClientService.Instance.CreateOpenAIClient());
            priorityGenerator = new(this, ClientService.Instance.CreateOpenAIClient());
            Planner.OnUpdate += OnPlannerUpdate;

        }
        public override void EnableAI()
        {
            base.EnableAI();
            switch (aigcMode)
            {
                case AIGCMode.Discovering:
                    StartDiscovering();
                    break;
                case AIGCMode.Training:
                    StartTraining();
                    break;
            }
        }
        public override void DisableAI()
        {
            CleanUp();
            base.DisableAI();
        }
        protected override void OnDestroy()
        {
            ct.Cancel();
            ct.Dispose();
            stateStopWatch.Dispose();
            planRunner.OnTaskStart -= OnTaskStart;
            planRunner.OnEnd -= OnPlanEnd;
            Planner.OnUpdate -= OnPlannerUpdate;
            base.OnDestroy();
        }
        private void OnTaskStart(TaskWrapper taskWrapper)
        {
            Action = taskWrapper.Action.Name;
            Log($"Plan move to {Action}");
            OnAgentUpdate?.Invoke();
        }
        private void OnPlanEnd()
        {
            Action = null;
            Plan = null;
        }
        private void OnPlannerUpdate(IPlanner planner)
        {
            if (planner.ActivatePlan == null || planner.ActivatePlan.Count == 0)
            {
                Plan = Action = null;
                return;
            }
            Action = planner.ActivatePlan[planner.ActiveActionIndex].Name;
            StringBuilder stringBuilder = new();
            stringBuilder.Append('[');
            for (int i = 0; i < planner.ActivatePlan.Count; ++i)
            {
                if (i != 0)
                    stringBuilder.Append(',');
                stringBuilder.Append(planner.ActivatePlan[i].Name);
            }
            stringBuilder.Append(']');
            Plan = stringBuilder.ToString();
            OnAgentUpdate?.Invoke();
            if (waitPlanner)
            {
                waitPlanner = false;
                //Cache planner result
                inferenceContext.RecordPlan();
                //Set goal
                goal = (planner.ActivateGoal as DescriptiveGoal).SelfDescription;
                TrainingPipeline().Forget();
            }
        }

        private void UpdateAIGCMode(AIGCMode value)
        {
            if (value == aigcMode) return;
            waitPlanner = false;
            StopDiscovering();
            if (value == AIGCMode.Auxiliary)
            {
                GeneratePriority();
            }
            else if (value == AIGCMode.Training)
            {
                StartTraining();
            }
            else
            {
                StartDiscovering();
            }
            aigcMode = value;
        }
        protected override void SetupGOAP()
        {
            goals = DataSet.GetGoals().OfType<DescriptiveGoal>().ToList();
            foreach (var goal in goals)
            {
                goal.Setup(this);
            }
            actions = DataSet.GetActions().OfType<DescriptiveAction>().ToList();
            foreach (var action in actions)
            {
                action.Setup(this);
            }
            Planner.SetGoalsAndActions(goals, actions);
            LoadMemory();
            if (aigcMode == AIGCMode.Auxiliary)
            {
                GeneratePriority();
            }
        }
        private async void GeneratePriority()
        {
            if (priorityGenerator.IsRunning) return;
            LogStatus("Start generating priority...");
            if (!await priorityGenerator.StartGenerate(ct.Token))
            {
                LogError("Generate priority failed");
            }
        }
        private string GetMemoryPath() => Path.Combine(PathUtil.GetOrCreateAgentFolder(guid), "Memory.json");
        public void SaveMemory()
        {
            string path = GetMemoryPath();
            File.WriteAllText(path, JsonUtility.ToJson(Memory, true));
            Log($"Memory save to {path}");
        }
        public void LoadMemory()
        {
            string filePath = GetMemoryPath();
            if (File.Exists(filePath))
            {
                Log($"Load memory from {filePath}");
                Memory = JsonUtility.FromJson<AgentMemory>(File.ReadAllText(filePath));
                Memory.Append(actions);
            }
            else
            {
                Memory = new(actions);
            }
        }
        public Dictionary<string, object> GetActionsMemoryMessage()
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var action in actions)
            {
                if (Memory.TryGetActionMemory(action.Name, out var memory))
                    dictionary.Add(action.Name, memory.ToMessage());
                else
                {
                    LogError($"Action {action.Name} not existed in agent memory");
                }
            }
            return dictionary;
        }
        public IReadOnlyDictionary<string, string> GetGoalsDescription()
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var goal in goals)
            {
                dictionary.Add(goal.Name, goal.SelfDescription);
            }
            return dictionary;
        }
        public bool TryGetAction(string actionName, out DescriptiveAction descriptiveAction)
        {
            foreach (var action in actions)
            {
                if (action.Name == actionName)
                {
                    descriptiveAction = action;
                    return true;
                }
            }
            descriptiveAction = null;
            return false;
        }
        /// <summary>
        /// Wait planner next result and start training pipeline
        /// </summary>
        public void StartTraining()
        {
            LogStatus("Waiting planner next result");
            //Force replan
            Planner.enabled = true;
            waitPlanner = true;
        }
        /// <summary>
        /// Start discovering pipeline
        /// </summary>
        public void StartDiscovering()
        {
            LogStatus("Start discovering mode");
            CleanUp();
            //Close procedural planner
            Planner.AbortActivePlan();
            Planner.enabled = false;
            //Set flag to discover immediately
            stateStopWatch.StateChanged = true;
            DiscoveringPipeline().Forget();
        }
        public void StopDiscovering()
        {
            LogStatus("Stop discovering mode");
            CleanUp();
            Planner.enabled = true;
        }
        /// <summary>
        /// Kill all running tasks and reset states
        /// </summary>
        private void CleanUp()
        {
            ct.Cancel();
            ct.Dispose();
            ct = new();
            planRunner.Abort();
            stateStopWatch.StateChanged = false;
            Plan = Action = null;
            OnAgentUpdate?.Invoke();
        }
        /// <summary>
        /// Discover plan on input context
        /// </summary>
        /// <param name="evaluateContext"></param>
        /// <returns></returns>
        private async UniTask<bool> TryDiscoverPlan(InferenceContext evaluateContext)
        {
            bool discoverState = await GeneratePlan(evaluateContext);
            if (discoverState)
                discoverState &= evaluateContext.TryGetTasks();
            return discoverState;
        }
        /// <summary>
        /// Run evaluate context's wrapped plan (task sequence)
        /// </summary>
        /// <param name="evaluateContext"></param>
        private void RunPlan(InferenceContext evaluateContext)
        {
            Action = evaluateContext.OptimalAction;
            Plan = evaluateContext.optimalPlanStream;
            OnGeneratePlan?.Invoke(Goal, evaluateContext);
            Log($"Start plan: {Plan}");
            planRunner.Run(evaluateContext.Tasks);
        }
        #region Training Pipeline
        private async UniTask TrainingPipeline()
        {
            //1. Initialize
            //* Record states for preventing influence when state changed during discovering
            inferenceContext.RecordStates();
            LogSuccess("Training pipeline start");
            //2. Discover
            if (!await TryDiscoverPlan(inferenceContext))
            {
                //Set dirty flag to force discover next interval time
                LogWarning("Plan discovering failed, training pipeline will restart later");
                waitPlanner = true;
                return;
            }
            //3. Evaluate plan
            await EvaluatePlan(inferenceContext);
            WaitNextTraining().Forget();
        }
        private async UniTask WaitNextTraining()
        {
            await UniTask.WaitForSeconds(discoverInterval);
            StartTraining();
        }
        #endregion
        #region  Discovering Pipeline

        private async UniTask WaitNextDiscovering()
        {
            await UniTask.WaitForSeconds(discoverInterval);
            DiscoveringPipeline().Forget();
        }
        private bool ValidateCurrentAction()
        {
            if (!planRunner.IsRunning) return false;
            return planRunner.Current.Action.PreconditionsSatisfied(WorldState);
        }
        /// <summary>
        /// Abort plan if possible
        /// </summary>
        /// <returns></returns>
        private bool TryAbortRunningPlan()
        {
            if (!planRunner.IsAbortable)
            {
                WaitNextDiscovering().Forget();
                return false;
            }
            else
            {
                if (!ValidateCurrentAction())
                {
                    LogWarning($"Plan is aborted since {Action}'s preconditions were not satisfied");
                    CleanUp();
                    stateStopWatch.StateChanged = true;
                }
            }
            return true;
        }
        private bool InitializeDiscoveringPipeline()
        {
            //1. Check is running
            if (planRunner.IsRunning)
            {
                //2. Check running plan is abortable
                if (!TryAbortRunningPlan())
                {
                    Log("Plan is blocking, plan discovering was skipped");
                    return false;
                }
            }
            //3. Check is dirty
            if (!stateStopWatch.StateChanged)
            {
                Log("State not changed, plan discovering was skipped");
                WaitNextDiscovering().Forget();
                return false;
            }
            return true;
        }
        /// <summary>
        /// Pipeline of discovering mode
        /// </summary>
        /// <returns></returns>
        private async UniTask DiscoveringPipeline()
        {
            //1. Initialize
            if (!InitializeDiscoveringPipeline()) return;
            stateStopWatch.StateChanged = false;
            //* Record states for preventing influence when state changed during discovering
            inferenceContext.RecordStates();
            LogStatus("Discovering pipeline start");
            //2. Discover
            if (!await TryDiscoverPlan(inferenceContext))
            {
                //Set dirty flag to force discover next interval time
                stateStopWatch.StateChanged = true;
                LogWarning("Plan discovering failed, discovering pipeline will restart later");
                WaitNextDiscovering().Forget();
                return;
            }
            //3. Discovering only evaluate preconditions
            if (await EvaluatePrecondition(inferenceContext))
            {
                RunPlan(inferenceContext);
            }
            WaitNextDiscovering().Forget();
        }
        #endregion

        #region  Logger
        private static void Log(string message)
        {
            Debug.Log($"[Real Agent] {message}");
        }
        private static void LogSuccess(string message)
        {
            Debug.Log($"<color=#3aff48>[Real Agent] {message}</color>");
        }
        private static void LogStatus(string message)
        {
            Debug.Log($"<color=#00C2FF>[Real Agent] {message}</color>");
        }
        private static void LogWarning(string message)
        {
            Debug.LogWarning($"[Real Agent] {message}");
        }
        private static void LogError(string message)
        {
            Debug.LogError($"[Real Agent] {message}");
        }
        #endregion
    }
}
