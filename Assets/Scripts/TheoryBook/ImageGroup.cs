using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGroup : MonoBehaviour
{
    public List<IconTab> iconTabImages;

    public TheoryBook theoryBook;
    public InspectComponent inspectComponent;

    public Image imageLocation;
    public GameObject inspectCanvas;

    public void Subscribe(IconTab icon)
    {
        if (iconTabImages == null)
        {
            iconTabImages = new List<IconTab>();
        }

        iconTabImages.Add(icon);
    }

    public void OnTabExit(IconTab image)
    {
        ResetTabs();
    }

    public void OnTabSelected(IconTab image)
    {
        ResetTabs();
        if (!theoryBook.theoryBookComponents.GetComponent<Image>().sprite == theoryBook.selectedTab)
        {
            imageLocation.transform.parent.gameObject.SetActive(true);
            imageLocation.sprite = image.tabImage.sprite;
        }
        else 
        {
            inspectCanvas.SetActive(true);
            inspectComponent.InspectingComponent(image.tabImage.sprite.name);
        }
    }

    public void ResetTabs()
    {
        imageLocation.transform.parent.gameObject.SetActive(false);
        imageLocation.sprite = null;
        inspectCanvas.SetActive(false);
    }

}
