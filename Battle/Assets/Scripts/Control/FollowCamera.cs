using Apps.Runtime.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Control
{
    public sealed class FollowCamera : MonoBehaviour, IFollowCamera
    {
        [SerializeField] Transform _appearancePosition;

        void Start()
        {
            if (NetworkManager.Singleton == null)
                throw new System.Exception("Requiring initialization from boot.scene");

            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            player.GetComponent<PlayerController>().SetFollowCamera(this);            
        }

        public void Follow(Transform player)
        {
            transform.position = player.position;
        }
    }
}