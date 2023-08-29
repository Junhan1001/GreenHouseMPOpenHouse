using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineCollision : MonoBehaviour
{
    internal Transform origin;
    internal Transform destination;

    [SerializeField]
    private GameObject line;

    [SerializeField]
    private Transform parent;
    //private bool isTriggerring = false;
    //private bool instantiatedLine = false;

    private void Start()
    {
        if (CompareTag("LineParent"))
        {
            return;
        }
        else
        {
            GameObject line = Instantiate(this.line, transform.position, Quaternion.identity);
            line.transform.localScale = transform.localScale;
            line.transform.rotation = transform.rotation;
            line.name = name;
            line.transform.parent = transform.parent;
            //instantiatedLine = true;
            Destroy(gameObject);
        }
    }
}
