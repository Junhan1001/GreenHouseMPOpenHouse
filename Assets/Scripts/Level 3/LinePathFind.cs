using Level3;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LinePathFind : MonoBehaviour
{
    #region Variables
    public RectGrid rectGrid;

    public bool finishedLevel = false;

    [Header("Layer Mask")]
    private int gridLayer = 1 << 3;
    private int lineLayer = 1 << 8;

    public List<Transform> secretPoints = new();

    [SerializeField]
    private Transform fromT = null;
    [SerializeField]
    private Transform toT = null;

    public Queue<Vector2Int> wayPoints = new Queue<Vector2Int>();

    private List<Vector2Int> zeros = new List<Vector2Int>() { Vector2Int.zero, Vector2Int.zero };

    public GameObject line;

    [SerializeField]
    private Vector2 lineStartPoint;
    [SerializeField]
    private Vector2 lineEndPoint;
    private Transform lineFrom;
    private Transform lineTo;

    private Transform changedFrom;
    private Transform changedTo;

    private bool hitPlaceholder = false;
    [SerializeField]
    private bool findingPath = false;

    [SerializeField]
    private GameObject imgFindingPath;

    [SerializeField]
    private GameObject imgError;

    [SerializeField]
    private GameObject reverseInstruction;
    private float reverseInstructionActiveTime = 0f;

    private float startTime;
    private bool firstTimeDrawLine = true;

    public bool typeOfLineSelected = false;
    public Color colorOfLineSelected;

    public Sprite transparentSprite;

    public Image imgComponentFrom;
    public Image imgComponentTo;
    public Image imgColorSelected;

    public GameObject selectColorPopUp;
    public Hover hoverTab;

    [SerializeField]
    private List<RectGridCell> affectedCellsList = new();
    private Dictionary<GameObject, List<RectGridCell>> previousDrawnLineDict = new();
    [SerializeField]
    private List<List<LineLimit>> previousDrawnLineFromAndTo = new();
    [SerializeField]
    private List<LineLimit> lastLineDrawn = new();
    [SerializeField]
    private List<Vector2> newReversePathLocations = new List<Vector2>();
    #endregion

    private void Awake()
    {
        imgComponentFrom.sprite = transparentSprite;
        imgComponentTo.sprite = transparentSprite;
        enabled = false;
    }

    private void OnEnable()
    {
        fromT = null;
        toT = null;
        imgComponentFrom.sprite = transparentSprite;
        imgComponentTo.sprite = transparentSprite;
        imgColorSelected.sprite = transparentSprite;
    }

    private void OnDisable()
    {
        previousDrawnLineDict.Clear();
        previousDrawnLineFromAndTo.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (reverseInstruction.activeSelf)
        {
            reverseInstructionActiveTime += Time.deltaTime;
            if (reverseInstructionActiveTime > 5f)
            {
                reverseInstruction.SetActive(false);
                reverseInstructionActiveTime = 0f;
            }
        }
        if (findingPath)
            return;
#if UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
#endif
#if UNITY_ANDROID
        if (InputSystem.Instance.LeftClick())
#endif
        {
#if UNITY_STANDALONE
            Vector3 touchPosition = Input.mousePosition;
#endif
#if UNITY_ANDROID
            Vector3 touchPosition = Input.GetTouch(0).position;    
#endif
            if (finishedLevel)
                return;
            if (reverseInstruction.activeSelf)
            {
                reverseInstruction.SetActive(false);
            }

            // If the player clicks on a line, reverse the line
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out RaycastHit hit3D, lineLayer))
            {
                hit3D.transform.GetComponent<DrawLine>()?.ReverseLine();
            }

            /// When the player clicks, checks raycast to see if it hits anything, ignoring the grid layer
            /// When it did not hit components,
            /// check if it hits placeholder (in some case, the raycast will detect the placeholder instead of the component)
            /// If it hits a placeholder and there is a component in it, set the component as fromT or toT (when fromT is already set and toT is not)
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchPosition), Vector2.zero, Mathf.Infinity, ~gridLayer);
            if (!hit)
                return;
            if (!hit.transform.CompareTag("Component"))
            {
                if (hit.transform.CompareTag("Selection") && hit.transform.childCount != 0)
                {
                    Debug.Log("hit placeholder");
                    hitPlaceholder = true;
                }
                else
                {
                    Debug.Log("Hitting " + hit.transform.name + ". Please select a component");
                    return;
                }
            }
            if (!fromT && !toT)
            {
                if (hitPlaceholder)
                {
                    fromT = hit.transform.GetChild(0);
                    imgComponentFrom.sprite = fromT.GetComponent<SpriteRenderer>().sprite;
                    hitPlaceholder = false;
                }
                else
                {
                    fromT = hit.transform;
                    imgComponentFrom.sprite = fromT.GetComponent<SpriteRenderer>().sprite;
                }
                // When no type of line selected, not allow to choose from and to
                if (!typeOfLineSelected)
                {
                    selectColorPopUp.SetActive(true);
                    hoverTab.SetHoverTab = true;
                    fromT = null;
                    toT = null;
                    imgComponentFrom.sprite = transparentSprite;
                    imgComponentTo.sprite = transparentSprite;
                    return;
                }
            }
            else if (fromT && !toT)
            {
                if (hitPlaceholder)
                {
                    toT = hit.transform.GetChild(0);
                    imgComponentTo.sprite = toT.GetComponent<SpriteRenderer>().sprite;
                    hitPlaceholder = false;
                }
                else
                {
                    toT = hit.transform;
                    imgComponentTo.sprite = toT.GetComponent<SpriteRenderer>().sprite;
                }
                if (toT == fromT)
                {
                    Debug.Log("Please select a different component");
                    toT = null;
                    imgComponentTo.sprite = transparentSprite;
                    return;
                }
                if (!typeOfLineSelected)
                {
                    selectColorPopUp.SetActive(true);
                    hoverTab.SetHoverTab = true;
                    toT = null;
                    imgComponentTo.sprite = transparentSprite;
                    return;
                }
                // When from and to are selected, find the nearest nodes of them
                List<Vector2Int> nearestNodes = GetNearestNode();
                // If there is no nodes found, show error and reset all the variables and return
                if (nearestNodes[0] == zeros[0] || nearestNodes[1] == zeros[1])
                {
                    Debug.Log("No nodes found.");
                    //if (previousDrawnLineFromAndTo.Count > 0)
                    //{
                    //    previousDrawnLineFromAndTo.RemoveAt(previousDrawnLineFromAndTo.Count - 1);
                    //}
                    StartCoroutine(ShowError());
                    if (changedFrom)
                        changedFrom.GetComponent<LineLimit>().AllowDrawLine = true;
                    if (changedTo)
                        changedTo.GetComponent<LineLimit>().AllowDrawLine = true;
                    changedFrom = null;
                    changedTo = null;
                    fromT = null;
                    toT = null;
                    imgComponentFrom.sprite = transparentSprite;
                    imgComponentTo.sprite = transparentSprite;
                    return;
                }
                else
                {
                    // Start to find path with the from node and to node
                    SetDestination(nearestNodes[0], nearestNodes[1], rectGrid, rectGrid.pathFinder);
                    fromT = null;
                    toT = null;
                }
            }
        }
    }

    private IEnumerator ShowError()
    {
        imgError.SetActive(true);
        imgError.GetComponent<PopUp>().timer = 2f;
        yield break;
        //yield return StartCoroutine(CloseErrorImg());
    }

    private IEnumerator CloseErrorImg()
    {
        float time = 0f;
        yield return new WaitForNextFrameUnit();
        while (true)
        {
            {
#if UNITY_STANDALONE
                if (Input.GetMouseButtonDown(0))
#endif
#if UNITY_ANDROID
                if (InputSystem.Instance.LeftClick())
#endif
                {
                    imgError.SetActive(false);
                    yield break;
                }
                time += Time.deltaTime;
                if (time >= 2f)
                {
                    imgError.SetActive(false);
                    yield break;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// Calculate the nearest nodes.
    /// Return a list of Vector2Int, the first one is the nearest node of from, the second one is the nearest node of to
    /// </summary>
    /// <returns></returns>
    private List<Vector2Int> GetNearestNode()
    {
        // Get the two nearest connection points of from and to
        List<Transform> nearestTwoPoints = CalculateNearestConnectionPoints(fromT, toT);
        if (!nearestTwoPoints[0] || !nearestTwoPoints[1])
        {
            Debug.Log("No nearest points found.");
            return zeros;
        }
        // Vector2 fromPoint is the nearest connection point of from
        Vector2 fromPoint = (Vector2)nearestTwoPoints[0].position;
        // fromPointInt is the nearest connection point of from in Vector2Int
        Vector2Int fromPointInt = new Vector2Int(Mathf.RoundToInt(nearestTwoPoints[0].position.x), Mathf.RoundToInt(nearestTwoPoints[0].position.y));
        // Get the neighbours of fromPoint
        List<Vector2Int> fromPointNeighbours = rectGrid.GetNeighbours(fromPointInt, true);

        // Similar to from
        Vector2 toPoint = (Vector2)nearestTwoPoints[1].position;
        Vector2Int toPointInt = new Vector2Int(Mathf.RoundToInt(nearestTwoPoints[1].position.x), Mathf.RoundToInt(nearestTwoPoints[1].position.y));
        List<Vector2Int> toPointNeighbours = rectGrid.GetNeighbours(toPointInt, true);

        /// distanceFrom is the float distance between fromPoint and nearest neighbour Vector2Int of fromPoint,
        /// this is to make sure that the distance will be the smallest as
        /// distanceFromInt is the int distance between fromPoint and nearest neighbour Vector2Int of fromPoint,
        /// distanceFromInt may be the same for the four points around fromPoint,
        /// so we need to use distanceFrom to make sure we are choosing the smallest
        float distanceFrom = 10000f;
        float distanceFromInt = 10000f;
        Vector2Int nearestPointFrom = Vector2Int.zero;
        foreach (Vector2Int point in fromPointNeighbours)
        {
            // Check if the point cell is walkable
            if (!rectGrid.transform.Find("cell_" + point.x + "_" + point.y).GetComponent<RectGridCell>().isWalkable)
            {
                continue;
            }
            // Calculate the distance between points
            if (Vector2Int.Distance(fromPointInt, point) <= distanceFromInt)
            {
                if (Vector2.Distance(fromPoint, point) < distanceFrom)
                {
                    distanceFromInt = Vector2Int.Distance(fromPointInt, point);
                    distanceFrom = Vector2.Distance(fromPoint, point);
                    nearestPointFrom = point;
                }
            }
        }
        // When no nearest point found
        if (nearestPointFrom == Vector2Int.zero)
        {
            Debug.Log("Can't find a nearest point to start, try to get more neighbours");
            return new List<Vector2Int>() { Vector2Int.zero, Vector2Int.zero };
        }

        // Similar to from
        float distanceTo = 10000f;
        float distanceToInt = 10000f;
        Vector2Int nearestPointTo = Vector2Int.zero;
        foreach (Vector2Int point in toPointNeighbours)
        {
            if (!rectGrid.transform.Find("cell_" + point.x + "_" + point.y).GetComponent<RectGridCell>().isWalkable)
            {
                continue;
            }
            if (Vector2Int.Distance(toPointInt, point) <= distanceToInt)
            {
                if (Vector2.Distance(toPoint, point) < distanceTo)
                {
                    distanceToInt = Vector2Int.Distance(toPointInt, point);
                    distanceTo = Vector2.Distance(toPoint, point);
                    nearestPointTo = point;
                }
            }
        }
        if (nearestPointTo == Vector2Int.zero)
        {
            Debug.Log("Can't find a nearest point to start, try to get more neighbours");
            return new List<Vector2Int>() { Vector2Int.zero, Vector2Int.zero };
        }
        return new List<Vector2Int>() { nearestPointFrom, nearestPointTo };
    }

    /// <summary>
    /// Calculate two nearest points between two given transforms.
    /// Return a list of two transforms, the first one is the nearest point of from, the second one is the nearest point of to
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private List<Transform> CalculateNearestConnectionPoints(Transform from, Transform to)
    {
        /// This will not only calculate the nearest connection points of from and to,
        /// and also consider if the point is nearer to the secret point
        /// Example: a component has two connection points (a, b), if the nearer point from this component to the other is point a,
        /// but there is a secret point nearby point b (distance between point b and secret point is shorter than distance between point a to the other),
        /// then the point b will be the nearest point

        // Create two lists to store all the connection points of from and to
        List<Transform> fromConnectionPoints = new();
        List<Transform> toConnectionPoints = new();

        for (int i = 0; i < from.childCount; i++)
        {
            // add all the child connection points to the list.
            fromConnectionPoints.Add(from.GetChild(i));
        }

        for (int i = 0; i < to.childCount; i++)
        {
            toConnectionPoints.Add(to.GetChild(i));
        }
        // initial distance
        float distanceBetweenFromAndSecret = 10000f;
        float distanceBetweenToAndSecret = 10000f;
        float distance = 10000f;
        Transform nearestFromPointFromSecret = null;
        Transform nearestToPointFromSecret = null;
        Transform fromPoint = null;
        Transform toPoint = null;
        for (int i = 0; i < fromConnectionPoints.Count; i++)
        {
            for (int j = 0; j < toConnectionPoints.Count; j++)
            {
                // if the connection point is already with line drawn, skip it.
                if (!fromConnectionPoints[i].GetComponent<LineLimit>().AllowDrawLine ||
                    !toConnectionPoints[j].GetComponent<LineLimit>().AllowDrawLine)
                    continue;
                for (int k = 0; k < secretPoints.Count; k++)
                {
                    float newDistanceBetweenFromAndSecret = Vector2.Distance(fromConnectionPoints[i].position, secretPoints[k].position);
                    if (newDistanceBetweenFromAndSecret < distanceBetweenFromAndSecret)
                    {
                        distanceBetweenFromAndSecret = newDistanceBetweenFromAndSecret;
                        nearestFromPointFromSecret = fromConnectionPoints[i];
                    }
                    float newDistanceBetweenToAndSecret = Vector2.Distance(toConnectionPoints[j].position, secretPoints[k].position);
                    if (newDistanceBetweenToAndSecret < distanceBetweenToAndSecret)
                    {
                        distanceBetweenToAndSecret = newDistanceBetweenToAndSecret;
                        nearestToPointFromSecret = toConnectionPoints[j];
                    }
                }
                float newDist = Vector2.Distance(fromConnectionPoints[i].position, toConnectionPoints[j].position);
                if (newDist < distance)
                {
                    // if the new distance is smaller, update the distance and the points.
                    distance = newDist;
                    fromPoint = fromConnectionPoints[i];
                    toPoint = toConnectionPoints[j];
                }
            }
        }
        if (distanceBetweenFromAndSecret < distance)
        {
            fromPoint = nearestFromPointFromSecret;
        }
        if (distanceBetweenToAndSecret < distance)
        {
            toPoint = nearestToPointFromSecret;
        }
        
        if (fromPoint && toPoint)
        {
            Debug.Log($"From point: {fromPoint.position} and to point: {toPoint.position}" +
            $"Distance between from point to secret is: {distanceBetweenFromAndSecret} and Distance between to point to secret is: {distanceBetweenToAndSecret}" +
            $"Distance: {distance}");
            // set the two nearest connection points to be not allow draw line, then return the two points transform
            changedFrom = fromPoint;
            changedTo = toPoint;
            changedFrom.GetComponent<LineLimit>().AllowDrawLine = false;
            changedTo.GetComponent<LineLimit>().AllowDrawLine = false;
            lineStartPoint = fromPoint.position;
            lineEndPoint = toPoint.position;
            lineFrom = fromPoint;
            lineTo = toPoint;
            previousDrawnLineFromAndTo.Add(new() { fromPoint.GetComponent<LineLimit>(), toPoint.GetComponent<LineLimit>() });
        }
        return new List<Transform>() { fromPoint, toPoint };
    }

    public void SetDestination(Vector2Int origin, Vector2Int destination, RectGrid grid, PathFinder pathFinder = null)
    {
        if (pathFinder.status != PathFinderStatus.RUNNING)
        {
                // clear all the waypoints.
                wayPoints.Clear();

                pathFinder.Init(origin, destination);
                // Start a coroutine to do go to loop the pathfinding steps.
                startTime = Time.time;
                StartCoroutine(Coroutine_PathFinding(pathFinder, grid));
        }
    }

    IEnumerator Coroutine_PathFinding(PathFinder pathFinder, RectGrid grid)
    {
        imgFindingPath.SetActive(true);
        int pathFindCount = 0;
        while (pathFinder.status == PathFinderStatus.RUNNING)
        {
            findingPath = true;
            /// When the path finder loop more than 300 times (after finding for 300 cells and still have found a path)
            if (pathFindCount > 300)
            {
                Debug.Log("PathFinding is taking too long, break it.");
                pathFinder.status = PathFinderStatus.FAILURE;
                break;
            }
            pathFinder.Step(false);
            pathFindCount++;
            yield return null;
        }
        if (pathFinder.status == PathFinderStatus.FAILURE)
        {
            StartCoroutine(ShowError());
            Debug.Log("Failed finding a path. No valid path exists");
        }
        // Found a path.
        if (pathFinder.status == PathFinderStatus.SUCCESS)
        {
            // curve point is the curve poiint of the path
            Vector2Int curvePoint = new (-1, -1);

            //Debug.Log("Found a path, time taken: " + (Time.time - startTime));

            // found a valid path.
            // accumulate all the locations by traversing from goal to the start.
            List<Vector2Int> reversePathLocations = new List<Vector2Int>();
            PathFinderNode node = pathFinder.GetCurrentNode();
            while (node != null)
            {
                reversePathLocations.Add(node.location);
                //Debug.Log(node.location);
                node = node.parent;
            }
            #region Calculation logic to make line less jagged
            for (int i = 1; i < reversePathLocations.Count - 1; i++)
            {
                // calculate the curve point, as the path found are straight lines, simply calculate using the x and y values of it to check with neighbours
                if ((reversePathLocations[i].x == reversePathLocations[i - 1].x && reversePathLocations[i].y == reversePathLocations[i + 1].y) ||
                    (reversePathLocations[i].x == reversePathLocations[i + 1].x && reversePathLocations[i].y == reversePathLocations[i - 1].y)) 
                {
                    curvePoint = reversePathLocations[i];
                    //Debug.Log(reversePathLocations[i] + " is the curve point.");
                    break;
                }
            }
            // if the curve point is not (-1, -1)
            if (curvePoint != Vector2Int.left + Vector2Int.down)
            {
                /// This part is to make the line drawn later less jagged
                Vector2 newCurvePoint = new Vector2(curvePoint.x, curvePoint.y);
                int indexOfCurvePoint = reversePathLocations.IndexOf(curvePoint);
                foreach (Vector2Int vector2Int in reversePathLocations)
                {
                    newReversePathLocations.Add(new Vector2(vector2Int.x, vector2Int.y));
                }
                // check if the from point to curve point is vertical or horizontal
                if (reversePathLocations[^1].x == curvePoint.x)
                {
                    Debug.Log("From point to curve point is vertical");
                    if (Mathf.Abs(lineStartPoint.x - newCurvePoint.x) < 0.5f)
                    {
                        // if the difference between the start point and curve point is less than 1 cell,
                        // then change the x value of the curve point to be the same as the start point
                        // and set the x value of curve point also
                        // similar to the rest
                        for (int i = newReversePathLocations.Count - 1; i >= indexOfCurvePoint; i--)
                        {
                            newReversePathLocations[i] = new Vector2(lineStartPoint.x, newReversePathLocations[i].y);
                        }
                        newCurvePoint = new Vector2(lineStartPoint.x, newCurvePoint.y);
                    }
                    //else
                    //{
                    //    Debug.Log("Difference between start point and curve point is more than one cell, so add the start point to the path.");
                    //    newReversePathLocations.Add(lineStartPoint);
                    //}
                }
                else
                {
                    Debug.Log("From point to curve point is horizontal");
                    if (Mathf.Abs(lineStartPoint.y - newCurvePoint.y) < 0.5f)
                    {
                        for (int i = newReversePathLocations.Count - 1; i >= indexOfCurvePoint; i--)
                        {
                            newReversePathLocations[i] = new Vector2(newReversePathLocations[i].x, lineStartPoint.y);
                        }
                        newCurvePoint = new Vector2(newCurvePoint.x, lineStartPoint.y);
                    }
                    //else
                    //{
                    //    Debug.Log("Difference between start point and curve point is more than one cell, so add the start point to the path.");
                    //    newReversePathLocations.Add(lineStartPoint);
                    //}
                }
                if (reversePathLocations[0].x == curvePoint.x)
                {
                    Debug.Log("To point to curve point is vertical");
                    if (Mathf.Abs(lineEndPoint.x - newCurvePoint.x) < 0.5f)
                    {
                        for (int i = 0; i < indexOfCurvePoint; i++)
                        {
                            newReversePathLocations[i] = new Vector2(lineEndPoint.x, newReversePathLocations[i].y);
                        }
                        newCurvePoint = new Vector2(lineEndPoint.x, newCurvePoint.y);
                    }
                    //else
                    //{
                    //    Debug.Log("Difference between end point and curve point is more than one cell, so add the end point to the path.");
                    //    newReversePathLocations.Insert(0, lineEndPoint);
                    //}
                }
                else
                {
                    Debug.Log("To point to curve point is horizontal");
                    if (Mathf.Abs(lineEndPoint.y - newCurvePoint.y) < 0.5f)
                    {
                        for (int i = 0; i < indexOfCurvePoint; i++)
                        {
                            newReversePathLocations[i] = new Vector2(newReversePathLocations[i].x, lineEndPoint.y);
                        }
                        newCurvePoint = new Vector2(newCurvePoint.x, lineEndPoint.y);
                    }
                    //else
                    //{
                    //    Debug.Log("Difference between end point and curve point is more than one cell, so add the end point to the path.");
                    //    newReversePathLocations.Insert(0, lineEndPoint);
                    //}
                }
                newReversePathLocations[indexOfCurvePoint] = newCurvePoint;
            }
            else
            {
                foreach (var v in reversePathLocations)
                {
                    newReversePathLocations.Add(new Vector2(v.x, v.y));
                }
            }
            #endregion

            GameObject line = Instantiate(this.line, transform);
            previousDrawnLineDict.Add(line, new List<RectGridCell>());
            line.GetComponent<DrawLine>().rectGrid = rectGrid;
            AddFromRotatePoint(line.GetComponent<DrawLine>(), newReversePathLocations[^1]); 
            // add all these points to the waypoints.
            for (int i = reversePathLocations.Count - 1; i >= 0; i--)
            {
                AddWayPoint(reversePathLocations[i]);
                //line.GetComponent<DrawLine>().points.Add(newReversePathLocations[i]);
                RectGridCell cell = rectGrid.transform.Find("cell_" + reversePathLocations[i].x + "_" + reversePathLocations[i].y).GetComponent<RectGridCell>();
                cell.SetNonWalkable();
                affectedCellsList.Add(cell);
            }
            for (int i = newReversePathLocations.Count - 1; i >= 0; i--)
            {
                line.GetComponent<DrawLine>().points.Add(newReversePathLocations[i]);
            }
            previousDrawnLineDict[line] = new(affectedCellsList);
            AddToRotatePoint(line.GetComponent<DrawLine>(), newReversePathLocations[0]);
            line.GetComponent<DrawLine>().lineFrom = lineFrom;
            line.GetComponent<DrawLine>().lineTo = lineTo;
            line.GetComponent<DrawLine>().lineColor = colorOfLineSelected;
            line.GetComponent<DrawLine>().affectedCellList = new (affectedCellsList);
            line.GetComponent<DrawLine>().finishedAddingPoints = true;

            if (firstTimeDrawLine)
            {
                ShowReverseInstruction(newReversePathLocations);
                firstTimeDrawLine = false;
            }
        }
        // reset all the used variables
        imgFindingPath.SetActive(false);
        findingPath = false;
        imgComponentFrom.sprite = transparentSprite;
        imgComponentTo.sprite = transparentSprite;
        affectedCellsList.Clear();
        newReversePathLocations.Clear();
        
    }

    public void AddWayPoint(Vector2Int point)
    {
        wayPoints.Enqueue(point);
    }

    private void AddFromRotatePoint(DrawLine line, Vector2 firstNodePos)
    {
        line.points.Add(lineStartPoint);
        line.points.Add(new Vector2(firstNodePos.x, lineStartPoint.y));
    }

    private void AddToRotatePoint(DrawLine line, Vector2 lastNodePos)
    {
        line.points.Add(new Vector2(lastNodePos.x, lineEndPoint.y));
        line.points.Add(lineEndPoint);
        //for (int i = 0; i < line.points.Count; i++)
        //{
        //    Debug.Log(line.points[i]);
        //}
    }

    /// <summary>
    /// Undo (remove) the last drawn line.
    /// </summary>
    public void UndoFunction()
    {
        if (IsFindingPath())
        {
            return;
        }
        // if there is a line previously drawn, remove it.
        if (previousDrawnLineDict.Count > 0)
        {
            GameObject lastLine = previousDrawnLineDict.Keys.ToList()[^1];
            List<RectGridCell> lastLineAffectedCells = previousDrawnLineDict[lastLine];
            foreach (RectGridCell cell in lastLineAffectedCells)
            {
                cell.SetWalkable();
            }
            previousDrawnLineDict.Remove(lastLine);
            Destroy(lastLine);
            previousDrawnLineFromAndTo[^1].ForEach(x => x.AllowDrawLine = true);
            previousDrawnLineFromAndTo.RemoveAt(previousDrawnLineFromAndTo.Count - 1);
        }
        else
        {
            return;
        }
    }

    private void ShowReverseInstruction(List<Vector2> linePoints)
    {
        Vector2 centerPoint = Vector2.zero;
        if (linePoints.Count % 2 == 0)
        {
            centerPoint = (linePoints[linePoints.Count / 2] + linePoints[linePoints.Count / 2 - 1]) / 2;
        }
        else
        {
            centerPoint = linePoints[linePoints.Count / 2];
        }
        Vector2 centerPointScreenPoint = Camera.main.WorldToScreenPoint(centerPoint);
        centerPointScreenPoint.y += 50f;
        //if (Mathf.Abs(linePoints[0].x - linePoints[^1].x) > Mathf.Abs(linePoints[0].y - linePoints[^1].y))
        //{
        //    // horizontal line
        //    if (centerPoint.y > 0)
        //        reverseInstruction.transform.position = new Vector2(centerPointScreenPoint.x, centerPointScreenPoint.y + 150);
        //    else
        //        reverseInstruction.transform.position = new Vector2(centerPointScreenPoint.x, centerPointScreenPoint.y - 150);
        //}
        //else
        //{
        //    // vertical line
        //    if (centerPoint.x < 0)
        //        reverseInstruction.transform.position = new Vector2(centerPointScreenPoint.x - 150, centerPointScreenPoint.y);
        //    else
        //        reverseInstruction.transform.position = new Vector2(centerPointScreenPoint.x + 150, centerPointScreenPoint.y);
        //}
        reverseInstruction.transform.position = centerPointScreenPoint;
        reverseInstruction.SetActive(true);
    }

    public bool IsFindingPath()
    {
        return findingPath;
    }
}
