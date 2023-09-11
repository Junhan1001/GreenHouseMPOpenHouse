using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Credits : MonoBehaviour
{
    ScrollRect scrollRect;
    TextAsset textCredit;
    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        textCredit = Resources.Load<TextAsset>("TextFiles/TextCredits");
        
        scrollRect = GetComponentInChildren<ScrollRect>();
        text = scrollRect.content.GetComponentInChildren<TextMeshProUGUI>();
        text.text = textCredit.text;
        scrollRect.content.GetComponent<GridLayoutGroup>().cellSize = new(1000, text.preferredHeight + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE
        if (Input.anyKeyDown)
#endif
#if UNITY_ANDROID
        if (Input.touchCount > 0)
#endif
        {
            gameObject.SetActive(false);
        }

        Debug.Log("scrolling");
        scrollRect.verticalNormalizedPosition -= 0.0005f;
        if (scrollRect.verticalNormalizedPosition <= 0)
        {
            gameObject.SetActive(false);
            scrollRect.verticalNormalizedPosition = 1;
        }
    }
}
