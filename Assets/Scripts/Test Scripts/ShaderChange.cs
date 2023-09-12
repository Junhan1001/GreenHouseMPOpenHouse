using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderChange : MonoBehaviour
{
    public GameObject gameObject1;
    public GameObject gameObject2;
    public Shader shaderRight;
    public Shader shaderWrong;
    Renderer[] testChild;

    public float thickness;
    public bool wrong;

    // Start is called before the first frame update
    void Start()
    {
        thickness = 1.07f;
        testChild = gameObject1.GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject1.GetComponent<Renderer>().material.shader = shaderRight;
        //gameObject1.GetComponent<Renderer>().sharedMaterial.SetColor("_OutlineColor", Color.green);
        //gameObject1.GetComponent<Renderer>().sharedMaterial.SetFloat("_OutlineWidth",1.07f);

        gameObject2.GetComponent<Renderer>().material.shader = shaderWrong;
        gameObject2.GetComponent<Renderer>().sharedMaterial.SetColor("_OutlineColor", Color.red);
        gameObject2.GetComponent<Renderer>().sharedMaterial.SetFloat("_OutlineWidth", 1.07f);

        foreach (Renderer child in testChild) 
        {
            if (wrong)
            {
                child.material.shader = shaderRight;
                child.sharedMaterial.SetColor("_OutlineColor", Color.red);
                child.sharedMaterial.SetFloat("_OutlineWidth", thickness);
            }
            else 
            {
                child.material.shader = shaderRight;
                child.sharedMaterial.SetColor("_OutlineColor", Color.green);
                child.sharedMaterial.SetFloat("_OutlineWidth", thickness);
            }
        }
    }
}
