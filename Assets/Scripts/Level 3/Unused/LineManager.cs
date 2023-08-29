//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.VisualScripting;

//namespace Level3
//{
//    public class LineManager : MonoBehaviour
//    //    //handles the state of the line
//    {
//        #region Unused
//        //        LineRenderer lineRenderer;
//        //        LineManagerController lineManagerController;
//        //        ComponentWheel componentWheel;
//        //        CheckFlow checkFlow;

//        //        private GameObject emptyLinePrefab;

//        //        public Vector3 startPos;

//        //        private Vector3 mousePosition = new();

//        //        [SerializeField]
//        //        internal bool isDrawing = false;

//        //        [SerializeField]
//        //        private float timePressed = 0f;

//        //        [SerializeField]
//        //        private Transform initT;
//        //        [SerializeField]
//        //        private Transform hitT;
//        //        private Vector2 centerPoint;

//        //        private GameObject GO;

//        //        private void Start()
//        //        {
//        //            componentWheel = FindObjectOfType<ComponentWheel>();
//        //            centerPoint = componentWheel.centerPointOfPlayArea;
//        //            lineManagerController = transform.parent.GetComponent<LineManagerController>();
//        //            initT = lineManagerController.componentClickedT;
//        //            emptyLinePrefab = lineManagerController.emptyLinePrefab;
//        //            //initial transform is set as the component that has been interacted with(iniT)
//        //            checkFlow = FindObjectOfType<CheckFlow>();

//        //            //lineRenderer = GetComponent<LineRenderer>();
//        //            //lineRenderer.startWidth = 0.1f;
//        //            //lineRenderer.endWidth = 0.1f;
//        //            //line variables

//        //            //mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        //            //mousePosition.z = 0;
//        //            //startPos = mousePosition;

//        //            //lineRenderer.enabled = true;
//        //            //lineRenderer.positionCount = 1;
//        //            //lineRenderer.SetPosition(0, startPos);
//        //            isDrawing = true;
//        //        }

//        //        // Update is called once per frame
//        //        void Update()
//        //        {
//        //            //mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        //            //mousePosition.z = 0;

//        //            if (InputSystem.Instance.LeftClick())
//        //            //if left click, run the function to draw lines
//        //            {
//        //                //CheckCanDrawLine();
//        //                LinePathFind();
//        //            }

//        //            //handles the cooldown of drawing lines, prevent spam and multiple lines drawn at once
//        //            //if (Input.GetMouseButton(0))
//        //            //{
//        //            //    timePressed += Time.deltaTime;
//        //            //}
//        //            //if (Input.GetMouseButtonUp(0))
//        //            //{
//        //            //    if (timePressed >= 1f)
//        //            //    {
//        //            //        CheckCanDrawLine();
//        //            //    }
//        //            //    timePressed = 0f;
//        //            //}

//        //            //if (lineRenderer.enabled && isDrawing)
//        //            //{
//        //            //Vector3 startPos = lineRenderer.GetPosition(0);
//        //            //Vector3 half = (mousePosition + startPos) / 2;
//        //            //lineRenderer.SetPosition(1, new Vector3(startPos.x, half.y, half.z));
//        //            //lineRenderer.SetPosition(2, new Vector3(mousePosition.x, half.y, half.z));
//        //            //lineRenderer.SetPosition(3, mousePosition);

//        //            //if (mousePosition != initT.position)
//        //            //if the current mouse position is not already on the component the line is starting from
//        //            //{
//        //            //if (mousePosition.x > initT.position.x)
//        //            //// when the mouse position has moved beyond the initial component x axis position in the positive direction
//        //            //{
//        //            //    //sets the line position to the mouse position then again?
//        //            //    lineRenderer.positionCount = 3;
//        //            //    lineRenderer.SetPosition(1, new Vector2(mousePosition.x, startPos.y));
//        //            //    lineRenderer.SetPosition(2, mousePosition);
//        //            //}
//        //            //}
//        //            //    }
//        //        }

