using System;
using Apps.Runtime.Control;
using Apps.Runtime.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Apps.Runtime.Combat
{
    public sealed class ServerReceiver : NetworkBehaviour
    {
        public IStatus Status => _status;
        Status _status;

        private static readonly int s_dieAnimation = Animator.StringToHash("_die");

        public override void OnNetworkSpawn()
        {
            enabled = IsServer;
        }

        public void Start()
        {
            _status = GetComponent<Status>();
        }

        public void Receive(uint damage)
        {
            _status.Subtract(damage); 
            if (_status.IsDead)
            {
                // TODO cache reference
                GetComponent<Animator>().SetTrigger(s_dieAnimation);
                GetComponent<ServerActionScheduler>().StartAction(null);
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<ServerFighter>().enabled = false;
                GetComponent<Collider>().enabled = false;

                // TODO resolve round reference
                if (TryGetComponent<PlayerController>(out var p))
                {
                    p.enabled = false;
                }
                if (TryGetComponent<ServerAIController>(out var ai))
                {
                    ai.enabled = false; 
                }
            }
        }

        public bool IsDead() => _status.IsDead;
    }
}