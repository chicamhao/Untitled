using Apps.Runtime.Combat;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Sandbox
{
    public sealed class DebugSpawner : MonoBehaviour
    {
        public void Start()
        {
            NetworkManager.Singleton.StartHost();
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            player.GetComponent<Status>().Initialize(1000);
            player.GetComponent<ServerMover>().Teleport(transform.position, transform.rotation);
        }
    }
}