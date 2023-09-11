using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IconTab : MonoBehaviour, IPointerClickHandler
{
    public Image tabImage;

    public ImageGroup imageGroup;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!imageGroup)
        {
            gameObject.SetActive(false);
            return;
        }
        imageGroup.OnTabSelected(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        tabImage = GetComponent<Image>();
        
        if (imageGroup)
        {
            imageGroup.Subscribe(this);
        }

    }
}
