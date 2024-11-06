using Apps.Runtime.Control;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Sandbox
{
    public sealed class DebugSpawner : MonoBehaviour
    {
        void Start()
        {
            NetworkManager.Singleton.StartHost();
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            player.GetComponent<PlayerController>().Inititalize(transform.position, Quaternion.identity, Camera.main);
        }
    }
}