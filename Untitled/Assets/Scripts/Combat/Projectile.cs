using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    public sealed class Projectile : NetworkBehaviour
    {
        [SerializeField] float _speed = 20f;
        [SerializeField] float _range = 30f;

        Vector3 _start;
        CapsuleCollider _target;
        Action _hitAction;

        public void Initialize(CapsuleCollider target, Action hitAction)
        {
            _target = target;
            _start = transform.position;
            transform.LookAt(_target.transform.position + Vector3.up * _target.height / 2);
            _hitAction = hitAction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsServer)
            {
                Hit(other.tag);
            }
        }

        private void Hit(string tag)
        {
            if (_target && !_target.CompareTag(tag)) return;
            NetworkObject.Despawn(true);
            _hitAction?.Invoke();
        }

        public void Update()
        {
            if (_target)
            {
                if (Vector3.Distance(_start, transform.position) > _range)
                {
                    NetworkObject.Despawn(true);
                }
                else
                {
                    transform.Translate(_speed * Time.deltaTime * Vector3.forward);
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            _target = null;
            _hitAction = null;
            transform.position = _start;
        }
    }
}