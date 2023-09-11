using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayConnect : MonoBehaviour
{
    public Connection connection;


    public Image ImgFrom;
    public Text FromTxt;

    public Image ImgTo;
    public Text ToTxt;

    public Image PipeSelected;
    public Text PipeText;

    public Sprite transparentSprite;

    private void Awake()
    {
        ImgFrom.sprite = transparentSprite;
        ImgTo.sprite = transparentSprite;
        PipeSelected.sprite = transparentSprite;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    //Debug.Log(connection.selectedComponent + "," + connection.points.Count);
    //    if (connection.points.Count == 0 && !connection.selectedComponent)
    //    {
    //        Reset();
    //    }
    //}

    public void FromSelected(Image component)
    {
        ImgFrom.sprite = component.sprite;
        if (component.gameObject.name == "AHUPanel(Clone)")
        {
            FromTxt.text = "AHU";
        }
        else if (component.gameObject.name == "ChillerPanel(Clone)")
        {
            FromTxt.text = "Chiller";
        }
        else if (component.gameObject.name == "CWPPanel(Clone)")
        {
            FromTxt.text = "CWP";
        }
        else if (component.gameObject.name == "CHWPPanel(Clone)")
        {
            FromTxt.text = "CHWP";
        }
        else if (component.gameObject.name == "CoolTowerPanel(Clone)")
        {
            FromTxt.text = "Cooling Tower";
        }
    }

    public void ToSelected(Image component)
    {
        ImgTo.sprite = component.sprite;
        if (component.gameObject.name == "AHUPanel(Clone)")
        {
            ToTxt.text = "AHU";
        }
        else if (component.gameObject.name == "ChillerPanel(Clone)")
        {
            ToTxt.text = "Chiller";
        }
        else if (component.gameObject.name == "CWPPanel(Clone)")
        {
            ToTxt.text = "CWP";
        }
        else if (component.gameObject.name == "CHWPPanel(Clone)")
        {
            ToTxt.text = "CHWP";
        }
        else if (component.gameObject.name == "CoolTowerPanel(Clone)")
        {
            ToTxt.text = "Cooling Tower";
        }
    }

    public void PipeClicked(Image pipe)
    {
        PipeSelected.sprite = pipe.sprite;
    }

    public void ResetDisplayConnect()
    {
        //PipeSelected.sprite = transparentSprite;
        ImgFrom.sprite = transparentSprite;
        ImgTo.sprite = transparentSprite;
        ToTxt.text = "Component 2";
        FromTxt.text = "Component 1";
    }
}
