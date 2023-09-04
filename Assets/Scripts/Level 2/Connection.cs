using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Connection : MonoBehaviour
{

    #region variables
    public bool tutorial;

    [Header("References")]
    public OpenHouseLevel2 level2AnswerSheet;
    public SelectedComponent selectedComponent;
    public ReturnValue valueReturnBtn;
    public ToggleMultiConnect multiConnectToggle;
    public CameraMovement cameraMovement;
    public DisplayConnect display;

    public GameObject particle;

    public Material highlightMat;
    public Material selectionMat;

    public List<GameObject> points;
    public List<GameObject> multiPoints;
    public List<GameObject> tobeunhighlighted;
    public SelectedComponent[] componentArray;
    public List<GameObject> OutCP;
    public List<GameObject> InCP;
    public List<GameObject> CheckList = null;

    public int multiConnectLimit;
    public bool multiConnect;

    public List<GameObject> pipes;

    public GameObject pipe;
    public GameObject entrance;
    public GameObject exit;
    public GameObject body;

    Transform point1;
    Transform point2;

    [HideInInspector]
    public List<Transform> multiplePoints;
    [HideInInspector]
    public List<Vector3> pipeConnection;
    [HideInInspector]
    public List<Vector3> centerPoints;
    [HideInInspector]
    public List<float> lengths;

    Material originalMat;
    Transform highlight;
    Transform selection;
    RaycastHit raycastHit;

    Renderer[] renderers;

    bool allMatch;
    public bool anomalyFound;
    public bool pipeWarning;
    public GameObject pipeWarningPanel;
    public Fade fade;
    public GameObject uiParent;
    public GameObject ErrorInandIn;

    private KeyValuePair<List<Transform>, List<List<Transform>>> kypAHUConnectionPoints = new();
    [SerializeField]
    private List<Transform> AHUs = new();
    [SerializeField]
    private List<List<Transform>> AHUConnectionPoints = new();
    [SerializeField]
    private List<Transform> AHUConnectionPoint1 = new();
    [SerializeField]
    private List<Transform> AHUConnectionPoint2 = new();

    public List<GameObject> AHUPoint1;
    public List<GameObject> AHUPoint2;
    #endregion


    private void Awake()
    {
        Camera.main.transform.parent.GetComponent<CameraMovement>().zoomStopDistance = 30f;
        componentArray = FindObjectsOfType<SelectedComponent>();
        display = FindObjectOfType<DisplayConnect>();

        AHUConnectionPoints = new List<List<Transform>>() { AHUConnectionPoint1, AHUConnectionPoint2 };
        kypAHUConnectionPoints = new KeyValuePair<List<Transform>, List<List<Transform>>>(AHUs, AHUConnectionPoints);
    }

    // Update is called once per frame
    void Update()
    {
        if (!fade.PauseCheck)
        {
            //CheckUndoPipes();

            Highlight();
            

            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    void CheckUndoPipes()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (pipes.Count > 0)
                {
                    if (pipes.Contains(pipes[pipes.Count - 1]))
                    {
                        GameObject Clone = pipes[pipes.Count - 1];
                        pipes.Remove(pipes[pipes.Count - 1]);
                        Destroy(Clone);
                    }
                }
            }
        }
    }

    void Highlight()
    {
        if (!pipe && !entrance && !exit && !body)
        {
            pipeWarning = true;
        }
        else 
        {
            pipeWarning = false;
        }
        // checks if there is an object being highlighted
        // if so, remove highlight by resetting the object's material to it's original material
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#if UNITY_STANDALONE
        if(highlight != null)
        {
            highlight.GetComponent<MeshRenderer>().material = originalMat;
            highlight = null;
        }


        // checks if the raycast being drawn from the mouse hits an object
        // if so, check if the tag of the highlight is called "Connection" before setting the colour of the object's material
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            if(highlight.CompareTag("Connection") && highlight != selection)
            {
                if(highlight.GetComponent<MeshRenderer>().material != highlightMat)
                {
                    originalMat = highlight.GetComponent<MeshRenderer>().material;
                    highlight.GetComponent<MeshRenderer>().material = highlightMat;
                }
            }
            else
            {
                highlight = null;
            }
        }
