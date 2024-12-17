using Apps.Runtime.Data;
using Apps.Runtime.Movement;
using Apps.Runtime.Presentation;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Sandbox
{
    public sealed class DebugSpawner : MonoBehaviour
    {
        public void Start()
        {
            if (NetworkManager.Singleton.StartHost())
            {
                var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

                player.GetComponent<Status>().Initialize("PLAYER " + player.OwnerClientId.ToString(), 1000);
                player.GetComponent<ServerMover>().Teleport(transform.position, transform.rotation);
                player.GetComponent<CharacterPresenter>().Initialize(Camera.main);
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}