using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
	public sealed class WeaponPickup : NetworkBehaviour
	{
		[SerializeField] Weapon _weapon;

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            if (other.CompareTag("Player"))
            {
                other.GetComponent<ServerFighter>().Pickup(_weapon);
                gameObject.SetActive(false); // TODO pool
            }
        }
    }
}

