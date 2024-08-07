using UnityEngine;
using Unity.Netcode;
using Apps.Runtime.Movement;
using Apps.Runtime.Core;
using System;
using Unity.Netcode.Components;

namespace Apps.Runtime.Control
{
	public sealed class ServerPlayerController : NetworkBehaviour
	{
		[SerializeField] ServerActionScheduler _actionScheduler;
		[SerializeField] ServerMover _mover;

        public override void OnNetworkSpawn()
        {
			if (!IsServer)
			{
				enabled = false;
				return;
			}
        }

        [Rpc(SendTo.Server)]
		public void RequestMoveRpc(Vector3 destination)
		{
			_actionScheduler.StartAction(_mover);
			_mover.MoveTo(destination);
		}

		[Rpc(SendTo.Server)]
        public void TeleportRpc(Vector3 position)
        {
			_mover.Teleport(position);
        }
    }

}
