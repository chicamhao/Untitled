using Apps.RealTime.Combat;
using Apps.Runtime.Core;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Apps.Runtime.Control
{
    /// <summary>
    /// with multiplay context, it's a client input sender
    /// </summary>
    public sealed class PlayerController : NetworkBehaviour
    {
        //[SerializeField] ActionScheduler _actionScheduler;
        //[SerializeField] ServerMover _serverMover;
        [SerializeField] Fighter _fighter;
        [SerializeField] ServerPlayerController _serverPlayerController;

        private Camera _followCamera;
        public NetworkVariable<int> Health = new(100);
        public NetworkVariable<FixedString32Bytes> PlayerName = new(new FixedString32Bytes(string.Empty));

        bool _interactRequest;

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _interactRequest = true;
            }
        }

        private void FixedUpdate()
        {
            if (_interactRequest)
            {
                _interactRequest = !_interactRequest;

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray); // TODO none alloc

                foreach (var hit in hits)
                {
                    // attack
                    if (hit.transform.TryGetComponent<Receiver>(out var receiver))
                    {
                        //_actionScheduler.StartAction(_fighter);
                        _fighter.Attack(receiver);
                        continue;
                    }

                    // movement
                    if (hit.transform.TryGetComponent<Terrain>(out var _))
                    {
                        _serverPlayerController.RequestMoveRpc(hit.point);
                        //_actionScheduler.StartAction(_mover);
                       // _mover.MoveTo(hit.point);
                        continue;
                    }
                }
            }

            if (IsLocalPlayer && _followCamera)
            {
                _followCamera.transform.parent.transform.position = transform.position;
            }
        }

        public void Inititalize(Vector3 position, Camera camera)
        {
            _serverPlayerController.TeleportRpc(position);
            _followCamera = camera;
        }
    }
}
