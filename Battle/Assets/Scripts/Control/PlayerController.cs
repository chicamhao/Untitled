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
                if (Physics.Raycast(ray, out var hit)) // TODO none alloc
                {
                    // attack
                    if (hit.transform.TryGetComponent<ServerReceiver>(out var receiver))
                    {
                        if (!receiver.CompareTag(tag))
                        {
                            _serverPlayerController.AttackRpc(receiver.NetworkObjectId);
                        }
                    }

                    // movement
                    else if (hit.transform.TryGetComponent<Terrain>(out var _))
                    {
                        _serverPlayerController.MoveRpc(hit.point);
                    }
                }
            }

            if (IsLocalPlayer && _followCamera != null)
            {
                _followCamera.Follow(transform);
            }
        }

        public void SetFollowCamera(IFollowCamera camera)
        {
            _followCamera = camera;
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            _serverPlayerController.TeleportRpc(position, rotation);
        }
    }
}
