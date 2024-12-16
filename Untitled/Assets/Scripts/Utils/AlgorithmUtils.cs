using UnityEngine;

namespace Apps.Utils
{
	public static class AlgorithmUtils
	{
        // TODO use for temporary purposes, since this backtracking approach's expensive
        public static Transform BacktrackFindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                else
                {
                    Transform found = BacktrackFindChild(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }
    }
}