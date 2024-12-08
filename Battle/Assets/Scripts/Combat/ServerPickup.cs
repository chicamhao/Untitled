using Apps.Runtime.Core;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    public sealed class ServerPickup : NetworkBehaviour
	{
        [SerializeField] Transform _rootTransform;
        [SerializeField] Weapon _initialWeapon;

        ServerFighter _fighter;
        Animator _animator;

        Weapon _weapon;
        GameObject _handedWeapon;

        public override void OnNetworkSpawn()
        {
            enabled = IsServer;
        }

        private void Start()
        {
            _fighter = GetComponent<ServerFighter>();
            _animator = GetComponent<Animator>();

            if (_initialWeapon != null)
            {
                HandWeapon(_initialWeapon);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsServer && other.CompareTag("Pickup"))
            {
                HandWeapon(other.GetComponent<WeaponPickup>().Weapon);            
                other.gameObject.SetActive(false); // TODO pool
            }
        }

        private void HandWeapon(Weapon weapon)
        {
            if (_handedWeapon != null)
            {
                _handedWeapon.SetActive(false); // TODO pool
            }

            _weapon = weapon;
            var handTransform = AlgorithmHelper.RecursiveFindChild(_rootTransform, _weapon.HandBoneName);
            _handedWeapon = Instantiate(_weapon.WeaponPrefab, handTransform);

            // change weapon stats
            _fighter.ChangeWeapon(_weapon, handTransform);

            // change attack animation
            _animator.runtimeAnimatorController = _weapon.AnimatorOnverride;
        }
    }
}

