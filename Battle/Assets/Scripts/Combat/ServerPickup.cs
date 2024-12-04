using Apps.Runtime.Core;
using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    public sealed class ServerPickup : NetworkBehaviour
	{
        [SerializeField] Transform _rootTransform;

        ServerFighter _fighter;
        Animator _animator;

        GameObject _handedWeapon;
        Weapon _weapon;

        private void Start()
        {
            _fighter = GetComponent<ServerFighter>();
            _animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pickup"))
            {
                if (_handedWeapon != null)
                {
                    _handedWeapon.SetActive(false); // TODO pool
                }

                _weapon = other.GetComponent<WeaponPickup>().Weapon;
                _handedWeapon = Instantiate(_weapon.WeaponPrefab, AlgorithmHelper.RecursiveFindChild(_rootTransform, _weapon.HandBoneName));

                // change weapon stats
                _fighter.ChangeWeapon(_weapon);

                // change attack animation
                _animator.runtimeAnimatorController = _weapon.AnimatorOnverride;
                
                other.gameObject.SetActive(false); // TODO pool
            }
        }
    }
}

