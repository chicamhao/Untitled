﻿using Apps.Runtime.Control;
using Apps.Runtime.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Apps.Runtime.Combat
{
    public sealed class ServerReceiver : NetworkBehaviour
    {
        // TODO configurable
        public float _health = 100f;
        public bool IsDead => _health == 0;

        private static readonly int s_dieAnimation = Animator.StringToHash("_die");

        public void Receive(float damage)
        {
            _health = Mathf.Max(_health - damage, 0f);
            if (IsDead)
            {
                // TODO cache reference
                GetComponent<Animator>().SetTrigger(s_dieAnimation);
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<ServerActionScheduler>().StartAction(null);
                GetComponent<ServerFighter>().enabled = false;
                GetComponent<ServerAIController>().enabled = false; // TODO round reference
            }
        }
    }
}