#endif
        if (InputSystem.Instance.LeftClick())
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.layer == 5)
                    return;
            }
            if (Physics.Raycast(ray, out raycastHit))
            {
                foreach (Touch touch in Input.touches)
                {
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        return;
                    }
                }
                selection = raycastHit.transform;
                if (selection.GetComponent<SelectedComponent>())
                {
                    if (selection.GetComponent<SelectedComponent>().gameObject.tag.Contains("AHU") && selectedComponent != null)
                    {
                        return;
                    }
                    selectedComponent = selection.GetComponent<SelectedComponent>();
                    //selectedComponent.valueReturn.selectedComponentBtn = selectedComponent;
                    //valueReturnBtn.selectedComponentBtn = selectedComponent;

                    foreach (SelectedComponent component in componentArray) 
                    {
                        if (selectedComponent != component)
                        { 
                            component.RemoveUI();   
                        }
                        else //if (selectedComponent == component) //if smth selected & same item, look at
                        {
                            cameraMovement.LookAtComponent(selectedComponent.transform);
                            component.ShowUI(Camera.main.WorldToScreenPoint(selectedComponent.transform.position));
                        }
                    }
                }
                else 
                {
                    if (selectedComponent)
                    {
                        selectedComponent.RemoveUI();
                        cameraMovement.transform.rotation = Quaternion.Euler(11, 90, 0);
                        cameraMovement.allowRotation = true;
                        cameraMovement.zooming = false;
                        Camera.main.fieldOfView = 25;

                        if (points.Count == 0) 
                        {
                            display.ResetDisplayConnect();
                        }
                    }
                }
            }
            else
            {
                if (selectedComponent) 
                {
                    selectedComponent.RemoveUI();
                    cameraMovement.transform.rotation = Quaternion.Euler(11, 90, 0);
                    cameraMovement.allowRotation = true;
                    cameraMovement.zooming = false;
                    Camera.main.fieldOfView = 25;
                    if (points.Count == 0)
                    {
                        display.ResetDisplayConnect();
                    }
                    selectedComponent = null;
                }
            }
        }
        if (valueReturnBtn && valueReturnBtn.pressedBtn)
        {
            if (selectedComponent.IndexReturn() != null)
            {

                if (selectedComponent.IndexReturn().GetComponent<MeshRenderer>().sharedMaterial == selectionMat)
                {
                    selectedComponent.IndexReturn().GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                    points.Remove(selectedComponent.IndexReturn().gameObject);
                    if (tobeunhighlighted[tobeunhighlighted.Count-1] == selectedComponent.IndexReturn() && (AHUPoint1.Contains(selectedComponent.IndexReturn()) || AHUPoint2.Contains(selectedComponent.IndexReturn()) ))
                    {
                        if (AHUPoint1.Contains(selectedComponent.IndexReturn()))
                        {
                            foreach (GameObject point in AHUPoint1)
                            {
                                if (tobeunhighlighted.Contains(point))
                                {
                                    point.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                                    tobeunhighlighted.Remove(point);
                                }
                            }
                        }
                        if (AHUPoint2.Contains(selectedComponent.IndexReturn()))
                        {
                            foreach (GameObject point in AHUPoint2)
                            {
                                if (tobeunhighlighted.Contains(point))
                                {
                                    point.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                                    tobeunhighlighted.Remove(point);
                                }
                            }
                        }
                    }
                    else
                    {
                        tobeunhighlighted.RemoveAt(tobeunhighlighted.Count - 1);
                    }
                    valueReturnBtn.pressedBtn = false;
                    return;
                }
                originalMat = selectedComponent.IndexReturn().GetComponent<MeshRenderer>().material;
                selectedComponent.IndexReturn().GetComponent<MeshRenderer>().sharedMaterial = selectionMat;
            }

            if (!points.Contains(selectedComponent.IndexReturn()) && selectedComponent.IndexReturn() != null)
            {
                //if no points contains the index return and the index return not null
                if (points.Contains(selectedComponent.IndexReturn().gameObject))
                {
                    
                    var last = points.Count;

                    if (AHUPoint1.Contains(points[last - 1]))
                    {
                        foreach (GameObject point in AHUPoint1)
                        {
                            if (!tobeunhighlighted.Contains(point))
                            {
                                point.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                                tobeunhighlighted.Remove(point);
                            }
                        }
                    }
                    else
                    {
                        points.Remove(selectedComponent.IndexReturn().gameObject);
                        tobeunhighlighted.RemoveAt(tobeunhighlighted.Count - 1);
                    }
                }
                else
                {
                    points.Add(selectedComponent.IndexReturn().gameObject);
                    var last = points.Count;
                    
                    if (AHUPoint1.Contains(points[last - 1]))
                    {
                        foreach (GameObject point in AHUPoint1)
                        {
                            if (!tobeunhighlighted.Contains(point))
                            {
                                point.GetComponent<MeshRenderer>().sharedMaterial = selectionMat;
                                tobeunhighlighted.Add(point);
                            }
                        }
                    }
                    else if (AHUPoint2.Contains(points[last - 1]))
                    {
                        foreach (GameObject point in AHUPoint2)
                        {
                            if (!tobeunhighlighted.Contains(point))
                            {
                                point.GetComponent<MeshRenderer>().sharedMaterial = selectionMat;
                                tobeunhighlighted.Add(point);
                            }
                        }
                    }
                    else if (!tobeunhighlighted.Contains(points[last - 1]))
                    {
                        points[last - 1].GetComponent<MeshRenderer>().sharedMaterial = selectionMat;
                        tobeunhighlighted.Add(points[last - 1]);
                    }
                }
 
                if (points.Count >= 2)
                {

                    //Insert Cannot Connect in and in/Out and Out here
                    int insame = 0;
                    int outsame = 0;
                    foreach (GameObject pipe in points)
                    {
                        if (InCP.Contains(pipe))
                        {
                            insame += 1;
                        }
                        else if (OutCP.Contains(pipe))
                        {
                            outsame += 1;
                        }
                        
                        if (insame >= 2 || outsame >= 2)
                        {
                            tobeunhighlighted.Remove(pipe);
                            outsame = insame = 0;

                            foreach (GameObject connectionPoint in points) 
                            {
                                connectionPoint.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                            }
                            points.Clear();
                            display.ResetDisplayConnect();
                            ErrorInandIn.SetActive(true);
                            return;
                        }
                    }
                    outsame = insame = 0;

                    var pointlist = new List<GameObject>();
                    for (int i = 0; i < points.Count; i++)
                    {

                        if (AHUPoint1.Contains(points[i]))
                        {
                            if (i == 0)
                            {
                                pointlist = new List<GameObject>(AHUPoint1);
                                pointlist.Add(points[1]);
                            }
                            if (i == 1)
                            {
                                pointlist = new List<GameObject>(AHUPoint1);
                                pointlist.Add(points[0]);
                            }
                            MultiConnect(pointlist);
                        }
                        else if (AHUPoint2.Contains(points[i]))
                        {
                            if (i == 0)
                            {
                                pointlist = new List<GameObject>(AHUPoint2);
                                pointlist.Add(points[1]);
                            }
                            if (i == 1)
                            {
                                pointlist = new List<GameObject>(AHUPoint2);
                                pointlist.Add(points[0]);
                            }
                            MultiConnect(pointlist);
                        }
                    }
                    if (points.Count >= 2)
                    {
                        Connect();
                    } 
                    //Connect();
                    //if (FindObjectOfType<Tutorial>() != null)
                    //{
                    //    FindObjectOfType<Tutorial>().CheckConnection();
                    //}
                }
            }
            valueReturnBtn.pressedBtn = false;
        }
    }

    void Connect()
    {
        if (!pipe && !entrance && !exit && !body)
        {
            return;
        }
        for (int i = 0; i < points.Count; i++)
        {
            point1 = points[0].transform;
            point2 = points[1].transform;
        }
        //Debug.Log(pipe.transform.name);

        Quaternion rotation1 = point1.rotation;
        Quaternion rotation2 = point2.rotation;

        GameObject pipeMain = Instantiate(pipe, transform.position, Quaternion.identity);
        //Instantiates an empty gameobject to be the parent of the pipes generated by this function

        //Instantiates both the pipes int
        //
        //to be the beginning of the system and the end
        //sets the point where they instantiate based on the selected points from the list
        //changes the rotation to make sure the pipes faces the points transform
        GameObject pipeEntrance = Instantiate(entrance, point1.transform.position, rotation1);
        pipeEntrance.transform.LookAt(point1.transform);

        Vector3 first = pipeEntrance.transform.GetChild(1).position;

        GameObject pipeExit = Instantiate(exit, point2.transform.position, rotation2);
        pipeExit.transform.LookAt(point2.transform);

        //used later.
        Vector3 second = pipeExit.transform.GetChild(1).position;

        Vector3 center = (first + second) / 2;

        float length = Vector3.Distance(first, second);

        GameObject pipeBody = Instantiate(body, center, Quaternion.identity);
        pipeBody.transform.localScale = new Vector3(pipeBody.transform.localScale.x, pipeBody.transform.localScale.y, length);
        pipeBody.transform.LookAt(first);

        pipeEntrance.transform.parent = pipeMain.transform;
        pipeExit.transform.parent = pipeMain.transform;
        pipeBody.transform.parent = pipeMain.transform;

        pipes.Add(pipeMain);

        Connected();
        point1 = null;
        point2 = null;
        //tobeunhighlighted = new List<GameObject>(points);
        points.Clear();
    }


    void MultiConnect(List<GameObject> pointList)
    {
        if (!pipe && !entrance && !exit && !body)
        {
            return;
        }
        for (int i = 0; i < pointList.Count; i++)
        {
            multiplePoints.Add(pointList[i].transform);
        }

        GameObject pipeMain = Instantiate(pipe, transform.position, Quaternion.identity);

        for (int i = 0; i < pointList.Count; i++)
        {
            if (entrance.transform.name == "Pipe Final CHWR") // this extends the pipe for CHWR so that it doesn't overlap with CHWS
            {
                GameObject pipeEntry = Instantiate(entrance, pointList[i].transform.position, pointList[i].transform.rotation);
                pipeEntry.transform.localScale = new Vector3(pipeEntry.transform.localScale.x, pipeEntry.transform.localScale.y, pipeEntry.transform.localScale.z * 2f);
                pipeEntry.transform.LookAt(pointList[i].transform);
                pipeEntry.transform.parent = pipeMain.transform;

                pipeConnection.Add(pipeEntry.transform.GetChild(1).transform.position);
            }
            else //all other pipes stay the same length
            {
                GameObject pipeEntry = Instantiate(entrance, pointList[i].transform.position, pointList[i].transform.rotation);
                pipeEntry.transform.LookAt(pointList[i].transform);
                pipeEntry.transform.parent = pipeMain.transform;


                pipeConnection.Add(pipeEntry.transform.GetChild(1).transform.position);
            }
        }

        for (int i = 0; i < pipeConnection.Count - 1; i++)
        {
            centerPoints.Add((pipeConnection[i] + pipeConnection[i + 1]) / 2);
        }

        for (int i = 0; i < centerPoints.Count; i++)
        {
            lengths.Add(Vector3.Distance(pipeConnection[i], pipeConnection[i + 1]));
        }

        for (int i = 0; i < centerPoints.Count; i++)
        {
            GameObject pipeBody = Instantiate(body, centerPoints[i], Quaternion.identity);
            pipeBody.transform.localScale = new Vector3(pipeBody.transform.localScale.x, pipeBody.transform.localScale.y, lengths[i]);
            pipeBody.transform.LookAt(pipeConnection[i]);
            pipeBody.transform.parent = pipeMain.transform;
        }

        pipes.Add(pipeMain);

        //pipeMain.AddComponent<ParticleFlow>();
        //tobeunhighlighted = new List<GameObject>(points);

        Connected();
        pointList.Clear();
        multiplePoints.Clear();
        points.Clear();
        pipeConnection.Clear();
        centerPoints.Clear();
        lengths.Clear();
    }

    public void UndoPipe()
    {
        //if (tobeunhighlighted.Count > 0)
        //{
        //    if (tobeunhighlighted[tobeunhighlighted.Count - 1].transform.GetComponent<MeshRenderer>().sharedMaterial == selectionMat)
        //    {
        //        if (AHUPoint1.Contains(tobeunhighlighted[tobeunhighlighted.Count - 1]) || AHUPoint1.Contains(tobeunhighlighted[tobeunhighlighted.Count - 2]) || AHUPoint1.Contains(tobeunhighlighted[tobeunhighlighted.Count - 3]))
        //        {
        //            foreach (GameObject point in AHUPoint1)
        //            {
        //                Debug.Log("Removing");
        //                point.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
        //                tobeunhighlighted.Remove(point);
        //            }
        //        }
        //        else if (AHUPoint2.Contains(tobeunhighlighted[tobeunhighlighted.Count - 1]) || (AHUPoint2.Contains(tobeunhighlighted[tobeunhighlighted.Count - 2])) || AHUPoint2.Contains(tobeunhighlighted[tobeunhighlighted.Count - 3]))
        //        {
        //            foreach (GameObject point2 in AHUPoint2)
        //            {
        //                Debug.Log("Removing 2");
        //                point2.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
        //                tobeunhighlighted.Remove(point2);
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    Debug.Log("No Pipes Left");
        //    return;
        //}



        if (tobeunhighlighted.Count > 1)
        {
            if (tobeunhighlighted.Count % 2 == 0)
            {
                for (int i = 1; i <= 2; i++)
                {
                    if (tobeunhighlighted[tobeunhighlighted.Count - 1].transform.GetComponent<MeshRenderer>().sharedMaterial == selectionMat)
                    {
                        //Debug.Log(tobeunhighlighted.Count - 1);
                        //Debug.Log("reached");
                        tobeunhighlighted[tobeunhighlighted.Count - 1].transform.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                        CheckList.Add(tobeunhighlighted[tobeunhighlighted.Count - 1]);
                        Debug.Log(CheckList.Count);
                        tobeunhighlighted.RemoveAt(tobeunhighlighted.Count - 1);
                        
                    }
                }
                points.Clear();
                foreach (GameObject removed in CheckList)
                {
                    if (AHUPoint1.Contains(removed) || AHUPoint2.Contains(removed))
                    {
                        if (AHUPoint1.Contains(removed))
                        {
                            for (int index = 0; index < AHUPoint1.Count-1;index++)
                            {
                                if (AHUPoint1.Contains(tobeunhighlighted[tobeunhighlighted.Count-1]))
                                {
                                    AHUPoint1[index].transform.GetComponent<MeshRenderer>().sharedMaterial = originalMat; //tobeunhighlighted[tobeunhighlighted.Count - 1]
                                    tobeunhighlighted.Remove(AHUPoint1[index]);
                                }
                            }
                            //CheckList.Remove(removed);
                        }
                        else if (AHUPoint2.Contains(removed))
                        {
                            for (int index = 0; index < AHUPoint2.Count - 1; index++)
                            {
                                if (AHUPoint2.Contains(tobeunhighlighted[tobeunhighlighted.Count - 1]))
                                {
                                    AHUPoint2[index].transform.GetComponent<MeshRenderer>().sharedMaterial = originalMat; //tobeunhighlighted[tobeunhighlighted.Count - 1]
                                    tobeunhighlighted.Remove(AHUPoint2[index]);
                                }
                            }
                            //CheckList.Remove(removed);
                        }
                    }
                }
                CheckList.Clear();
                Debug.Log("Cleared");
            }

            else if (tobeunhighlighted.Count % 2 == 1)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Debug.Log(tobeunhighlighted.Count);
                    if (tobeunhighlighted[tobeunhighlighted.Count - 1].transform.GetComponent<MeshRenderer>().sharedMaterial == selectionMat)
                    {
                        tobeunhighlighted[tobeunhighlighted.Count - 1].transform.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                        CheckList.Add(tobeunhighlighted[tobeunhighlighted.Count - 1]);
                        tobeunhighlighted.RemoveAt(tobeunhighlighted.Count - 1);
                    }
                }
                points.Clear();
                foreach (GameObject removed in CheckList)
                {
                    if (AHUPoint1.Contains(removed) || AHUPoint2.Contains(removed))
                    {
                        Debug.Log("Reached the important part");
                        if (AHUPoint1.Contains(removed))
                        {
                            for (int index = 0; index < AHUPoint1.Count - 1; index++)
                            {
                                if (AHUPoint1.Contains(tobeunhighlighted[tobeunhighlighted.Count - 1]))
                                {
                                    AHUPoint1[index].transform.GetComponent<MeshRenderer>().sharedMaterial = originalMat; //tobeunhighlighted[tobeunhighlighted.Count - 1]
                                    tobeunhighlighted.Remove(AHUPoint1[index]);
                                }
                            }
                        }
                        else if (AHUPoint2.Contains(removed))
                        {
                            for (int index = 0; index < AHUPoint1.Count - 1; index++)
                            {
                                if (AHUPoint1.Contains(tobeunhighlighted[tobeunhighlighted.Count - 1]))
                                {
                                    AHUPoint1[index].transform.GetComponent<MeshRenderer>().sharedMaterial = originalMat; //tobeunhighlighted[tobeunhighlighted.Count - 1]
                                    tobeunhighlighted.Remove(AHUPoint1[index]);
                                }
                            }
                        }
                    }
                }
                CheckList.Clear();
            }
        }
        else
        {
            tobeunhighlighted[tobeunhighlighted.Count - 1].transform.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
            tobeunhighlighted.RemoveAt(tobeunhighlighted.Count - 1);
        }


        if (pipes.Count >= 1)
        {
            GameObject Clone = pipes[pipes.Count - 1];
            pipes.Remove(pipes[pipes.Count - 1]);
            Destroy(Clone);
            anomalyFound = false;

        }
        Debug.Log("Removed the lastest pipe");

    }


    void Connected()
    {
        if (level2AnswerSheet.ListComparison(points))
        {

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (points[i].name.Equals(points[j].name))
                    {
                        allMatch = true;
                        name = points[i].name;
                    }
                    else if (!points[i].name.Equals(points[j].name))
                    {
                        anomalyFound = true;
                    }
                }

            }

            if (allMatch && !anomalyFound)
            {
                //
                level2AnswerSheet.connectionsChecked = true;

                //pipeMain.AddComponent<ParticleFlow>();

                Debug.Log("matches");
            }
            else if (anomalyFound)
            {
                Debug.Log("anomaly present");
                Debug.Log("no match");
                level2AnswerSheet.connectionsChecked = false;
            }
        }
    }


    //void AHUHighlight()
    //{

    //    for (int i = 0; i < kypAHUConnectionPoints.Value.Count; i++)
    //    {
    //        Debug.Log(kypAHUConnectionPoints.Value.Count + "+" + kypAHUConnectionPoints.Key.Count);

    //        for (int k = 0; k < kypAHUConnectionPoints.Key.Count; k++)
    //        {
    //            foreach (GameObject POINT in tobeunhighlighted)
    //            {
    //                if (kypAHUConnectionPoints.Key.Contains(POINT.transform))
    //                {
    //                    foreach (Transform T in kypAHUConnectionPoints.Value[i])
    //                    {
    //                        if (!tobeunhighlighted.Contains(T.gameObject))
    //                        {
    //                            tobeunhighlighted.Add(T.gameObject);
    //                        }
    //                    }   
    //                }
    //            }
    //            Debug.Log(kypAHUConnectionPoints.Value[i][k]);
    //            originalMat = kypAHUConnectionPoints.Value[i][k].GetComponent<MeshRenderer>().sharedMaterial;
    //            //return an index ( 0 /1 )
    //            // get all kvp keys, highlight all its value[index]

    //            kypAHUConnectionPoints.Value[i][k].GetComponent<MeshRenderer>().sharedMaterial = selectionMat;
    //            //tobeunhighlighted.Add(kypAHUConnectionPoints.Value[i][k].gameObject);

    //        }
    //    }
    //}
}
