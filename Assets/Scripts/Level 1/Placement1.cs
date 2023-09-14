using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Placement1 : MonoBehaviour
//quick drag and let go issues
{
    public CameraMovement cameraMovement;
    public Level1AnswerSheet1 answerSheet1;
    public HoverGroup hoverGroup;

    [HideInInspector]
    public GameObject component;
    [HideInInspector]
    public GameObject selectedPrefab;

    Transform highlightedPlacement;
    GameObject[] selections;
    public List<GameObject> duplicationPoints = new List<GameObject>();

    private bool allowDelete;

    private GameObject deletableGameobject;
    [HideInInspector]
    public GameObject objectToTrack;
    public LayerMask layerToIgnore;
    public bool deletingObject;

    private bool dontClear;

    private Color darkBlue = new Color(0, 0.024f, 1, 1);
    //0.024 is 6/255 rounded up to nearest 2SF

    // Start is called before the first frame update
    void Start()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();
        answerSheet1 = FindObjectOfType<Level1AnswerSheet1>();
        hoverGroup = FindObjectOfType<HoverGroup>();

        selections = GameObject.FindGameObjectsWithTag("Selection");

        allowDelete = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        ComponentController();
    }

    private void ComponentController()
    {
        Ray ray = new();
        if (Input.touchCount != 0)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~layerToIgnore))
        {
            if (!component)
                DeleteComponent(hit.transform);

            Highlight(hit);


            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (highlightedPlacement)
                {
                    if (duplicationPoints.Contains(highlightedPlacement.gameObject))
                    {
                        AutoPlace(highlightedPlacement.transform, false);
                    }
                    else
                    {
                        PlaceComponent(highlightedPlacement);
                    }
                }
            }
        }
        else
        {
            if (deletingObject)
            //prevents it being called when nothing is happening
            {
                DeleteComponent(); 
            }
        }


    }

    private void Highlight(RaycastHit hit)
    {
        foreach (GameObject selectPoints in selections)
        {
            if (selectPoints == hit.collider.gameObject)
            //checks for which object the collider hits and matches it to a gameobject in the array, that will become the highlighted/selected point
            {
                selectPoints.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else
            //everything else will be reset to its original colour
            {
                if (selectPoints.transform.GetChild(0).CompareTag("SharedSelection"))
                {
                    selectPoints.transform.GetChild(0).GetComponent<SpriteRenderer>().color = darkBlue;
                }
                else
                {
                    selectPoints.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
            continue;
        }


        if (hit.transform.CompareTag("Selection") && hit.transform.GetChild(0).GetComponent<SpriteRenderer>().color == Color.yellow)
        //assigns the highlighted placement variable to the hit object. The hit object has to meet two conditions, a tag of selection and the current colour has to be yellow
        //if allowdelete is false, it prevents the gameobject from being deleted since it is outside the boundaries of the play area
        {
            highlightedPlacement = hit.collider.transform;
            allowDelete = false;
        }
        else
        {
            allowDelete = true;
        }
    }

    private void AutoPlace(Transform objectToCheck, bool delete) 
    //automatically places the selected component across all the blue points
    {
        if (!objectToCheck || !selectedPrefab)
            return;

        if (!delete)
        {
            Destroy(component);
            for (int i = 0; i < duplicationPoints.Count; i++) 
            {
                if (duplicationPoints[i].transform.childCount < 2 && duplicationPoints[i] != objectToCheck && duplicationPoints[i].transform.childCount > 0)
                {
                    GameObject temp = Instantiate(selectedPrefab, duplicationPoints[i].transform.position, Quaternion.identity, duplicationPoints[i].transform);
                    temp.transform.localScale = temp.transform.localScale / 9;
                    temp.transform.localPosition = Vector3.zero;
                    temp.GetComponent<Collider>().enabled = true;

                    duplicationPoints[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = darkBlue;
                    duplicationPoints[i].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    duplicationPoints[i].transform.GetComponent<BoxCollider>().enabled = false;
                    duplicationPoints[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    duplicationPoints[i].transform.GetChild(0).GetComponent<Animator>().SetBool("ObjectPlaced", true);


                }
            }
            highlightedPlacement = null;
            component = null;
        }
        else if(delete && duplicationPoints.Contains(objectToCheck.gameObject))
        {
            foreach (GameObject dupepoint in duplicationPoints) 
            {
                if (dupepoint.transform.childCount > 1)
                {
                    Destroy(dupepoint.transform.GetChild(1).gameObject);
                    dupepoint.GetComponent<BoxCollider>().enabled = true;
                    dupepoint.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                    dupepoint.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    dupepoint.transform.GetChild(0).GetComponent<Animator>().SetBool("ObjectPlaced", false);
                }
            }
        }
    }

    private void PlaceComponent(Transform selectedTransform) 
    {
        if (!highlightedPlacement || !selectedPrefab)
            return;

        if (!component) 
        {
            component = Instantiate(selectedPrefab, selectedTransform.position, selectedPrefab.transform.rotation);
        }

        component.transform.parent = selectedTransform;
        component.transform.localPosition = Vector3.zero;
        component.transform.localRotation = selectedPrefab.transform.rotation;
        component.GetComponent<Collider>().enabled = true;

        if (selectedTransform.transform.GetChild(0).CompareTag("SharedSelection"))
        {
            selectedTransform.GetChild(0).GetComponent<SpriteRenderer>().color = darkBlue;
        }
        else 
        {
            selectedTransform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
        }
        selectedTransform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        selectedTransform.GetComponent<BoxCollider>().enabled = false;
        selectedTransform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        selectedTransform.GetChild(0).GetComponent<Animator>().SetBool("ObjectPlaced", true);

        if (!dontClear)
        {
            highlightedPlacement = null;
        }

        if (component.CompareTag("Component/CwpOpt") && selectedTransform.name.Equals("Selection Point (17)"))
        {
            Vector3 scaleTemp = component.transform.localScale;
            scaleTemp.x *= -1;
            component.transform.localScale = scaleTemp;
        }
        else if (component.CompareTag("Component/CwpOptElavated") && selectedTransform.name.Equals("Selection Point (16)"))
        {
            Vector3 scaleTemp = component.transform.localScale;
            scaleTemp.z *= -1;
            component.transform.localScale = scaleTemp;
        }

        cameraMovement.allowRotation = true;
        highlightedPlacement = null;
        component = null;

    }

    public void DeleteComponent(Transform selectedTransform = null) 
    {
        if (!selectedTransform && !allowDelete && !deletableGameobject && !hoverGroup.dragToPlace)
        {
            Destroy(component);
            return;
        }

        if (selectedTransform && !objectToTrack && selectedTransform.tag.StartsWith("Component"))
        {
            deletableGameobject = selectedTransform.gameObject;
            objectToTrack = deletableGameobject;
            deletingObject = true;
            cameraMovement.allowZoom = false;
        }

        if (Input.touchCount > 0 && objectToTrack)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                objectToTrack.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.nearClipPlane + 150));
                cameraMovement.allowRotation = false;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            //no longer detecting the touch
            {
                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out RaycastHit hit, Mathf.Infinity, ~(1 << 6)))
                {
                    objectToTrack.transform.parent.GetComponent<BoxCollider>().enabled = true;
                    objectToTrack.transform.parent.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                    objectToTrack.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    objectToTrack.transform.parent.GetChild(0).GetComponent<Animator>().SetBool("ObjectPlaced", false);
                    Destroy(objectToTrack.gameObject);
                    AutoPlace(deletableGameobject.transform.parent, true);
                    objectToTrack = null;
                    highlightedPlacement = null;
                    deletingObject = false;
                    cameraMovement.allowZoom = true;
                    cameraMovement.allowRotation = true;
                }
                else
                {
                    objectToTrack.gameObject.transform.localPosition = Vector3.zero;
                    objectToTrack = null;
                    deletingObject = false;
                    cameraMovement.allowZoom = true;
                    cameraMovement.allowRotation = true;
                }
            }
        }

    }
}
