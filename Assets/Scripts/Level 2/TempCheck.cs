using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempCheck : MonoBehaviour
{
    public GameObject AnswerCheck;
    public GameObject AfterLevel;
    public Camera Camera;
    public GameObject[] TempValues;
    //public GameObject[] ValueImage;
    public Light light;
    public GameObject connection;
    public GameObject undo;
    public GameObject Lock;
    public GameObject Display;

    private bool showing;

    public string[] Temps;

    // Start is called before the first frame update
    void Start()
    {
        //Camera = null;
    }
    
    public void ReviewLevel()
    {
        
        //light.transform.localRotation = Quaternion.identity;
        Camera = Camera.main;
        Camera.transform.position = new Vector3(-350, -20, 0);
        Camera.transform.rotation = Quaternion.Euler(1,90,0);
        Camera.transform.parent.GetComponent<CameraMovement>().enabled = false;
        connection.transform.GetComponent<Connection>().enabled = false;
        Level2AnswerSheet.showPopUp = true;
        undo.SetActive(false);  
        AnswerCheck.SetActive(false);
        Lock.SetActive(false);
        Display.SetActive(false);
        AfterLevel.SetActive(true);
    }
    
    public void TemperatureCheck()
    {
        if (TempValues.Length == Temps.Length) 
        {
            if (!showing)
            {
                for (int i = 0; i < TempValues.Length; i++)
                {
                    TempValues[i].gameObject.SetActive(true);
                    //alueImage[i].gameObject.SetActive(true);
                    TextMeshProUGUI texts = TempValues[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    texts.text = Temps[i];
                }
                showing = true;
            }
            else
            {
                for (int i = 0; i < TempValues.Length; i++)
                {
                    TempValues[i].gameObject.SetActive(false);
                    //ValueImage[i].gameObject.SetActive(false);
                }
                showing = false;
            }
        }
    }
}
