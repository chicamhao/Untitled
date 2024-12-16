using Unity.Netcode;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
    public class Weapon : ScriptableObject
    {
        public AnimatorOverrideController AnimatorOnverride => _animatorOnverride;
        [SerializeField] AnimatorOverrideController _animatorOnverride;

        public GameObject WeaponPrefab => _weaponPrefab;
        [SerializeField] GameObject _weaponPrefab;

        public string HandBoneName => _handBoneName;
        [SerializeField] string _handBoneName = "Hand_R";

        public GameObject ProjectilePrefab;

        [Header("Runtime")]
        public float Range => _range;
        [SerializeField] float _range = 2f;

        public uint Damage => _damage;
        [SerializeField] uint _damage = 50;

        public float Duration => _duration;
        [SerializeField] float _duration = 1.5f;
    }
}
