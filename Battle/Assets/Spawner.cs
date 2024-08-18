using Apps.Runtime.Control;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.SceneManager
{
    public sealed class Spawner : MonoBehaviour
    {
        [SerializeField] Transform _appearancePosition;

        NetworkManager _networkManager;

        void Start()
        {
            _networkManager = FindFirstObjectByType<NetworkManager>();
            
            var player = _networkManager.SpawnManager.GetLocalPlayerObject();
            player.GetComponent<PlayerController>().Inititalize(_appearancePosition.position, Camera.main);            
        }
    }
}