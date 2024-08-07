using Apps.Runtime.Core;
using UnityEngine;

namespace Apps.RealTime.Combat
{
    public sealed class Fighter : MonoBehaviour, IAction
    {
        Animator _animator;
        //Mover _mover;

        Receiver _receiver;

        float _durationCounter;
        private readonly static int s_attackAnimation = Animator.StringToHash("_attack");
        private readonly static int s_stopAttackAnimation = Animator.StringToHash("_stopAttack");

        private void Start()
        {
            _animator = GetComponent<Animator>();
           // _mover = GetComponent<Mover>();
        }

        public void Update()
        {
            _durationCounter += Time.deltaTime;

            if (_receiver != null && !_receiver.IsDead)
            {
                if (!InAttackRange())
                {
                   // _mover.MoveTo(_receiver.transform.position);
                }
                else
                {
                   // _mover.Cancel();
                    AttackBehaviour();
                }
            }
        }

        private bool InAttackRange()
        {
            var distance = Vector3.Distance(transform.position, _receiver.transform.position);
            //return distance <= Configurations.AttackRange;
            return true;
        }

        private void AttackBehaviour()
        {
            if (_receiver != null)
            {
                transform.LookAt(_receiver.transform);
            }

            //if (_durationCounter >= Configurations.AttackDuration)
            {
                _animator.ResetTrigger(s_stopAttackAnimation);
                _animator.SetTrigger(s_attackAnimation); // this'll trigger Hit()

                _durationCounter = 0f;
            }
        }

        // animation event
        public void Hit()
        {
            // TODO dodge
            if (_receiver != null)
            {
                // TODO configurable
                _receiver.Receive(damage: 50f);
            }
        }

        /// <summary>
        /// move to the target and attack
        /// /// </summary>
        /// <param name="receiver"></param>
        public void Attack(Receiver receiver)
        {
            // self-attack
            if (transform == receiver.transform) return;

            _receiver = receiver;
        }

        public void Cancel()
        {
            _animator.ResetTrigger(s_attackAnimation);
            _animator.SetTrigger(s_stopAttackAnimation);
            _receiver = null;
        }
    }
}