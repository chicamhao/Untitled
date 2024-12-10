using Apps.Runtime.Core;
using Unity.Cinemachine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;

namespace Apps.Runtime.Movement
{
    public sealed class ServerMover : NetworkBehaviour, IAction
    {
        [SerializeField] NetworkTransform _networkTransform;
        [SerializeField] CinemachineCamera _camera;
        [SerializeField] NavMeshAgent _navMeshAgent;
        [SerializeField] Animator _animator;

        readonly static int s_forwardSpeedAnimation = Animator.StringToHash("_forwardSpeed");

        public override void OnNetworkSpawn()
        {
            enabled = IsServer;
            if (_camera != null)
            {
                _camera.enabled = IsLocalPlayer;
            }
        }

        private void Update()
        {
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination)
        {
            if (Mathf.Approximately(0.01f, Vector3.Distance(destination, transform.position)))
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            _navMeshAgent.SetDestination(destination);
            _navMeshAgent.isStopped = false;
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            _networkTransform.Teleport(position, rotation, Vector3.one);

            // turn off in the prefab to avoid no NavMesh when spawned
            _navMeshAgent.enabled = true;
            _navMeshAgent.Warp(position);
            _navMeshAgent.isStopped = true;
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