using Kurisu.UniChat.LLMs;
namespace Kurisu.RealAgents
{
    /// <summary>
    /// Service to create open ai client
    /// </summary>
    public class ClientService : SingletonService<ClientService>, IClientService
    {
        private readonly LLMSettings setting = new();
        private LLMFactory factory;
        public string BaseUrl
        {
            get => setting.OpenAI_API_URL;
            set => setting.OpenAI_API_URL = value;
        }
        public string ApiKey
        {
            get => setting.OpenAIKey;
            set => setting.OpenAIKey = value;
        }
        protected override void Awake()
        {
            base.Awake();
            factory = new LLMFactory(setting);
        }
        public OpenAIClient CreateOpenAIClient()
        {
            return factory.CreateLLM(LLMType.OpenAI) as OpenAIClient;
        }
    }
}
