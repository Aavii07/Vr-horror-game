using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public Transform GetPoint(int index)
    {
        return transform.GetChild(index);
    }

    public int PointCount
    {
        get { return transform.childCount; }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform point = transform.GetChild(i);

            Gizmos.DrawSphere(point.position, 0.3f);

            if (i + 1 < transform.childCount)
            {
                Transform next = transform.GetChild(i + 1);
                Gizmos.DrawLine(point.position, next.position);
            }
            else if (i + 1 == transform.childCount)
            {
                Transform next = transform.GetChild(0);
                Gizmos.DrawLine(point.position, next.position);
            }
        }
    }
}