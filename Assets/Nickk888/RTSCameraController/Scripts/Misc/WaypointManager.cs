using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private void OnDrawGizmos() 
    {
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; i ++)
        {
            // First Child
            Transform child = transform.GetChild(i);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(child.position, 1);

            // Second Child
            Transform child2 = null;
            if(i < (childCount - 1))
            {
                child2 = transform.GetChild(i + 1);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(child.position, child2.position);
            }
        }   
    }

    public int GetNextWaypoint(int currentWaypointIndex, out Vector3 position)
    {
        int childCount = transform.childCount;
        if(currentWaypointIndex < (childCount - 1))
        {
            currentWaypointIndex++;
        }
        else
        {
            currentWaypointIndex = 0;
        }
        Transform child = transform.GetChild(currentWaypointIndex);
        position = child.position;
        return currentWaypointIndex;
    }
}
