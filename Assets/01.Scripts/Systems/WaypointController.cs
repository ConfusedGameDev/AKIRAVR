using KVRL.KVRLENGINE.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : SingletonComponent<WaypointController>
{
    public   List<Transform> waypoints;

    private   void OnDrawGizmos()
    {
      
        if(waypoints.Count>1)
        for (int i = 1; i < waypoints.Count; i++)
        {
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
        }
        if (waypoints.Count > 2)
            Gizmos.DrawLine(waypoints[waypoints.Count-1].position, waypoints[0].position);

    }
    public   Transform getClosestWaypoint(Transform Agent)
    {
        var minDist = Vector3.Distance(Agent.position, waypoints[0].position);
        int closestTargetIndex= 0;
        for (int i = 1; i < waypoints.Count; i++)
        {
            var currentDist= Vector3.Distance(Agent.position, waypoints[1].position);
            if(currentDist<minDist)
            {
                closestTargetIndex = i;
                minDist = currentDist;
            }
        }
        return waypoints[closestTargetIndex];
    }
    public Transform getNextWaypoint(Transform wp)
    {
        if(waypoints.Contains(wp))
        {
            var nextIndex = waypoints.IndexOf(wp)+1;
            nextIndex = nextIndex >= waypoints.Count ? 0 : nextIndex;
            return waypoints[nextIndex];
        }
        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
