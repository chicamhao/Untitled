using Apps.Runtime.Core;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    public sealed class ServerFighter : NetworkBehaviour, IAction
    {
        [SerializeField] Weapon _weapon;
        GameObject _handedWeapon;

        Animator _animator;
        ServerMover _mover;
        ServerReceiver _receiver;

        float _durationCounter;
        private readonly static int s_attackAnimation = Animator.StringToHash("_attack");
        private readonly static int s_stopAttackAnimation = Animator.StringToHash("_stopAttack");

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

        private bool IsAttacking() => _receiver != null;

        public void Cancel()
        {
            _animator.ResetTrigger(s_attackAnimation);
            _animator.SetTrigger(s_stopAttackAnimation);
            _receiver = null;
        }

        public void ChangeWeapon(Weapon weapon, GameObject handedWeapon)
        {
            _weapon = weapon;
            _handedWeapon = handedWeapon;
        }

        #region animation event
        public void Hit()
        {
            if (IsAttacking())
            {
                _receiver.Receive(_weapon.Damage);
                if (_receiver.IsDead())
                {
                    Cancel();
                }
            }
        }

        public void Shoot()
        {
            var projectile = Instantiate(_weapon.ProjectilePrefab,
                _handedWeapon.transform.position, _handedWeapon.transform.rotation);
            projectile.Initialize(_receiver.GetComponent<CapsuleCollider>(), Hit);    
        }
        #endregion
    }
}