//        //        private void LinePathFind()
//        //        {
//        //            if (!initT || !hitT /*|| !initT.GetComponentInParent<ComponentEvent>().AllowDrawLine() || !hitT.GetComponentInParent<ComponentEvent>().AllowDrawLine()*/)
//        //            {
//        //                Debug.Log("Initial or hit transform is null or not allowed to draw line as exceeded the max line allow to draw");
//        //                lineManagerController.componentClickedT = null;
//        //                Destroy(gameObject);
//        //                return;
//        //            }
//        //            initT.GetComponentInChildren<LineLimit>().AllowDrawLine = false;
//        //            hitT.GetComponentInChildren<LineLimit>().AllowDrawLine = false;
//        //            GO = new();
//        //            GO.transform.position = initT.position;
//        //            PathFinder.instance.FindShortestPathOfPoints(GO.transform.position, hitT.position, PathFinder.instance.graphData.lineType,
//        //                Execution.Asynchronously, SearchMode.Simple, delegate (List<Vector3> points)
//        //                {
//        //                    PathFollowerUtility.StopFollowing(GO.transform);
//        //                    if (points != null)
//        //                    {
//        //                        PathFollowerUtility.FollowPath(GO.transform, points, 10f, true);
//        //                        List<Node> Nodes = PathFinder.instance.graphData.nodes;
//        //                        Vector3 from = points[0];
//        //                        foreach (Vector3 point in points)
//        //                        {
//        //                            if (from != point)
//        //                            {
//        //                                OneTurnConnection(from, point);
//        //                                from = point;
//        //                            }
//        //                        }
//        //                        List<Node> nodeInUsed = new();
//        //                        foreach (Node node in Nodes)
//        //                        {
//        //                            if (points.Contains(node.Position))
//        //                            {
//        //                                nodeInUsed.Add(node);
//        //                            }
//        //                        }
//        //                        Node node1 = nodeInUsed[0];
//        //                        foreach (Node node in nodeInUsed)
//        //                        {
//        //                            if (node == node1)
//        //                                continue;
//        //                            if (PathFinder.instance.graphData.GetPathBetween(node, node1) != null)
//        //                            {
//        //                                PathFinder.instance.graphData.GetPathBetween(node, node1).isOpen = false;
//        //                            }
//        //                            else if (PathFinder.instance.graphData.GetPathBetween(node1, node) != null)
//        //                            {
//        //                                PathFinder.instance.graphData.GetPathBetween(node1, node).isOpen = false;
//        //                            }
//        //                            node1 = node;
//        //                        }
//        //                    }
//        //                    lineManagerController.linesDrawn.Add(true);
//        //                    lineManagerController.i++;
//        //                    lineManagerController.componentClickedT = null;
//        //                    Destroy(GO);
//        //                    Destroy(gameObject);
//        //                });
//        //        }

//        //        private void CheckCanDrawLine()
//        //        {
//        //            if (isDrawing)
//        //            {
//        //                isDrawing = false;
//        //                if (lineManagerController.componentClicked && initT != hitT)
//        //                //if there is a component that has been interacted with
//        //                //and the initial component(iniT) does not match the new component(hitT) that has been interacted with
//        //                {
//        //                    //adds the new line drawn into a list of lines drawn
//        //                    lineManagerController.linesDrawn.Add(true);
//        //                    lineManagerController.i++;
//        //                    lineManagerController.CheckLineDrawn();
//        //                    lineManagerController.componentClickedT = null;

//        //                    //handles generating the flow(particles) and the positions of the end and beginning points for the particles to flow
//        //                    //checkFlow.AddPipe(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
//        //                    //checkFlow.AddPipe(lineRenderer.GetPosition(1), lineRenderer.GetPosition(2));
//        //                    //checkFlow.CheckAnswer();
//        //                }
//        //                else
//        //                {
//        //                    //sets the component clicked to null and destroys the line
//        //                    lineManagerController.componentClickedT = null;
//        //                    Destroy(gameObject);
//        //                }
//        //            }
//        //        }

//        //        //for reference to set the hitT variable in LineManagerController script
//        //        public Transform SetHitT
//        //        {
//        //            set { hitT = value; }
//        //        }

//        //        public Vector2 GetCentrePoint(Vector2 point1, Vector2 point2)
//        //        {
//        //            return new Vector2((point1.x + point2.x) / 2, (point1.y + point2.y) / 2);
//        //        }

//        //        private void OneTurnConnection(Vector2 point1, Vector2 point2)
//        //        {
//        //            if (Mathf.Abs(point1.x - point2.x) > 0.5f || Mathf.Abs(point1.y - point2.y) > 0.5f)
//        //            {
//        //                // This means that the two point x or y difference can be clearly visible, so we draw a line that rotates 90 degrees
//        //                TwoTurnConnection(point1, point2);
//        //                return;
//        //            }
//        //            Vector2 centre = GetCentrePoint(point1, point2);
//        //            GameObject line = Instantiate(emptyLinePrefab, centre, Quaternion.identity);
//        //            // Pythagoras theorem, a2 + b2 = c2
//        //            float length = GetLength(point1, point2);
//        //            line.transform.localScale = new Vector2(length, 0.1f);
//        //            // tan theta = opposite / adjacent
//        //            // theta = atan2(opposite, adjacent), since atan2 returns the angle in radians, we need to convert it to degrees by using Rad2Deg
//        //            float zRot = GetAngle(point1, point2);
//        //            line.transform.rotation = Quaternion.Euler(0, 0, zRot);
//        //            line.name = "Line";
//        //            line.GetComponent<LineCollision>().origin = initT;
//        //            line.GetComponent<LineCollision>().destination = hitT;
//        //        }

