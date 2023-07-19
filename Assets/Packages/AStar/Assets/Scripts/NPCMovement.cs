using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Level3;

public class NPCMovement : MonoBehaviour
{
    LineRenderer lr;

    public float speed = 1f;

    //  A queue of waypoints.
    public Queue<Vector2Int> wayPoints = new Queue<Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        StartCoroutine(Coroutine_MoveTo());
    }

    public void AddWayPoint(Vector2Int point)
    {
        wayPoints.Enqueue(point);
    }

    public void SetDestination(Vector2Int destination, RectGrid grid, PathFinder pathFinder = null)
    {
        if (pathFinder != null)
        {
            // With pathfinding.
            // 1. If pathfinder is running dont do anything.
            // 2. while pathfinder is running
            // 2.a perform one iteration of the loop.
            // 2.b continue until a success or failure is returned.
            if (pathFinder.status != PathFinderStatus.RUNNING)
            {
                // clear all the waypoints.
                wayPoints.Clear();
                Vector2Int start = Vector2Int.zero;
                start.x = (int)transform.position.x;
                start.y = (int)transform.position.y;

                pathFinder.Init(start, destination);
                grid.ResetCellColors();
                // Start a coroutine to do go to loop the pathfinding steps.
                StartCoroutine(Coroutine_PathFinding(pathFinder, grid));
            }
        }
        else
        {
            // Without pathfinding.
            // we do not have pathfinding yet.
            // For now we just add the destination point to the waypoint queue.
            AddWayPoint(destination);
        }
    }

    IEnumerator Coroutine_PathFinding(PathFinder pathFinder, RectGrid grid)
    {
        while (pathFinder.status == PathFinderStatus.RUNNING)
        {
            pathFinder.Step(true);
            yield return null;
        }
        // completed pathfinding.

        if (pathFinder.status == PathFinderStatus.FAILURE)
        {
            Debug.Log("Failed finding a path. No valid path exists");
        }
        if (pathFinder.status == PathFinderStatus.SUCCESS)
        {
            // found a valid path.
            // accumulate all the locations by traversing from goal to the start.
            List<Vector2Int> reversePathLocations = new List<Vector2Int>();
            PathFinderNode node = pathFinder.GetCurrentNode();
            while (node != null)
            {
                reversePathLocations.Add(node.location);
                node = node.parent;
            }
            // add all these points to the waypoints.
            for (int i = reversePathLocations.Count - 1; i >= 0; i--)
            {
                AddWayPoint(reversePathLocations[i]);
                grid.SetCellColor(reversePathLocations[i], grid.COLOR_PATH);
            }
        }
    }

    IEnumerator Coroutine_MoveTo()
    {
        while (true)
        {
            while (wayPoints.Count > 0)
            {
                yield return StartCoroutine(Coroutine_MoveToPoint(wayPoints.Dequeue(), speed));
            }
            yield return null;
        }
    }

    IEnumerator Coroutine_MoveToPoint(Vector2Int p, float speed)
    {
        lr.SetPosition(lr.positionCount++, new Vector3(p.x, p.y, 0));
        yield return null;
        //// Vector3 endP = new Vector3(p.x, p.y, 0.0f);
        //Vector2 startP = new Vector2(transform.position.x, transform.position.y);

        //float distance = Vector2.Distance(p, startP);
        //float totalTime = distance / speed;
        //float elaspedTime = 0f;

        //while (elaspedTime < totalTime)
        //{
        //    float t = Mathf.Clamp01(elaspedTime / totalTime);
        //    Vector2 pos = Vector2.Lerp(startP, p, t);
        //    transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        //    //Debug.Log(elaspedTime);
        //    elaspedTime += Time.deltaTime;
        //    yield return new WaitForEndOfFrame();
        //}
        //transform.position = new Vector3(p.x, p.y, transform.position.z);
    }
}