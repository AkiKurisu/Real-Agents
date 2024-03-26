using Kurisu.NGDS.AI;
namespace Kurisu.RealAgents
{
    public interface IGPTService
    {
        GPTModel Model { get; }
        GPTAgent CreateGPTAgent();
    }
}