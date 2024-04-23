using Kurisu.UniChat.LLMs;
namespace Kurisu.RealAgents
{
    public interface IClientService
    {
        OpenAIClient CreateOpenAIClient();
    }
}