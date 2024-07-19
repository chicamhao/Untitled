using Apps.RealTime.Combat;
using Apps.RealTime.Core;
using Apps.RealTime.Movement;
using UnityEngine;

namespace Apps.RealTime.Control
{
    public sealed class AIController : MonoBehaviour
    {
        enum Category
        {
            Guardian
        }

        [SerializeField] PatrolPath _partrolPath;
        [SerializeField] Category _category;

        Fighter _fighter;
        Receiver _receiver;
        Mover _mover;
        ActionScheduler _actionScheduler;

        // suspecion interval
        float _timeSinceLastSawPlayer = float.MaxValue;

        // guarding path
        float _timeAtWayPoint = float.MaxValue;
        Vector3 _currentWayPoint;

        void Start()
        {
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();

            _currentWayPoint = transform.position;

            // TODO multiplayer
            _receiver = GameObject.FindWithTag("Player").GetComponent<Receiver>();
        }

        void Update()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeAtWayPoint += Time.deltaTime;

            if (TryAttack()) return;
            if (TrySuspect()) return;
            Guard();
        }

        private bool TryAttack()
        {
            if (InObservantRange())
            {
                _timeSinceLastSawPlayer = 0f;
                if (_actionScheduler.StartAction(_fighter))
                {
                    _fighter.Attack(_receiver);
                }
                return true;
            }
            return false;
        }

        private bool TrySuspect()
        {
            if (_timeSinceLastSawPlayer < Configurations.SuspicionTime)
            {
                _actionScheduler.StartAction(null);
                return true;
            }
            return false;
        }

        private void Guard()
        {
            if (_category != Category.Guardian) return;

            if (_category == Category.Guardian && _partrolPath == null)
                throw new System.InvalidOperationException();

            
            var wayPoint = _partrolPath.GetWayPoint(transform.position);
            if (wayPoint.AtWayPoint && _timeAtWayPoint >= Configurations.ObservantRange)
            {
                _timeAtWayPoint = 0f;
                _currentWayPoint = wayPoint.NextWayPoint;
            }
            
            _actionScheduler.StartAction(_mover);
            _mover.MoveTo(_currentWayPoint);
        }

        private bool InObservantRange()
        {
            var distance = Vector3.Distance(gameObject.transform.position, _receiver.transform.position);
            return distance <= Configurations.ObservantRange;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, Configurations.ObservantRange);
        }
    }
}
