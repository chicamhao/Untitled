using Apps.Runtime.Combat;
using Apps.Runtime.Core;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Control
{
    public sealed class ServerPlayerController : NetworkBehaviour
	{
		ServerActionScheduler _actionScheduler;
		ServerMover _mover;
		ServerFighter _fighter;

        public override void OnNetworkSpawn()
        {
			if (!IsServer)
			{
				enabled = false;
				return;
			}

			_actionScheduler = GetComponent<ServerActionScheduler>();
			_mover = GetComponent<ServerMover>();
			_fighter = GetComponent<ServerFighter>();
        }

        [Rpc(SendTo.Server)]
		public void MoveRpc(Vector3 destination)
		{
			_actionScheduler.StartAction(_mover);
			_mover.MoveTo(destination);
		}

		[Rpc(SendTo.Server)]
		public void AttackRpc(ulong receiverObjectId)
		{
			var receiver = NetworkManager.SpawnManager.SpawnedObjects[receiverObjectId]
				.GetComponent<ServerReceiver>(); // TODO cache reference
            _fighter.Attack(receiver);
			_actionScheduler.StartAction(_fighter);
		}

		[Rpc(SendTo.Server)]
        public void TeleportRpc(Vector3 position, Quaternion rotation)
        {
			_mover.Teleport(position, rotation);
        }
    }
}
