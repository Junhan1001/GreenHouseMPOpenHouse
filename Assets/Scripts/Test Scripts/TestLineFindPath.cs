//using System.Collections;
//using System.Collections.Generic;
//using System.Xml.Serialization;
//using UnityEngine;
//using System.Drawing;
//using System.IO;

//public class TestLineFindPath : MonoBehaviour
//{
//    [SerializeField]
//    private GameObject emptyLinePrefab;
//    [SerializeField]
//    private GameObject emptyGO;

//    [SerializeField]
//    private Transform hit1;
//    [SerializeField]
//    private Transform hit2;

//    private GameObject GO;
//    private int hitCount = 0;

//    private void Start()
//    {
//        GameObject colliderParent = new("Path Collider");
//        List<QPathFinder.Path> paths = PathFinder.instance.graphData.paths;
//        foreach (QPathFinder.Path path in paths)
//        {
//            //Debug.Log("Path: " + PathFinder.instance.graphData.GetNode(path.IDOfA).Position + ", " + PathFinder.instance.graphData.GetNode(path.IDOfB).Position);
//            Vector3 from = PathFinder.instance.graphData.GetNode(path.IDOfA).Position;
//            Vector3 to = PathFinder.instance.graphData.GetNode(path.IDOfB).Position;
//            GameObject pathCollider = Instantiate(emptyGO, GetCentrePoint(from, to), Quaternion.identity, colliderParent.transform);
//            pathCollider.transform.localScale = new Vector2(GetLength(from, to), 0.1f);
//            pathCollider.transform.rotation = Quaternion.Euler(0, 0, GetAngle(from, to));
//            pathCollider.name = (PathFinder.instance.graphData.paths.IndexOf(path) + 1).ToString();
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            // Detect the hit object
//            RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
//            if (raycastHit2D)
//            {
//                if (hitCount == 0)
//                {
//                    hitCount = 1;
//                    hit1 = raycastHit2D.transform;
//                    GO = new GameObject();
//                    GO.transform.position = hit1.position;
//                }
//                else if (hitCount == 1)
//                {
//                    hit2 = raycastHit2D.transform;
//                    PathFinder.instance.FindShortestPathOfPoints(GO.transform.position, hit2.position, PathFinder.instance.graphData.lineType,
//                        Execution.Asynchronously, SearchMode.Simple, delegate (List<Vector3> points)
//                        {
//                            PathFollowerUtility.StopFollowing(GO.transform);
//                            if (points != null)
//                            {
//                                PathFollowerUtility.FollowPath(GO.transform, points, 10, true);

//                                List<Node> Nodes = PathFinder.instance.graphData.nodes;
//                                Vector3 from = points[0];
//                                foreach (Vector3 point in points)
//                                {
//                                    if (from != point)
//                                    {
//                                        OneTurnConnection(from, point);
//                                        from = point;
//                                    }
//                                }
//                                List<Node> nodeInUsed = new();
//                                foreach (Node node in Nodes)
//                                {
//                                    if (points.Contains(node.Position))
//                                    {
//                                        nodeInUsed.Add(node);
//                                    }
//                                }
//                                Node node1 = nodeInUsed[0];
//                                foreach (Node node in nodeInUsed)
//                                {
//                                    if (node == node1)
//                                        continue;
//                                    if (PathFinder.instance.graphData.GetPathBetween(node, node1) != null)
//                                    {
//                                        PathFinder.instance.graphData.GetPathBetween(node, node1).isOpen = false;
//                                    }
//                                    else if (PathFinder.instance.graphData.GetPathBetween(node1, node) != null)
//                                    {
//                                        PathFinder.instance.graphData.GetPathBetween(node1, node).isOpen = false;
//                                    }
//                                    node1 = node;
//                                }
//                            }
//                        });
//                    hitCount = 0;
//                }
//            }
//        }
//    }

//    private Vector2 GetCentrePoint(Vector2 point1, Vector2 point2)
//    {
//        return new Vector2((point1.x + point2.x) / 2, (point1.y + point2.y) / 2);
//    }

