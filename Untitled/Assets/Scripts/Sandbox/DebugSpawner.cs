using Apps.Runtime.Combat;
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
        }
    }
}