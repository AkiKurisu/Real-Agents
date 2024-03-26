using UnityEngine;
using UnityEngine.AI;
using Kurisu.AkiBT;
using Random = UnityEngine.Random;
namespace Kurisu.RealAgents.Example
{
    [AkiLabel("Navmesh: SamplePosition")]
    [AkiGroup("Navmesh")]
    public class SamplePosition : Action
    {
        public SharedTObject<Transform> centerTransform;
        public SharedFloat radius;
        [ForceShared]
        public SharedVector3 storeResult;
        protected override Status OnUpdate()
        {
            var point = RandomNavmeshPoint(centerTransform.Value.position, radius.Value);
            if (point == Vector3.zero)
            {
                point = centerTransform.Value.position;
            }
            storeResult.Value = point;
            return Status.Success;
        }
        private static Vector3 RandomNavmeshPoint(Vector3 center, float radius)
        {
            Vector3 randomPoint = Vector3.zero;
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * radius;
                randomDirection += center;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
                {
                    randomPoint = hit.position;
                    break;
                }
            }
            return randomPoint;
        }
    }
}
