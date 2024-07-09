using Apps.RealTime.Core;
using Apps.RealTime.Movement;
using UnityEngine;

namespace Apps.RealTime.Combat
{
	public sealed class Fighter : MonoBehaviour, IAction
	{
		Animator _animator;
		Mover _attacker;
		Receiver _receiver;

		float _durationCounter;
        private readonly static int s_attackAnimation = Animator.StringToHash("_attack");
		private readonly static int s_stopAttackAnimation = Animator.StringToHash("_stopAttack");

        private void Start()
        {
			_animator = GetComponent<Animator>();
        }

        public void Update()
        {
			_durationCounter += Time.deltaTime;

            if (_receiver != null && !_receiver.IsDead && _attacker != null)
			{
				var distance = Vector3.Distance(transform.position, _receiver.transform.position);
				if (distance > Configurations.AttackRange)
				{
                    _attacker.MoveTo(_receiver.transform.position);
				}
				else
				{
					_attacker.Cancel();
					AttackBehaviour();
				}
			}
        }

		private void AttackBehaviour()
		{
			if (_receiver != null)
			{
				transform.LookAt(_receiver.transform);
			}

			if (_durationCounter >= Configurations.AttackDuration)
			{
				_animator.ResetTrigger(s_stopAttackAnimation);
				_animator.SetTrigger(s_attackAnimation); // this'll trigger Hit()

                _durationCounter = 0f;
			}
        }

		// animation event
		public void Hit()
		{
			if (_receiver != null)
			{
				// TODO configurable
				_receiver.Receive(damage: 50f);
			}
		}

        public void Attack(Receiver receiver, Mover attacker)
		{
			// self-attack
			if (transform == receiver.transform) return;

            _receiver = receiver;
			_attacker = attacker;
		}

		public void Cancel()
		{
			_animator.ResetTrigger(s_attackAnimation);
			_animator.SetTrigger(s_stopAttackAnimation);
			_receiver = null;
		}
	}
}