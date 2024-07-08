using UnityEngine;
using UnityEngine.AI;

public sealed class Mover : MonoBehaviour
{
    [SerializeField] GameObject _target;
    [SerializeField] NavMeshAgent _navMeshAgent;
    [SerializeField] Animator _animator;

    private void Update()
     {
        UpdateMovement();
        UpdateAnimator();
    }

    private void UpdateMovement()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {  
                _navMeshAgent.SetDestination(hitInfo.point);
            }
        }
    }

    private void UpdateAnimator()
    {
        // transform velocity direction from gobal space to local space
        var velocity = transform.InverseTransformDirection(_navMeshAgent.velocity);

        // forward direction
        _animator.SetFloat("_forwardSpeed", Mathf.Abs(velocity.z));
    }
}
