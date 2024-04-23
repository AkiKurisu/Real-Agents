using System.Threading;
using Cysharp.Threading.Tasks;
using Kurisu.GOAP;
namespace Kurisu.RealAgents
{
    public interface ISelfDescriptive
    {
        /// <summary>
        /// Author made or agent generated description
        /// </summary>
        /// <value></value>
        string SelfDescription { get; set; }
        /// <summary>
        /// Inject virtual worldState and initialize behavior
        /// </summary>
        /// <param name="worldState"></param>
        void VirtualInitialize(WorldState worldState);
    }
    public interface ISelfDescriptiveSet
    {
        UniTask DoSelfDescription(IClientService service, CancellationToken ct);
    }
}
