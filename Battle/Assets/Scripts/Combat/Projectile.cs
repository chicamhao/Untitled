using UnityEngine;
using Unity.Netcode;
using System;

namespace Apps.Runtime.Combat
{
	public sealed class Projectile : NetworkBehaviour
	{
        [SerializeField] float _speed = 10f;
        CapsuleCollider _target;
        Action _hitAction;

        public void Initialize(CapsuleCollider target, Action hitAction)
        {
            _target = target;
            _hitAction = hitAction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsServer && _target != null)
            {
                _hitAction?.Invoke();
            }            
        }

        public void Update()
        {
            if (_target != null)
            {
                transform.LookAt(GetLookAtPosition());
                transform.Translate(_speed * Time.deltaTime * Vector3.forward);
            }
        }

        private Vector3 GetLookAtPosition()
        {
            return _target.transform.position + Vector3.up * _target.height / 2;
        }

        public void Cancel()
        {
            _target = null;
            _hitAction = null;
        }
    }
}
