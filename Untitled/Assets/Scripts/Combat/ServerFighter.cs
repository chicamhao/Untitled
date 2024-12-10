using Apps.Runtime.Core;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    public sealed class ServerFighter : NetworkBehaviour, IAction
    {
        Weapon _weapon;
        Transform _handTransform;
        Animator _animator;

        ServerMover _mover;
        ServerReceiver _receiver;

        ProjectilePool _projectilePool;
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
        /// /// </summary>
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
            _receiver = null;
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

            _receiver.Receive(_weapon.Damage);
            if (_receiver.IsDead())
            {
                Cancel();
            }
        }

        public void Shoot()
        {
            if (!IsAttacking()) return;

            _projectilePool ??= new ProjectilePool();
            var projectile = _projectilePool.Spawn(_weapon.ProjectilePrefab);
            projectile.Initialize(
                _receiver.Collider, _handTransform.transform, ProjectileHitCallback);
        }

        private void ProjectileHitCallback(Projectile projectile)
        {
            Hit();
            _projectilePool.Release(projectile);
        }
        #endregion
    }
}