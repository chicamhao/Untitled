using Apps.RealTime.Combat;
using Apps.RealTime.Core;
using Apps.RealTime.Movement;
using UnityEngine;

namespace Apps.RealTime.Control
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] ActionScheduler _actionScheduler;
        [SerializeField] Mover _mover;
        [SerializeField] Fighter _fighter;

        void Update()
        {
            // TODO since it's physics, should be FixedUpdate
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray); // TODO none alloc

            if (InteractAttack(hits)) return;
            if (InteractMovement(hits)) return;
        }

        private bool InteractMovement(RaycastHit[] hits)
        {
            if (_mover == null) return false;

            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<Terrain>(out var _))
                {
                    if (Input.GetMouseButton(0))
                    {
                        _actionScheduler.StartAction(_mover);
                        _mover.MoveTo(hit.point);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool InteractAttack(RaycastHit[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<Receiver>(out var receiver))
                {
                    if (Input.GetMouseButton(0))
                    {
                        _actionScheduler.StartAction(_fighter);
                        _fighter.Attack(receiver);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
