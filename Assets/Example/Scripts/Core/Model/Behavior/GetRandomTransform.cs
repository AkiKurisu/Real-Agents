using System.Linq;
using Kurisu.AkiBT;
using Kurisu.Framework;
using UnityEngine;
namespace Kurisu.RealAgents.Example
{
    [AkiLabel("Transform: GetRandomTransform")]
    [AkiGroup("Transform")]
    public class GetRandomTransform : Action
    {
        public SharedTObject<Transform> parent;
        [ForceShared]
        public SharedTObject<Transform> storeResult;
        private Transform[] transforms;
        protected override Status OnUpdate()
        {
            transforms ??= parent.Value.GetComponentsInChildren<Transform>().Where(x => x != parent.Value).ToArray();
            storeResult.Value = transforms.Random();
            return Status.Success;
        }
    }
}
