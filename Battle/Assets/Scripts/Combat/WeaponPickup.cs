using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
	public sealed class WeaponPickup : NetworkBehaviour
	{
		public Weapon Weapon => _weapon;
		[SerializeField] Weapon _weapon;
    }
}

