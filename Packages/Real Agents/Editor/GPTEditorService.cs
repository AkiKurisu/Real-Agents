using Kurisu.UniChat.LLMs;
namespace Kurisu.RealAgents.Editor
{
    public class GPTEditorService : IClientService
    {
        private readonly ILLMSettings llmSetting;
        public GPTEditorService()
        {
            llmSetting = RealAgentsSetting.GetOrCreateSettings().LLMSettings;
        }
        public OpenAIClient CreateOpenAIClient()
        {
            return LLMFactory.Create(LLMType.ChatGPT, llmSetting) as OpenAIClient;
        }
    }
}