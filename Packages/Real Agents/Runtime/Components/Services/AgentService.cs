using UnityEngine;
using Kurisu.NGDS.AI;
using Kurisu.NGDS;
using System.Threading;
namespace Kurisu.RealAgents
{
    public enum GPTModel
    {
        ChatGPT, ChatGLM
    }
    /// <summary>
    /// Service to create ai agent
    /// </summary>
    public class AgentService : SingletonService<AgentService>, IGPTService
    {
        /// <summary>
        /// Set agent gpt model
        /// </summary>
        /// <value></value>
        [field: SerializeField]
        public GPTModel Model { get; set; }
#if UNITY_EDITOR
        [SerializeField]
#endif
        private AITurboSetting setting;
        public string address = "127.0.0.1";
        public int port = 8000;
        private static bool ServerInitialized;
        public string BaseUrl
        {
            get => setting.ChatGPT_URL_Override;
            set => setting.ChatGPT_URL_Override = value;
        }
        public string ApiKey
        {
            get => setting.OpenAIKey;
            set => setting.OpenAIKey = value;
        }
        private void Start()
        {
            if (setting == null) setting = ScriptableObject.CreateInstance<AITurboSetting>();
            if (!ServerInitialized)
                InitializeServer();
        }
        private async void InitializeServer()
        {
            var cts = new CancellationTokenSource();
            long code = await CreateLangChainAgent().Initialize(setting.OpenAIKey, cts.Token);
            if (code == 0)
            {
                Debug.LogWarning("Can not connect LangChain server");
            }
            else if (code != 200)
            {
                Debug.LogError("LangChain server initialized failed!");
            }
            else
            {
                Debug.Log("Initialize LangChain server");
                ServerInitialized = true;
            }
        }
        public LangChainAgent CreateLangChainAgent()
        {
            return new LangChainAgent($"http://{address}:{port}");
        }
        public GPTAgent CreateGPTAgent()
        {
            if (Model == GPTModel.ChatGPT) return new GPTAgent(LLMFactory.Create(LLMType.ChatGPT, setting));
            var agent = new GPTAgent(LLMFactory.Create(LLMType.ChatGLM, setting))
            {
                //TODO: Change prompt to focus on planning tasks
                SystemPrompt = "Please follow the user's instructions carefully."
            };
            return agent;
        }
#if UNITY_EDITOR
        protected override void OnDestroy()
        {
            BaseUrl = string.Empty;
            ApiKey = "打包前请确保APIKey为空";
            base.OnDestroy();
        }
#endif
    }
}
