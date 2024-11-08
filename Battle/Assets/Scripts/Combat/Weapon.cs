using Apps.Runtime.Core;
using UnityEngine;

namespace Apps.Runtime.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController _animatorOnverride;
        [SerializeField] GameObject _weaponPrefab;
        [SerializeField] string _handBoneName = "Hand_R";

        [Header("Runtime")]
        public float Range => _range;
        [SerializeField] float _range = 2f;

        public float Damage => _damage;
        [SerializeField] float _damage = 50f;

        public float Duration => _duration;
        [SerializeField] float _duration = 1.5f;

        public void Spawn(Transform root, Animator animator)
        {
            Instantiate(_weaponPrefab, AlgorithmHelper.RecursiveFindChild(root, _handBoneName));
            animator.runtimeAnimatorController = _animatorOnverride;           
        }
    }
}
