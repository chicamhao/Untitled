using UnityEngine;
using UnityEngine.AI;

public sealed class Mover : MonoBehaviour
{
    [SerializeField] GameObject _target;
    [SerializeField] NavMeshAgent _navMeshAgent;

    private void Update()
    {
        _navMeshAgent.SetDestination(_target.transform.position);
    }
}
