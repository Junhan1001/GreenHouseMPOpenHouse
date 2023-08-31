using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TheoryTab : MonoBehaviour, IPointerClickHandler
{
    public TheoryTabGroup tabGroup;

    public Image tabImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        tabImage = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void GoToAnswers()
    {
        tabGroup.OnTabSelected(this);
    }
}
