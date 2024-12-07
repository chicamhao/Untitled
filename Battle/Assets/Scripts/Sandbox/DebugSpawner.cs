using Apps.Runtime.Control;
using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Sandbox
{
    public sealed class DebugSpawner : MonoBehaviour, IFollowCamera
    {
        public void Start()
        {
            NetworkManager.Singleton.StartHost();
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var control = player.GetComponent<PlayerController>();
            control.SetFollowCamera(this);
        }

        public void Follow(Transform player)
        {
            transform.position = player.position;
        }
    }
}