using Kurisu.NGDS;
using Kurisu.NGDS.AI;
namespace Kurisu.RealAgents.Editor
{
    public class GPTEditorService : IGPTService
    {
        public GPTModel Model => realAgentsSetting.EditorGPTModel;
        private readonly AITurboSetting aiTurboSetting;
        private readonly RealAgentsSetting realAgentsSetting;
        public GPTEditorService()
        {
            aiTurboSetting = (realAgentsSetting = RealAgentsSetting.GetOrCreateSettings()).AITurboSetting;
        }
        public GPTAgent CreateGPTAgent()
        {
            return new GPTAgent(LLMFactory.Create(realAgentsSetting.EditorGPTModel == GPTModel.ChatGPT ? LLMType.ChatGPT : LLMType.ChatGLM, aiTurboSetting));
        }
    }
}