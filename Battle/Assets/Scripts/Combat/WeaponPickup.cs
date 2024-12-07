using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
	public sealed class WeaponPickup : NetworkBehaviour
	{
		public Weapon Weapon => _weapon;
		[SerializeField] Weapon _weapon;
        [SerializeField] float _rotationAngel = 2f;

        public void Update()
        {
            transform.Rotate(Vector3.up, _rotationAngel, Space.Self);
        }
    }
}

