using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlidingChapterAnimation : MonoBehaviour
{
    public List<Transform> objects;
    public List<bool> chapterAvailable;

    [Header("Adjustable Values")]
    public float leftPosition;
    public float rightPosition;
    public float verticalPosition, centreVerticalPosition;
    public float initialSize = 1f;
    public float smallerSize = 0.7f;

    private Vector3 centreScale;
    private Vector3 objecthiddenScale;
    public float rotateSpeed = 1f;

    [Header("Debug Purposes")]
    [SerializeField]
    private Transform selectedObject;
    public int selectedObjectIndex = 0;

    private bool pointerDown = false;
    private Vector3 initialPointerPosition;

    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        int availableChapterCount = 0;
        foreach (bool available in chapterAvailable)
        {
            // if the chapter is unavailable, make the unavailable text active
            if (!available)
                objects[index].GetChild(objects[index].childCount - 1).gameObject.SetActive(true);
            index++;
            if (available)
            {
                // if the chapter is available, make the chapter available count increase by 1
                availableChapterCount++;
                // for the first available chapter, set the selected object to be the first available chapter
                if (availableChapterCount == 1)
                {
                    selectedObjectIndex = chapterAvailable.IndexOf(available);
                }
            }
        }

        centreScale = new Vector3(initialSize, initialSize, initialSize);
        objecthiddenScale = new Vector3(smallerSize, smallerSize, smallerSize);
        MakeSphereLike(objects, selectedObjectIndex, rotateSpeed);
    }
    
    private void Update()
    {
#if UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            initialPointerPosition = Input.mousePosition;
            pointerDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            pointerDown = false;
        }
        if (pointerDown)
        {
            StopAllCoroutines();
            if (initialPointerPosition.x - Input.mousePosition.x > 0)
            {
                // pointer moving right
                MoveRight(rotateSpeed);
                pointerDown = false;
            }
            else if (initialPointerPosition.x - Input.mousePosition.x < 0)
            {
                // pointer moving left
                MoveLeft(rotateSpeed);
                pointerDown = false;
            }
        }
#endif
#if UNITY_ANDROID
        if (Input.touchCount <= 0) return;

        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            initialPointerPosition = Input.GetTouch(0).position;
            pointerDown = true;
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            pointerDown = false;
        }

        if (pointerDown)
        {
            StopAllCoroutines();
            // Get the first touch's delta position
            Vector2 currentDeltaPosition = Input.GetTouch(0).position;

            // Compare with the previous delta position
            if (currentDeltaPosition.x > initialPointerPosition.x)
            {
                // pointer moving right
                MoveLeft(rotateSpeed);
                pointerDown = false;
            }
            else if (currentDeltaPosition.x < initialPointerPosition.x)
            {
                // pointer moving left
                MoveRight(rotateSpeed);
                pointerDown = false;
            }

            // Store the current delta position for the next frame
            initialPointerPosition = currentDeltaPosition;
        }
#endif

    }

    private void MakeSphereLike(List<Transform> objectList, int index, float time)
    //this function handles making the illusion of the object retaining the sphere like movement
    {
        Transform left = null;
        Transform right = null;

        if (index == 0)
        //if the index is at the beginning of the list the left most object would be the last item in the list
        {
            left = objectList[objectList.Count - 1];
        }
        else
        //else the left would be the object before the current index
        {
            left = objectList[index - 1];
        }

        if (index == objectList.Count - 1)
        //if the index is at the last object in the list, the right most object would be the first item in the list
        {
            right = objectList[0];
        }
        else
        //else it would be the object after the current index
        {
            right = objectList[index + 1];
        }

        //Left image, sets the new alpha
        Color leftColor = left.GetComponent<Image>().color;
        leftColor.a = 0.7f;
        left.GetComponent<Image>().color = leftColor;
        left.GetComponentInChildren<Button>().interactable = false;

        //Right image, sets the new alpha
        Color rightColor = right.GetComponent<Image>().color;
        rightColor.a = 0.7f;
        right.GetComponent<Image>().color = rightColor;
        right.GetComponentInChildren<Button>().interactable = false;

        //whichever object is the index on, priotizes showing that object
        selectedObject = objectList[index];
        selectedObject.SetAsLastSibling();
        selectedObject.GetComponentInChildren<Button>().interactable = true;

        //increases the one which is at the fronts alpha to max
        Color selectedColor = selectedObject.GetComponent<Image>().color;
        selectedColor.a = 1f;

        //sets the new colour to the object
        selectedObject.GetComponent<Image>().color = selectedColor;
        selectedObjectIndex = index;

        //left position
        StartCoroutine(Move(left, new(leftPosition, verticalPosition, 0), objecthiddenScale, time));
        //right position
        StartCoroutine(Move(right, new(rightPosition, verticalPosition, 0), objecthiddenScale, time));
        //centre position
        StartCoroutine(Move(selectedObject, new(0, centreVerticalPosition, 0), centreScale, time));
    }

    public void MoveLeft(float time = 0)
    {
        if (time == 0)
            time = this.rotateSpeed;
        StopAllCoroutines();
        if (selectedObjectIndex == 0)
        {
            MakeSphereLike(objects, objects.Count - 1, time);
        }
        else
        {
            MakeSphereLike(objects, selectedObjectIndex - 1, time);
        }
    }

    public void MoveRight(float time = 0)
    {
        if (time == 0)
            time = this.rotateSpeed;
        StopAllCoroutines();
        if (selectedObjectIndex == objects.Count - 1)
        {
            MakeSphereLike(objects, 0, time);
        }
        else
        {
            MakeSphereLike(objects, selectedObjectIndex + 1, time);
        }
    }

    private IEnumerator Move(Transform obj, Vector3 end, Vector3 endScale, float time)
    {
        float diff = Vector3.Distance(obj.localPosition, end);
        //the distance between the current object position and the end location
        float moveSpeed = (diff / time) * Time.fixedDeltaTime;
        //adjusts the movement of objects(illusion of rotation)
        float scale = (obj.localScale.x - endScale.x) / time * Time.fixedDeltaTime;
        //adjusts the scale appropriately with the time
        Vector3 scaleRate = new(scale, scale, scale);
        //some error ig
        while (true)
        {
            if (Vector3.Distance(obj.localPosition, end) < 1f)
            {
                //once the distance between the object and the end location reaches a certain thershold it fixes it and breaks the loop
                obj.localPosition = end;
                yield break;
            }
            //continously moves towards the final destination while adjusting the scale 
            obj.localPosition = Vector3.MoveTowards(obj.localPosition, end, moveSpeed);
            obj.localScale -= scaleRate;
            yield return new WaitForFixedUpdate();
        }
    }
}
