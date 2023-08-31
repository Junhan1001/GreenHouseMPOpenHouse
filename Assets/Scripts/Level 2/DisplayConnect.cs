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
    }

    public void ToSelected(Image component)
    {
        ImgTo.sprite = component.sprite;
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
    }
}
