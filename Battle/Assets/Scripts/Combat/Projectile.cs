using UnityEngine;
using Unity.Netcode;
using System;

namespace Apps.Runtime.Combat
{
	public sealed class Projectile : NetworkBehaviour
	{
        [SerializeField] float _speed = 10f;
        CapsuleCollider _target;
        Action<Projectile> _hitAction;

        public void Initialize(CapsuleCollider target, Transform trans, Action<Projectile> hitAction)
        {
            _target = target;
            _hitAction = hitAction;
            transform.SetPositionAndRotation(trans.position, trans.rotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            _hitAction?.Invoke(this);
            _target = null;
            _hitAction = null;
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
    }
}