//        //        private void TwoTurnConnection(Vector2 point1, Vector2 point2)
//        //        {
//        //            // Check if the x difference is bigger or y difference is bigger
//        //            // If x difference is bigger, draw the line horizontal first, then vertical
//        //            // If y difference is bigger, draw the line vertical first, then horizontal
//        //            float xDiff = Mathf.Abs(point1.x - point2.x);
//        //            float yDiff = Mathf.Abs(point2.y - point1.y);

//        //            if (xDiff > yDiff)
//        //            {
//        //                // centre1 is the centre point of the first line, centre2 is the centre point of the second line
//        //                Vector2 middlePoint = new Vector2(point2.x, point1.y);
//        //                DrawLine(point1, middlePoint, point2);
//        //            }
//        //            else if (xDiff < yDiff)
//        //            {
//        //                Vector2 middlePoint = new Vector2(point1.x, point2.y);
//        //                DrawLine(point1, middlePoint, point2);
//        //            }
//        //        }

//        //        private void DrawLine(Vector2 point1, Vector2 middlePoint, Vector2 point2)
//        //        {
//        //            // Create a parent object to hold the two lines
//        //            GameObject line = new GameObject("Line");
//        //            line.tag = "LineParent";
//        //            line.AddComponent<LineCollision>();

//        //            Vector2 centre1 = GetCentrePoint(point1, middlePoint);
//        //            Vector2 centre2 = GetCentrePoint(middlePoint, point2);
//        //            GameObject line1 = Instantiate(emptyLinePrefab, centre1, Quaternion.identity);
//        //            GameObject line2 = Instantiate(emptyLinePrefab, centre2, Quaternion.identity);
//        //            line1.transform.parent = line.transform;
//        //            line2.transform.parent = line.transform;

//        //            // Calculate the length of the two lines
//        //            float length1 = GetLength(point1, middlePoint);
//        //            line1.transform.localScale = new Vector2(length1, 0.1f);
//        //            float length2 = GetLength(middlePoint, point2);
//        //            line2.transform.localScale = new Vector2(length2, 0.1f);

//        //            // Calculate the angle of the two lines
//        //            float zRot1 = GetAngle(point1, middlePoint);
//        //            line1.transform.rotation = Quaternion.Euler(0, 0, zRot1);
//        //            float zRot2 = GetAngle(middlePoint, point2);
//        //            line2.transform.rotation = Quaternion.Euler(0, 0, zRot2);

//        //            line1.name = "Line1";
//        //            line2.name = "Line2";
//        //            line1.GetComponent<LineCollision>().origin = initT;
//        //            line1.GetComponent<LineCollision>().destination = hitT;
//        //            line2.GetComponent<LineCollision>().origin = initT;
//        //            line2.GetComponent<LineCollision>().destination = hitT;
//        //        }

//        //        /// <summary>
//        //        /// x returns the length of the line, y returns the angle of the line
//        //        /// </summary>
//        //        private Vector2 CalculateLengthAndTheta(Vector2 point1, Vector2 point2)
//        //        {
//        //            float adjacent = point2.x - point1.x;
//        //            float opposite = point2.y - point1.y;
//        //            return new Vector2(GetCSqr(adjacent, opposite), GetTheta(opposite, adjacent));
//        //        }

//        //        public float GetLength(Vector2 point1, Vector2 point2)
//        //        {
//        //            return CalculateLengthAndTheta(point1, point2).x;
//        //        }

//        //        public float GetAngle(Vector2 point1, Vector2 point2)
//        //        {
//        //            return CalculateLengthAndTheta(point1, point2).y;
//        //        }

//        //        private float GetCSqr(float adjacent, float opposite)
//        //        {
//        //            return Mathf.Sqrt(Mathf.Pow(adjacent, 2) + Mathf.Pow(opposite, 2));
//        //        }

//        //        private float GetTheta(float opposite, float adjacent)
//        //        {
//        //            return Mathf.Atan2(opposite, adjacent) * Mathf.Rad2Deg;
//        //        }
//        #endregion
//    }
//}