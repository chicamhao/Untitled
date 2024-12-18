using Apps.Runtime.Core;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace Apps.Runtime.Combat
{
    public sealed class ServerFighter : NetworkBehaviour, IAction
    {
        Weapon _weapon;
        Transform _handTransform;
        Animator _animator;

        ServerMover _mover;

        public ServerReceiver Self => _self;
        ServerReceiver _self;

        ServerReceiver _receiver;

        readonly static int s_attackAnimation = Animator.StringToHash("_attack");
        readonly static int s_stopAttackAnimation = Animator.StringToHash("_stopAttack");

        float _durationCounter;

        public override void OnNetworkSpawn()
        {
            enabled = IsServer;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _mover = GetComponent<ServerMover>();
            _self = GetComponent<ServerReceiver>();
        }

        public void Update()
        {
            _durationCounter += Time.deltaTime;

            if (IsAttacking())
            {
                // appoach the enemy 
                if (!InAttackRange())
                {
                    _mover.MoveTo(_receiver.transform.position);
                }
                // start attacking
                else
                {
                    _mover.Cancel();
                    AttackBehaviour();
                }
            }
        }

        private bool InAttackRange()
        {
            var distance = Vector3.Distance(transform.position, _receiver.transform.position);
            return distance <= _weapon.Range;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_receiver.transform);

            if (_durationCounter >= _weapon.Duration)
            {
                _animator.ResetTrigger(s_stopAttackAnimation);
                _animator.SetTrigger(s_attackAnimation);

                _durationCounter = 0f;
            }
        }

        /// <summary>
        /// move to the target and attack
        /// </summary>
        /// <param name="receiver"></param>
        public void Attack(ServerReceiver receiver)
        {
            _receiver = receiver;
        }

        private bool IsAttacking() => _receiver != null && !_receiver.IsDead();

        public void Cancel()
        {
            _animator.ResetTrigger(s_attackAnimation);
            _animator.SetTrigger(s_stopAttackAnimation);

            if (_receiver)
            {
                _receiver.BeingAttackedBy = null;
                _receiver = null;
            }
        }

        public void ChangeWeapon(Weapon weapon, Transform handTransform)
        {
            _weapon = weapon;
            _handTransform = handTransform;
        }

        #region animation event
        public void Hit()
        {
            if (!IsAttacking()) return;

            _receiver.BeingAttackedBy = this;
            _receiver.Receive(_weapon.Damage);
            if (_receiver.IsDead())
            {
                Cancel();
            }
        }

        public void Shoot()
        {
            if (!IsAttacking()) return;

            var p = NetworkObjectPool.Singleton.GetNetworkObject(_weapon.ProjectilePrefab, _handTransform.position, _handTransform.rotation);
            p.GetComponent<Projectile>().Initialize(_receiver.Collider, Hit);
            if (!p.IsSpawned)
            {
                p.Spawn(true);
            }
        }
        #endregion
    }
}