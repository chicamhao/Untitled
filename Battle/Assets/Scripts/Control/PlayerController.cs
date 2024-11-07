using Apps.Runtime.Combat;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Control
{
    /// <summary>
    /// as in multiplay context, it's a client input sender
    /// </summary>
    public sealed class PlayerController : NetworkBehaviour
    {
        [SerializeField] ServerPlayerController _serverPlayerController;

        IFollowCamera _followCamera;
        public NetworkVariable<int> Health = new(100); // TODO configurable

        bool _interactRequest;

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            if (IsLocalPlayer && Input.GetMouseButton(0))
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
                    if (hit.transform.TryGetComponent<ServerReceiver>(out var receiver))
                    {
                        _serverPlayerController.AttackRpc(receiver.NetworkObjectId);
                        break;
                    }

                    // movement
                    if (hit.transform.TryGetComponent<Terrain>(out var _))
                    {
                        _serverPlayerController.MoveRpc(hit.point);
                        break;
                    }
                }
            }

            if (IsLocalPlayer && _followCamera != null)
            {
                _followCamera.Follow(transform);
            }
        }

        public void Inititalize(Vector3 position, Quaternion rotation, IFollowCamera camera)
        {
            _serverPlayerController.TeleportRpc(position, rotation);
            _followCamera = camera;
        }
    }
}
