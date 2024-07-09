using UnityEngine;

namespace Apps.RealTime.Core
{
	public interface IAction
	{
		void Cancel();
	}

	public sealed class ActionScheduler : MonoBehaviour
	{
		IAction _currentAction;

		/// <summary>
		/// allow overriding of actions 
		/// /// </summary>
		/// <param name="action"></param>
		public void StartAction(IAction action)
		{
			// TODO priority judgment?
			if (_currentAction == action) return;
            if (_currentAction != null)
			{
				_currentAction.Cancel();
			}
			_currentAction = action;
		}
	}
}