//    private void OneTurnConnection(Vector2 point1, Vector2 point2)
//    {
//        if (Mathf.Abs(point1.x - point2.x) > 0.5f || Mathf.Abs(point1.y - point2.y) > 0.5f)
//        {
//            // This means that the two point x or y difference can be clearly visible, so we draw a line that rotates 90 degrees
//            TwoTurnConnection(point1, point2);
//            return;
//        }
//        Vector2 centre = GetCentrePoint(point1, point2);
//        GameObject line = Instantiate(emptyLinePrefab, centre, Quaternion.identity);
//        // Pythagoras theorem, a2 + b2 = c2
//        float length = GetLength(point1, point2);
//        line.transform.localScale = new Vector2(length, 0.1f);
//        // tan theta = opposite / adjacent
//        // theta = atan2(opposite, adjacent), since atan2 returns the angle in radians, we need to convert it to degrees by using Rad2Deg
//        float zRot = GetAngle(point1, point2);
//        line.transform.rotation = Quaternion.Euler(0, 0, zRot);
//        line.name = "Line";
//        line.GetComponent<LineCollision>().origin = hit1;
//        line.GetComponent<LineCollision>().destination = hit2;
//    }

//    private void TwoTurnConnection(Vector2 point1, Vector2 point2)
//    {
//        // Check if the x difference is bigger or y difference is bigger
//        // If x difference is bigger, draw the line horizontal first, then vertical
//        // If y difference is bigger, draw the line vertical first, then horizontal
//        float xDiff = Mathf.Abs(point1.x - point2.x);
//        float yDiff = Mathf.Abs(point2.y - point1.y);

//        if (xDiff > yDiff)
//        {
//            // centre1 is the centre point of the first line, centre2 is the centre point of the second line
//            Vector2 middlePoint = new Vector2(point2.x, point1.y);
//            DrawLine(point1, middlePoint, point2);
//        }
//        else if (xDiff < yDiff)
//        {
//            Vector2 middlePoint = new Vector2(point1.x, point2.y);
//            DrawLine(point1, middlePoint, point2);
//        }
//    }

//    private void DrawLine(Vector2 point1, Vector2 middlePoint, Vector2 point2)
//    {
//        // Create a parent object to hold the two lines
//        GameObject line = new GameObject("Line");
//        line.tag = "LineParent";
//        line.AddComponent<LineCollision>();

//        Vector2 centre1 = GetCentrePoint(point1, middlePoint);
//        Vector2 centre2 = GetCentrePoint(middlePoint, point2);
//        GameObject line1 = Instantiate(emptyLinePrefab, centre1, Quaternion.identity);
//        GameObject line2 = Instantiate(emptyLinePrefab, centre2, Quaternion.identity);
//        line1.transform.parent = line.transform;
//        line2.transform.parent = line.transform;

//        // Calculate the length of the two lines
//        float length1 = GetLength(point1, middlePoint);
//        line1.transform.localScale = new Vector2(length1, 0.1f);
//        float length2 = GetLength(middlePoint, point2);
//        line2.transform.localScale = new Vector2(length2, 0.1f);

//        // Calculate the angle of the two lines
//        float zRot1 = GetAngle(point1, middlePoint);
//        line1.transform.rotation = Quaternion.Euler(0, 0, zRot1);
//        float zRot2 = GetAngle(middlePoint, point2);
//        line2.transform.rotation = Quaternion.Euler(0, 0, zRot2);

//        line1.name = "Line1";
//        line2.name = "Line2";
//        line1.GetComponent<LineCollision>().origin = hit1;
//        line1.GetComponent<LineCollision>().destination = hit2;
//        line2.GetComponent<LineCollision>().origin = hit1;
//        line2.GetComponent<LineCollision>().destination = hit2;
//    }

//    /// <summary>
//    /// x returns the length of the line, y returns the angle of the line
//    /// </summary>
//    private Vector2 CalculateLengthAndTheta(Vector2 point1, Vector2 point2)
//    {
//        float adjacent = point2.x - point1.x;
//        float opposite = point2.y - point1.y;
//        return new Vector2(GetCSqr(adjacent, opposite), GetTheta(opposite, adjacent));
//    }

//    private float GetLength(Vector2 point1, Vector2 point2)
//    {
//        return CalculateLengthAndTheta(point1, point2).x;
//    }

//    private float GetAngle(Vector2 point1, Vector2 point2)
//    {
//        return CalculateLengthAndTheta(point1, point2).y;
//    }

//    private float GetCSqr(float adjacent, float opposite)
//    {
//        return Mathf.Sqrt(Mathf.Pow(adjacent, 2) + Mathf.Pow(opposite, 2));
//    }

//    private float GetTheta(float opposite, float adjacent)
//    {
//        return Mathf.Atan2(opposite, adjacent) * Mathf.Rad2Deg;
//    }
//}
