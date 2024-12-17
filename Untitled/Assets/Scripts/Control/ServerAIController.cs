using Apps.Runtime.Combat;
using Apps.Runtime.Core;
using Apps.Runtime.Data;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Control
{
    public sealed class ServerAIController : NetworkBehaviour
    {
        [SerializeField] PatrolPath _partrolPath;
        [SerializeField] float _observantRange = 5f;
        [SerializeField] float _suspicionTime = 2f;

        ServerFighter _fighter;
        ServerReceiver[] _receivers;
        ServerReceiver _self;
        ServerMover _mover;
        ServerActionScheduler _actionScheduler;

        // suspecion interval
        float _timeSinceLastSawPlayer = float.MaxValue;

        // guarding path
        float _timeAtWayPoint = float.MaxValue;
        Vector3 _currentWayPoint;

        public override void OnNetworkSpawn()
        {
            enabled = IsServer;

            // TODO configurable
            GetComponent<Status>().Initialize(string.Empty, 500);
        }

        private void Start()
        {
            _fighter = GetComponent<ServerFighter>();
            _mover = GetComponent<ServerMover>();
            _actionScheduler = GetComponent<ServerActionScheduler>();
            _self = GetComponent<ServerReceiver>();

            _currentWayPoint = transform.position;
        }

        public void Initialize(GameObject[] players)
        {
            _receivers = new ServerReceiver[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                _receivers[i] = players[i].GetComponent<ServerReceiver>();
            }
        }

        void Update()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeAtWayPoint += Time.deltaTime;

            if (_receivers != null)
            {
                foreach (var receiver in _receivers)
                {
                    if (TryAttack(receiver)) return;
                }
            }
            if (TryCounterAttack()) return;
            if (TrySuspect()) return;
            Guard();
        }

        private bool TryAttack(ServerReceiver receiver)
        {
            if (!receiver.IsDead() && InObservantRange(receiver))
            {
                _timeSinceLastSawPlayer = 0f;
                Attack(receiver);
                return true;
            }
            return false;
        }

        private bool TryCounterAttack()
        {
            if (!_self.BeingAttacked) return false;

            Attack(_self.BeingAttackedBy.Self);
            return true;
        }

        private void Attack(ServerReceiver receiver)
        {
            if (_actionScheduler.StartAction(_fighter))
            {
                _fighter.Attack(receiver);
            }
        }

        private bool TrySuspect()
        {
            if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                _actionScheduler.StartAction(null);
                return true;
            }
            return false;
        }

        private void Guard()
        {
            if (_partrolPath == null) return;
            
            var wayPoint = _partrolPath.GetWayPoint(transform.position);
            if (wayPoint.AtWayPoint && _timeAtWayPoint >= _observantRange)
            {
                _timeAtWayPoint = 0f;
                _currentWayPoint = wayPoint.NextWayPoint;
            }
            
            _actionScheduler.StartAction(_mover);
            _mover.MoveTo(_currentWayPoint);
        }

        private bool InObservantRange(ServerReceiver receiver)
        {
            var distance = Vector3.Distance(gameObject.transform.position, receiver.transform.position);
            return distance <= _observantRange;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _observantRange);
        }
    }
}