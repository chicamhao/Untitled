using Apps.Runtime.Combat;
using Apps.Runtime.Core;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Control
{
    public sealed class ServerAIController : NetworkBehaviour
    {
        [SerializeField] PatrolPath _partrolPath;
        [SerializeField] float _observantRange = 5f;
        [SerializeField] float _suspicionTime = 4f;

        ServerFighter _fighter;
        ServerReceiver[] _receivers;
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
            GetComponent<Status>().Initialize(200); 

            // TODO cache references
            var players = GameObject.FindGameObjectsWithTag("Player");
            _receivers = new ServerReceiver[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                _receivers[i] = players[i].GetComponent<ServerReceiver>();
            }
        }

        private void Start()
        {
            _fighter = GetComponent<ServerFighter>();
            _mover = GetComponent<ServerMover>();
            _actionScheduler = GetComponent<ServerActionScheduler>();

            _currentWayPoint = transform.position;
        }

        void Update()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeAtWayPoint += Time.deltaTime;

            foreach (var receiver in _receivers)
            {
                if (receiver == null) continue; 
                if (TryAttack(receiver)) return;
            }
            if (TrySuspect()) return;
            Guard();
        }

        private bool TryAttack(ServerReceiver receiver)
        {
            if (InObservantRange(receiver))
            {
                _timeSinceLastSawPlayer = 0f;
                if (_actionScheduler.StartAction(_fighter))
                {
                    _fighter.Attack(receiver);
                }
                return true;
            }
            return false;
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