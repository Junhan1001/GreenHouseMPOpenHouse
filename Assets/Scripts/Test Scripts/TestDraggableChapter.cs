using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestDraggableChapter : MonoBehaviour
{
    public List<Transform> objects;

    [SerializeField]
    private Transform selectedObject;
    private int selectedObjectIndex = 0;

    private Vector3 behindScale = new(0.7f, 0.7f, 0.7f);
    private float time = 1f;

    private bool pointerDown = false;
    private Vector3 initialPointerPosition;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            objects.Add(transform.GetChild(i));
        }

        MakeSphereLike(objects, selectedObjectIndex, time);
    }

    private void Update()
    {
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
                MoveRight(time);
                pointerDown = false;
            }
            else if (initialPointerPosition.x - Input.mousePosition.x < 0)
            {
                // pointer moving left
                MoveLeft(time);
                pointerDown = false;
            }
        }
    }

    private void MakeSphereLike(List<Transform> objectList, int index, float time)
    {
        Transform left = null;
        Transform right = null;

        if (index == 0)
        {
            left = objectList[objectList.Count - 1];
        }
        else
        {
            left = objectList[index - 1];
        }

        if (index == objectList.Count - 1)
        {
            right = objectList[0];
        }
        else
        {
            right = objectList[index + 1];
        }

        //left.localPosition = new(-450, 0, 0);
        //left.localScale = behindScale;
        //right.localPosition = new(450, 0, 0);
        //right.localScale = behindScale;
        Color leftColor = left.GetComponent<Image>().color;
        leftColor.a = 0.7f;
        left.GetComponent<Image>().color = leftColor;
        Color rightColor = right.GetComponent<Image>().color;
        rightColor.a = 0.7f;
        right.GetComponent<Image>().color = rightColor;
        selectedObject = objectList[index];
        //selectedObject.localPosition = Vector3.zero;
        //selectedObject.localScale = Vector3.one;
        selectedObject.SetAsLastSibling();
        Color selectedColor = selectedObject.GetComponent<Image>().color;
        selectedColor.a = 1f;
        selectedObject.GetComponent<Image>().color = selectedColor;
        selectedObjectIndex = index;
        StartCoroutine(Move(left, new(-450, 0, 0), behindScale, time));
        StartCoroutine(Move(right, new(450, 0, 0), behindScale, time));
        StartCoroutine(Move(selectedObject, Vector3.zero, Vector3.one, time));
    }

    public void MoveLeft(float time = 0)
    {
        if (time == 0)
            time = this.time;
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
            time = this.time;
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
        float moveRate = (diff / time) * Time.fixedDeltaTime;
        float scale = (obj.localScale.x - endScale.x) / time * Time.fixedDeltaTime;
        Vector3 scaleRate = new(scale, scale, scale);
        while (true)
        {
            if (Vector3.Distance(obj.localPosition, end) < 1f)
            {
                obj.localPosition = end;
                yield break;
            }
            obj.localPosition = Vector3.MoveTowards(obj.localPosition, end, moveRate);
            obj.localScale -= scaleRate;
            yield return new WaitForFixedUpdate();
        }
    }
}