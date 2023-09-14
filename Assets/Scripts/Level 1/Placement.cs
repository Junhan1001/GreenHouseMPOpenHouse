using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Placement : MonoBehaviour
//Handles the Placement of component gameobjects
//Also handles the highlighting of the selected gameobjects(green boxes) in the scene
{
    public TextMeshProUGUI testingText;

    public CameraMovement cameraMovement;
    public Level1AnswerSheet1 answerSheet1;
    public Hover hover;
    public HoverGroup hoverGroup;

    public Sprite highlightSprite;
    public Sprite selectionSprite;
    public Sprite originalSprite;

    //[HideInInspector]
    public GameObject component;
    [HideInInspector]
    public GameObject selectedPrefab;
    Transform selected;
    Transform highlightedPlacement;
    GameObject[] selections;
    public GameObject[] duplicationPoints;

    public bool tutorial;
    private bool allowDelete = false;

    private GameObject deletableGameobject;
    private GameObject deletingGO;
    public bool deletingObject;

    private Transform highlighted;

    private void Start()
    {
        selections = GameObject.FindGameObjectsWithTag("Selection");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(allowDelete);
        ////resets allow delete to true after object has been placed(no finger touching screen)
        //if (!allowDelete && Input.touchCount > 0)
        //{
        //    if (Input.GetTouch(0).phase == TouchPhase.Ended)
        //    {
        //        allowDelete = true;
        //    }
        //}
    }

    private void FixedUpdate()
    {
        Highlight();
    }

    public void Select(Transform selectedTransform)
    {
        if (selectedPrefab != null)
        {

            //instatiates the prefab of the game object and sets the prefab the child of the selected box
            //turns off the selected box, renderer and box collider, not visible after the prefab is placed down
            allowDelete = false;

            if (component == null)
            {
                //component = Instantiate(selectedPrefab, selectedTransform.position, selectedPrefab.transform.rotation);
            }
            else if(hover)
            { 
                component = hover.componentPrefab;
                hover.buttonLetgo = false;
            }

            component.transform.parent = selectedTransform;
            component.transform.localPosition = Vector3.zero;
            component.GetComponent<Collider>().enabled = true;

            selectedTransform.GetChild(0).GetComponent<SpriteRenderer>().sprite = originalSprite;
            selectedTransform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            //selectedTransform.GetComponent<Renderer>().enabled = false;
            selectedTransform.GetComponent<BoxCollider>().enabled = false;
            selectedTransform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            selectedTransform.GetChild(0).GetComponent<Animator>().SetBool("ObjectPlaced", true);


            //checks if chilled water pump or condensed water pump is placed and flip it if need be
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

            highlighted = null;
            component = null;
            highlightedPlacement = null;
            allowDelete = true;

            if (tutorial)
            {
                FindObjectOfType<Tutorial>().CheckPlacement();
            }
        }
        else
        //no components has been detected yet
        {
            Debug.Log("Please select a component first");
            //if there is no selected game object yet, it will set the new one based on the raycast hit, and sets the material
            if (selected == null)
            //brand new selected object(green squares)
            //since there is no previous selected, it will set the object hit by the raycast to become selectionMat, which is a darker green
            {
                selected = selectedTransform;
                selectedTransform.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = selectionSprite;
            }
            else
            //if there was a previous selected object
            //return the material back to the orignal material from selected mat
            //reapplies material to the new selected object
            {
                selected.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = originalSprite;
                selected = selectedTransform;
                selectedTransform.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = selectionSprite;
            }
            cameraMovement.allowRotation = true;
        }
    }

    private void Highlight()
    //handles turning the selected object(green boxes) yellow(highlighted)
    {
        Ray ray = new();
        if (Input.touchCount != 0)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        }

        if (Input.touchCount == 0 && hoverGroup)
        { 
            if (highlightedPlacement)
            {
                hoverGroup.dragToPlace = true;
                if (highlighted.gameObject.name == "Selection Point (2)" && selectedPrefab)
                {
                    testingText.text = "I am running";
                    autoPlace(highlighted);
                }
                else 
                {
                    Select(highlightedPlacement.transform);
                }
            }
            else
            {
                Destroy(component);
                return;
            }
        }
        
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity))
        //constantly generates a raycast from the mouse position
        //checks if the mouse is over a game object and the returns the hit object using raycast
        {
            if (hit.collider.CompareTag("Selection"))
            {
                cameraMovement.allowRotation = false;
                if (highlighted == hit.transform)
                {
                    allowDelete = false;
                    return;
                }
                else
                {
                    if (highlighted)
                    {
                        highlighted.GetChild(0).GetComponent<SpriteRenderer>().sprite = originalSprite;
                    }

                    highlighted = hit.transform;
                    hit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = highlightSprite;

                    if (hit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite == highlightSprite)
                    {
                        highlightedPlacement = hit.transform;
                    }
                }
            }
            if (InputSystem.Instance.LeftClick())
            //if there is a left click or a touch detected
            {
                if (!hit.transform.CompareTag("Selection"))
                {
                    //the game object is not the green boxes it will run the delete function
#if UNITY_ANDROID
                    Delete(hit.transform);
                    return;
#endif          
                }
                else 
                {
                    if (highlighted.gameObject.name == "Selection Point (2)")
                    {
                        autoPlace(hit.transform);
                    }
                    else
                    {
                        //if not it will run the select function
                        Select(hit.transform);
                    }
                    return;
                }
            }

            else if (Input.GetTouch(0).phase == TouchPhase.Moved && !hoverGroup.dragToPlace)
            {
                cameraMovement.allowRotation = false;
                if (!hit.transform.CompareTag("Selection"))
                {
                    //the game object is not the green boxes it will run the delete function
#if UNITY_ANDROID
                    Delete(hit.transform);
                    return;
#endif          
                }


                //if not it will run the select function
                Select(hit.transform);
                return;
            }
            //not sure if needed
            else if (hit.transform.tag.StartsWith("Component") && allowDelete && Input.touchCount > 0)
            {
                Debug.Log("ad");
                autoPlace(hit.transform);
                Delete(hit.transform);
                return;
            }
            else 
            {
                cameraMovement.allowRotation = true;
            }
#if UNITY_STANDALONE
            if (InputSystem.Instance.RightClick())
            //right click the game object to delete the game object
            {
                Delete(hit.transform);
                return;
            }
#endif
            // Change color of the component to highlight material
            if (hit.transform.CompareTag("Selection"))
            //when the raycast returns the tag of the gameobject it hits and it matches the tag for the green boxes
            {
                if (hit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite != highlightSprite && hit.transform != selected)
                //as long as the hit gameobject is not already the material for highlight or detected
                //it will change the material to the highlight material
                {
                    hit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = highlightSprite;
                }
            }
            else
            //handles returning all green boxes back to the original material except the highlighted one
            {
                foreach (GameObject selection in selections)
                {
                    // if the selection is not in the selected array, change the material back to original material
                    if (selection.transform.childCount > 1 || selection.transform != selected)
                    {
                        selection.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = originalSprite;
                    }
                }
            }
        }
        else
        //handles returning all green boxes back to the orignal mat as long as none of them are currently being hovered over by the mouse
        {
            if (deletingGO)
            {
                if (Input.touchCount == 0)
                {
                    if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out RaycastHit deletehit, Mathf.Infinity, ~(1 << 6)))
                    {
                        autoPlace();
                        deletingGO.transform.parent.GetComponent<BoxCollider>().enabled = true;
                        deletingGO.transform.parent.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                        deletingGO.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        deletingGO.transform.parent.GetChild(0).GetComponent<Animator>().SetBool("ObjectPlaced", false);
                        Destroy(deletingGO);
                        deletingGO = null;
                        cameraMovement.allowRotation = true;
                        return;
                    }
                    else 
                    {
                        deletingGO.transform.localPosition = Vector3.zero;
                        deletingGO = null;
                    }
                }
                deletingGO.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.nearClipPlane + 150));
            }
            else if (Input.touchCount <= 0)
            {
                cameraMovement.allowRotation = true;
            }
            highlighted = null;
            highlightedPlacement = null;


            if (!answerSheet1.placementChecks)
            {
                foreach (GameObject selection in selections)
                {
                    if (!selection.activeSelf)
                        continue;
                    // if the selection is not in the selected list, change the material back to original material
                    if (selected == null || selection.transform != selected)
                    {
                        selection.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = originalSprite;
                    }
                }
            }
            //if (InputSystem.Instance.LeftClick())
            ////resets the variable that stores the currently hit green box to null
            //{
            //    selected = null;
            //}
            //if (originalMat != null)
            //{
            //    GameObject[] selections = GameObject.FindGameObjectsWithTag("Selection");
            //    foreach (GameObject selection in selections)
            //    {
            //        // if the selection is not in the selected list, change the material back to original material
            //        if (selected == null || selection.transform != selected)
            //        {
            //            selection.GetComponent<MeshRenderer>().material = originalMat;
            //        }
            //    }
            //}
        }
    }

    private void autoPlace(Transform hitObject = null) 
    {
        if (!selectedPrefab)
            return;
        if (!deletableGameobject)
        {
            foreach (GameObject placementPoint in duplicationPoints)
            {
                if (hitObject.transform.tag != placementPoint.tag)
                    return;
                Select(placementPoint.transform);
            }
        }
        else if(deletableGameobject == duplicationPoints[1].transform.GetChild(1).gameObject)
        {
            foreach (GameObject placementPoint in duplicationPoints)
            {
                if (placementPoint.transform.childCount > 1 && deletableGameobject != placementPoint.transform.GetChild(1).gameObject)
                {
                    if (placementPoint.transform.childCount < 1)
                        return;
                    Destroy(placementPoint.transform.GetChild(1).gameObject);
                    
                }
            }
            return;
        }
    }

    public void Delete(Transform selectedTransform)
    //handles deletion of the component gameobjects
    {
        if (!allowDelete) 
        {
            return;
        }
        if (!cameraMovement.zooming)
        {
            //null exception
            if (!selectedTransform.gameObject)
                return;
#if UNITY_STANDALONE
        //gets the hit gamebject from the raycast(passed in through the parameter)
        //checks the tags to ensure its one of the components
        //component is a child to the green boxes
        //sets the green boxes to a visible(Renderer) and interactable(Box Collider) state
        //destroys the component
        if (selectedTransform.CompareTag("Component/Chiller") || selectedTransform.CompareTag("Component/AHU") ||
            selectedTransform.CompareTag("Component/CoolingTower") || selectedTransform.CompareTag("Component/CwpOpt") ||
            selectedTransform.CompareTag("Component/CwpOptElavated"))
        {
            selectedTransform.parent.GetComponent<Renderer>().enabled = true;
            selectedTransform.parent.GetComponent<BoxCollider>().enabled = true;
            Destroy(selectedTransform.gameObject);
        }
    
#endif
#if UNITY_ANDROID
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            //once the touch is detected
            {
                //returns the component which the gameobject matches the tag
                if (selectedTransform.CompareTag("Component/Chiller") || selectedTransform.CompareTag("Component/AHU") ||
                selectedTransform.CompareTag("Component/CoolingTower") || selectedTransform.CompareTag("Component/CwpOpt") ||
                selectedTransform.CompareTag("Component/CwpOptElavated"))
                {
                    deletingObject = true;
                    deletableGameobject = selectedTransform.gameObject;
                    deletingGO = deletableGameobject;
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            //once the finger starts moving
            {
                if (deletableGameobject == null)
                    return;
                //Set the vector finger point from the screen into a world point
                //set the gameobject we referenced to the world point
                Vector3 tempPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.nearClipPlane + 150));
                deletableGameobject.transform.position = tempPos;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            //no longer detecting the touch
            {
                // check if dragged out, if out, delete
                //null error check
                if (!deletableGameobject)
                    return;
                //creates a raycast, ignores the component layer and checks if it does not hit any object with a collider
                //if there is no collider will delete the gameobject as it is outside the boundaries
                //if the raycast hits a collider, it will return the game object to its parent transform
                deletableGameobject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.nearClipPlane + 150));
                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out RaycastHit hit, Mathf.Infinity, ~(1 << 6)))
                {
                    deletableGameobject.transform.parent.GetComponent<BoxCollider>().enabled = true;
                    deletableGameobject.transform.parent.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                    deletableGameobject.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    deletableGameobject.transform.parent.GetChild(0).GetComponent<Animator>().SetBool("ObjectPlaced", false);
                    Destroy(selectedTransform.gameObject);
                    deletingGO = null;
                    Debug.Log("destroy1");
                }
                else
                {
                    autoPlace(selectedTransform);
                    selectedTransform.gameObject.transform.localPosition = Vector3.zero;
                }
                deletingGO = null;
                deletingObject = false;
                cameraMovement.allowRotation = true;
            }
#endif
        }
    }
}
