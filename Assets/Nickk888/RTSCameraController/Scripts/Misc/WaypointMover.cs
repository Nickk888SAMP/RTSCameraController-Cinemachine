using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private WaypointManager waypointManager;
    [SerializeField] private float speed = 4;
    private int currentWaypoint;
    private Vector3 moveToPosition;

    void Start()
    {
        currentWaypoint = -1;
        MoveToNextWaypoint();
    }

    private void Update()
    {
        Vector3 direction = (moveToPosition - transform.position).normalized;
        transform.forward = direction;
        transform.Translate(direction * Time.deltaTime * speed, Space.World);

        if(Vector3.Distance(transform.position, moveToPosition) < 0.5f)
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        currentWaypoint = waypointManager.GetNextWaypoint(currentWaypoint, out moveToPosition);
    }
}
