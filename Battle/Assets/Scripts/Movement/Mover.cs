using Apps.RealTime.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Apps.RealTime.Movement
{
    public sealed class Mover : MonoBehaviour, IAction
    {
        [SerializeField] ActionScheduler _actionScheduler;
        [SerializeField] GameObject _target;
        [SerializeField] NavMeshAgent _navMeshAgent;
        [SerializeField] Animator _animator;

        private readonly static int s_forwardSpeedAnimation = Animator.StringToHash("_forwardSpeed");

        private void Update()
        {
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination)
        {
            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            // transform velocity direction from global space to local space
            var velocity = transform.InverseTransformDirection(_navMeshAgent.velocity);

            // forward direction
            _animator.SetFloat(s_forwardSpeedAnimation, Mathf.Abs(velocity.z));
        }
    }
}
