using Level3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathFinder : MonoBehaviour
{
    public RectGrid rectGrid;

    public Queue<Vector2Int> wayPoints = new Queue<Vector2Int>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < rectGrid.transform.childCount; i++)
            {
                rectGrid.transform.GetChild(i).GetComponent<RectGridCell>().SetWalkable();
            }
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << 3);
            if (hit)
            {
                Vector2Int index = new((int)hit.point.x, (int)hit.point.y);
                SetDestination(Vector2Int.zero, index, rectGrid, rectGrid.pathFinder);
            }
        }
    }

    public void SetDestination(Vector2Int origin, Vector2Int destination, RectGrid grid, PathFinder pathFinder = null)
    {
        Debug.Log("Start to set destination");
        if (pathFinder.status != PathFinderStatus.RUNNING)
        {
            // clear all the waypoints.
            wayPoints.Clear();

            pathFinder.Init(origin, destination);
            StartCoroutine(Coroutine_PathFinding(pathFinder, grid));
        }
    }

    IEnumerator Coroutine_PathFinding(PathFinder pathFinder, RectGrid grid)
    {
        int pathFindCount = 0;
        while (pathFinder.status == PathFinderStatus.RUNNING)
        {
            pathFinder.Step(false);
            pathFindCount++;
            yield return null;
        }
        // completed pathfinding.
        if (pathFinder.status == PathFinderStatus.FAILURE)
        {
            Debug.Log("Failed finding a path. No valid path exists");
        }
        if (pathFinder.status == PathFinderStatus.SUCCESS)
        {
            Debug.Log("Found a path, success");
            //Debug.Log("Found a path, time taken: " + (Time.time - startTime));
            // found a valid path.
            // accumulate all the locations by traversing from goal to the start.
            List<Vector2Int> reversePathLocations = new List<Vector2Int>();
            PathFinderNode node = pathFinder.GetCurrentNode();
            while (node != null)
            {
                reversePathLocations.Add(node.location);
                node = node.parent;
            }
            for (int i = reversePathLocations.Count - 1; i >= 0; i--)
            {
                RectGridCell cell = rectGrid.transform.Find("cell_" + reversePathLocations[i].x + "_" + reversePathLocations[i].y).GetComponent<RectGridCell>();
                cell.SetNonWalkable();
            }
        }
    }
}
