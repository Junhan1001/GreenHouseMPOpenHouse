using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ReturnValue : MonoBehaviour
{
    public int indexValue;
    public SelectedComponent selectedComponentBtn;
    public Connection connection;
    public bool pressedBtn;
    Color greenish = new Color(0, 255, 0, 255);
    Color whiter = new Color(255, 255, 255, 143);

    private void Start()
    {
        connection = FindObjectOfType<Connection>();
        pressedBtn = false;
        indexValue = 0;


        foreach (Button btn in transform.GetComponentsInChildren<Button>())
        {
            btn.onClick.AddListener(delegate { ReturnConnectionPoint(); });
        }
        selectedComponentBtn = connection.selectedComponent;
        
        foreach (GameObject t in selectedComponentBtn.selectedTransform)
        {
            var mesh = t.GetComponent<MeshRenderer>().sharedMaterial;
            var connectionIndex = selectedComponentBtn.selectedTransform.IndexOf(t);
            int index = selectedComponentBtn.IndexReturning(t);
            //Debug.Log(mesh);
            //Debug.Log(t.name.Substring(t.name.Length-1,1) + "This is the Index of Connection Point from name");
            //Debug.Log((transform.GetChild(connectionIndex-1).GetComponent<Button>().gameObject.name));
            if (connection.tobeunhighlighted.Contains(t))
            {
                if (index == int.Parse(transform.GetChild(connectionIndex).GetComponent<Button>().gameObject.name))
                {
                    gameObject.transform.GetChild(index - 1).GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                gameObject.transform.GetChild(index - 1).GetComponent<Button>().interactable = true;
            }


            if (mesh == connection.selectionMat)
            {
                //Debug.Log(transform.GetChild(connectionIndex).GetComponent<Button>().gameObject.name);
                //Debug.Log(index);
                if (index == int.Parse(transform.GetChild(connectionIndex).GetComponent<Button>().gameObject.name))
                {
                    Debug.Log("reached");
                    gameObject.transform.GetChild(index-1).GetComponent<Image>().color = greenish;
                    //gameObject.transform.GetChild(index - 1).transform.GetChild(0).GetComponent<Text>().text = "In use";
                }
            }

        }
    }

    public void ReturnConnectionPoint()
    {

        if (connection.pipeWarning)
        {
            connection.pipeWarningPanel.SetActive(true);
            return;
        }
        pressedBtn = !pressedBtn;
        //string ogText = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;

        if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color != greenish)
        {
            //pressedBtn = true;
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = greenish;
            //EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text = "In use";
            indexValue = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        }
        else if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color != whiter)
        {
            Debug.Log(pressedBtn);
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = whiter;
            //EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text = ogText;
            indexValue = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        }
        scriptRef();
    }

    public int ReturnIndex() 
    {
        connection.valueReturnBtn = this;
        return indexValue;
    }

    private void scriptRef() 
    {
        if (pressedBtn)
        {
            connection.valueReturnBtn = this;
        }
    }

    
}
