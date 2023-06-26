using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentWheelButtonController : MonoBehaviour
{
    public int Id;
    private Animator anim;
    public string itemName;
    public TextMeshProUGUI itemText;
    public Image selectedItem;
    private bool selected = false;
    public Sprite icon;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            selectedItem.sprite = icon;
            itemText.text = itemName;
        }
    }

    public void Selected() 
    {
        anim.SetBool("select", true);
        anim.SetBool("hovered", false);
        selected = true;
        ComponentWheelController.componentID = Id;
        transform.SetSiblingIndex(4);
    }

    public void DeSelected()
    {
        anim.SetBool("select", false);
        selected = false;
        ComponentWheelController.componentID = 0;
        itemText.text = "";
    }

    public void HoverEnter() 
    {
        transform.SetSiblingIndex(4);
        anim.SetBool("hovered", true);
        itemText.text = itemName;
    }

    public void HoverExit()
    {
        anim.SetBool("hovered", false);
        itemText.text = "";
    }

}
