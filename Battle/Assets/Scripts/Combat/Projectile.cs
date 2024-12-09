using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;

namespace Apps.Runtime.Combat
{
    public sealed class Projectile : NetworkBehaviour
    {
        [SerializeField] float _speed = 20f;

        readonly NetworkVariable<bool> _activated = new(true);
        NetworkTransform _transform;
        CapsuleCollider _target;
        Action<Projectile> _hitAction;

        public override void OnNetworkSpawn()
        {
            _transform = GetComponent<NetworkTransform>();
            if (IsClient)
            {
                _activated.OnValueChanged += OnActivatedChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsClient)
            {
                _activated.OnValueChanged -= OnActivatedChanged;
            }
        }

        private void OnActivatedChanged(bool _, bool newValue)
        {
            gameObject.SetActive(newValue);
        }

        public void Initialize(CapsuleCollider target, Transform trans, Action<Projectile> hitAction)
        {
            if (!IsServer) return;

            _target = target;
            _hitAction = hitAction;

            _transform.Teleport(trans.position, Quaternion.identity, Vector3.one);
            _activated.Value = true;            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            _hitAction?.Invoke(this);
            _target = null;
            _hitAction = null;
            _activated.Value = false;
        }

        public void Update()
        {
            if (_target)
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
