using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    bool hoverOver;
    bool hoverTab;
    Vector3 startSize;
    Vector3 startPosition;
    public bool isTab;
    public float moveSpeed;
    public Vector3 desiredPosition;
    public Vector3 desiredSize;
    public GameObject component;
    public GameObject componentName;
    Placement placement;

    void Start()
    {
        placement = FindObjectOfType<Placement>();

        startPosition = transform.localPosition;
        startSize = transform.localScale;
    }
    void Update()
    {
        if (hoverOver)
        {
            if(transform.localScale.x < desiredSize.x)
            {
                transform.localScale += new Vector3(.1f, .1f, .1f);
            }
        }
        else
        {
            if (transform.localScale.x > startSize.x)
            {
                transform.localScale -= new Vector3(.1f, .1f, .1f);
            }
        }

        if (isTab)
        {
            if (hoverTab)
            {
                if (transform.localPosition.x < desiredPosition.x)
                {
                    transform.position += new Vector3(moveSpeed, 0f, 0f) * Time.deltaTime * 5;
                }
            }
            else
            {
                if (transform.localPosition.x > startPosition.x)
                {
                    transform.position -= new Vector3(moveSpeed, 0f, 0f) * Time.deltaTime * 5;
                }
            }
        }
    }
    public void Enter()
    {
        Debug.Log("hover");
        hoverOver = true;
        if (componentName != null)
        {
            componentName.SetActive(true);
        }
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    public void Exit()
    {
        Debug.Log("exit");
        hoverOver = false;
        if (componentName != null)
        {
            componentName.SetActive(false);
        }
    }

    public void TabEnter()
    {
        hoverTab = true;
    }

    public void TabExit()
    {
        hoverTab = false;
    }

    public void Selection()
    {
        if (!isTab)
        {
            placement.selectedPrefab = component;
        }
    }
}