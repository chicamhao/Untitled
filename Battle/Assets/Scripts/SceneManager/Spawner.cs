using Apps.Runtime.Control;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.SceneManager
{
    public sealed class Spawner : MonoBehaviour
    {
        [SerializeField] Transform _appearancePosition;

        void Start()
        {
            if (NetworkManager.Singleton == null)
                throw new System.Exception("Requiring initialization from boot.scene");

            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            player.GetComponent<PlayerController>().Inititalize(_appearancePosition.position, Camera.main);            
        }
    }
}