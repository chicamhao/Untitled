using Apps.Runtime.Control;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Sandbox
{
    public sealed class DebugSpawner : MonoBehaviour, IFollowCamera
    {
        void Start()
        {
            NetworkManager.Singleton.StartHost();
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            player.GetComponent<PlayerController>().Inititalize(transform.position, Quaternion.identity, this);
        }

        public void Follow(Transform player)
        {
            transform.position = player.position;
        }
    }
}