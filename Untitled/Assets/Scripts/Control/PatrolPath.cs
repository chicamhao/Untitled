using UnityEngine;

namespace Apps.Runtime.Control
{
    /// <summary>
    /// The moveable scope of the guardian is specified by reference waypoints(path),
    /// the path which are attached to the environment(map), not the guardian ​themself.
    /// </summary>
	public sealed class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(transform.GetChild(i).position, 0.2f);
                Gizmos.DrawLine(
                    transform.GetChild(i).position, GetNextWayPoint(i));
            }
        }

        private Vector3 GetNextWayPoint(int currentIndex)
        {
            return transform.GetChild((int)Mathf.Repeat(currentIndex + 1, transform.childCount)).position;
        }

        public Vector3 GetFirstWayPoint()
        {
            return transform.GetChild(0).position;
        }

        public WayPoint GetWayPoint(Vector3 position)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                if (Vector3.Distance(position, transform.GetChild(i).position) <= 0.5f)
                {
                    return new WayPoint(true, GetNextWayPoint(i));
                }
            }

            return new WayPoint(false, Vector3.zero);
        }

        public readonly struct WayPoint
        {
            public readonly bool AtWayPoint { get; }
            public readonly Vector3 NextWayPoint { get; }

            public WayPoint(bool atWayPoint, Vector3 nextWayPoint)
            {
                AtWayPoint = atWayPoint;
                NextWayPoint = nextWayPoint;
            }
        }
    }
}