using UnityEngine;

namespace Apps.RunTime.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform _player;

        void LateUpdate()
        {
            transform.position = _player.position;
        }
    }
}