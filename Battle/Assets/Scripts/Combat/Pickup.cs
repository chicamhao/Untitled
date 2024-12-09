using Apps.Runtime.Core;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    public sealed class Pickup : NetworkBehaviour
	{
        [SerializeField] Transform _rootTransform;
        [SerializeField] Weapon _initialWeapon;

        ServerFighter _fighter;
        Animator _animator;

        Weapon _weapon;
        GameObject _handedWeapon;

        private void Start()
        {
            _fighter = GetComponent<ServerFighter>();
            _animator = GetComponent<Animator>();
            HandWeapon(_initialWeapon);            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pickup"))
            {
                HandWeapon(other.GetComponent<WeaponPickup>().Weapon);            
                other.gameObject.SetActive(false); // TODO pool
            }
        }

        private void HandWeapon(Weapon weapon)
        {
            if (_handedWeapon != null)
            {
                Destroy(_handedWeapon); // TODO pool
            }

            _weapon = weapon;
            var handTransform = AlgorithmHelper.RecursiveFindChild(_rootTransform, _weapon.HandBoneName);

            if (_weapon.WeaponPrefab != null)
            {
                _handedWeapon = Instantiate(_weapon.WeaponPrefab, handTransform);
            }

            // change attack animation
            if (_weapon.AnimatorOnverride != null)
            {
                _animator.runtimeAnimatorController = _weapon.AnimatorOnverride;
            }

            // change weapon stats
            _fighter.ChangeWeapon(_weapon, handTransform);
        }
    }
}

