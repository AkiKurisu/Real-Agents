using Kurisu.UniChat.LLMs;
namespace Kurisu.RealAgents.Editor
{
    public class GPTEditorService : IClientService
    {
        private readonly ILLMSettings llmSetting;
        private readonly LLMFactory llmFactory;
        public GPTEditorService()
        {
            llmSetting = RealAgentsSetting.GetOrCreateSettings().LLMSettings;
            llmFactory = new(llmSetting);
        }
        public OpenAIClient CreateOpenAIClient()
        {
            return llmFactory.CreateLLM(LLMType.OpenAI) as OpenAIClient;
        }
    }
}