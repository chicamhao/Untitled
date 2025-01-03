using Apps.Runtime.Combat;
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
        bool _interactRequest;

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
                    else if (hit.transform.TryGetComponent<Terrain>(out var _)
                        || hit.transform.TryGetComponent<WeaponPickup>(out var _))
                    {
                        _serverPlayerController.MoveRpc(hit.point);
                    }
                }
            }
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            _serverPlayerController.TeleportRpc(position, rotation);
        }
    }
}