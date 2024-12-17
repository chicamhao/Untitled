using Apps.Runtime.Control;
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

                const uint maxHp = 1000;
                var status = player.GetComponent<Status>();
                status.Initialize("PLAYER " + player.OwnerClientId.ToString(), maxHp);
                status.HP.Value = maxHp;

                player.GetComponent<ServerMover>().Teleport(transform.position, transform.rotation);
                player.GetComponent<CharacterPresenter>().Initialize(Camera.main);

                foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<ServerAIController>().Initialize(new[] { player.gameObject });
                }
            }
        }
    }
}