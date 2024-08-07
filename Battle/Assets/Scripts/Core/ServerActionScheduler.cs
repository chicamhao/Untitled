using UnityEngine;

namespace Apps.Runtime.Core
{
    public interface IAction
    {
        void Cancel();
    }

    public sealed class ServerActionScheduler : MonoBehaviour
    {
        IAction _currentAction;

        /// <summary>
        /// allow overriding of actions 
        /// </summary>
        /// <param name="action"></param>
        public bool StartAction(IAction action)
        {
            // TODO priority judgment?
            if (_currentAction == action) return false;
            if (_currentAction != null)
            {
                _currentAction.Cancel();
            }
            _currentAction = action;
            return true;
        }
    }
}