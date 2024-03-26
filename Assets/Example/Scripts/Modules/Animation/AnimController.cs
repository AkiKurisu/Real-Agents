using UnityEngine;
using UnityEngine.AI;
namespace Kurisu.RealAgents.Example.Anim
{
    public class AnimController : MonoBehaviour
    {
        private int _animIDSpeed;
        private float velocity;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            _animIDSpeed = Animator.StringToHash("Speed");
            animator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (!animator) return;
            velocity = Mathf.Lerp(velocity, navMeshAgent.velocity.magnitude, Time.deltaTime * 10f);
            animator.SetFloat(_animIDSpeed, velocity);
        }
    }
}
