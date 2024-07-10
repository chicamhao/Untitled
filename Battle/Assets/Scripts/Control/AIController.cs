using UnityEngine;
using Apps.RealTime.Core;
using Apps.RealTime.Combat;
using Apps.RealTime.Movement;	

public sealed class AIController : MonoBehaviour
{
	Fighter _fighter;
	Receiver _receiver;

	void Start()
	{
		_fighter = GetComponent<Fighter>();

        // TODO multiplayer
        _receiver = GameObject.FindWithTag("Player").GetComponent<Receiver>();
	}

	void Update()
	{
		if (InChasingRange())
		{
			_fighter.Attack(_receiver);
        }
		else
		{
			_fighter.Cancel();
		}
	}

	private bool InChasingRange()
	{
		var distance = Vector3.Distance(gameObject.transform.position, _receiver.transform.position);
		return distance <= Configurations.AIChasingRange;
	}
}

