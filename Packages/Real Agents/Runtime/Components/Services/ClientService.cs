using Kurisu.UniChat.LLMs;
namespace Kurisu.RealAgents
{
    /// <summary>
    /// Service to create open ai client
    /// </summary>
    public class ClientService : SingletonService<ClientService>, IClientService
    {
        private readonly LLMSettings setting = new();
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
        public OpenAIClient CreateOpenAIClient()
        {
            return LLMFactory.Create(LLMType.ChatGPT, setting) as OpenAIClient;
        }
    }
}
