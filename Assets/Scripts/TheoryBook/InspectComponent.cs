using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InspectComponent : MonoBehaviour
{
    private TheoryBook theoryBook;

    public TextAsset partDescriptionTextFile;
    [SerializeField]
    private Camera inspectCamera;
    private List<string> partsComponentList = new List<string>();
    private List<string> componentToDisplayList = new List<string>();

    int startIndex, endIndex;

    private GameObject loadedObject;
    private GameObject componentPrefab;

    public Canvas mainCanvas;

    [SerializeField]
    private Transform instantiatePoint;
    [SerializeField]
    private GameObject titlePanel, descriptionPanel;
    [SerializeField]
    private GameObject cross;
    [SerializeField]
    private TextMeshProUGUI partName, partDescription;

    private bool KeepRunning;

    // Start is called before the first frame update
    void Start()
    {
        theoryBook = FindAnyObjectByType<TheoryBook>();
        string[] lines = partDescriptionTextFile.text.Split('\n');
        partsComponentList = lines.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                //gets the previous touch position and checks the difference between the new position before applying that value as the rotation of the object
                instantiatePoint.transform.Rotate(Vector3.up, Input.GetTouch(0).deltaPosition.x, Space.World);
            }
        }
    }

    public void InspectingComponent(string prefabName) 
    {
        if (theoryBook.theoryBookComponents.GetComponent<Image>().sprite != theoryBook.selectedTab)
            return;

        mainCanvas.gameObject.SetActive(false);
        loadedObject = Resources.Load<GameObject>("Prefabs/Components/" + prefabName + "_Inspect");
        componentPrefab = Instantiate(loadedObject, instantiatePoint, false);
        componentPrefab.GetComponentInChildren<Canvas>().worldCamera = inspectCamera;

        titlePanel.SetActive(true);
        titlePanel.GetComponentInChildren<TextMeshProUGUI>().text = componentPrefab.GetComponent<InspectAttributes>().TitleText;

        componentPrefab.GetComponentInChildren<Canvas>().overrideSorting = true;
        componentPrefab.transform.localPosition = Vector3.zero;
        componentPrefab.transform.localScale *= 30f;

        foreach (Button btn in componentPrefab.GetComponentsInChildren<Button>())
        {
            btn.onClick.AddListener(delegate { DisplayDescription(); });
        }

        ComponentLinesAppend(prefabName, "End" + prefabName);
        cross.SetActive(true);
    }

    private void ComponentLinesAppend(string startString, string endString)
    //changes which part of the text file to take based on the text that seperates the segments
    {
        //Debug.Log(startString + "," + endString);
        //Debug.Log(partsComponentList.Count);
        for (int i = 0; i < partsComponentList.Count; i++)
        {
            Debug.Log("test");
            if (partsComponentList[i].StartsWith(startString))
            {
                startIndex = i + 1;
            }
            if (partsComponentList[i].StartsWith(endString))
            {
                endIndex = i;
            }
        }

        //Debug.Log(startIndex + endIndex);
        for (int i = startIndex; i < endIndex; i++)
        {
            //Debug.Log("test");
            componentToDisplayList.Add(partsComponentList[i]);
        }

    }

    private void DisplayDescription()
    {
        string ClickedBtnName = EventSystem.current.currentSelectedGameObject.name;
        if (!descriptionPanel.activeInHierarchy)
        {
            descriptionPanel.SetActive(true);
            partName.text = ClickedBtnName;
            for (int i = 0; i < componentToDisplayList.Count; i++)
            {
                //Debug.Log(componentToDisplayList[i].StartsWith("[" + ClickedBtnName + "]"));
                if (componentToDisplayList[i].StartsWith("[" + ClickedBtnName + "]"))
                {
                    int index = componentToDisplayList[i].IndexOf("]");
                    partDescription.text = componentToDisplayList[i].Substring(index + 1);
                    break;
                }
            }
        }
        else
        {
            descriptionPanel.SetActive(false);
        }
    }

    public void RemoveInspect()
    {
        loadedObject = null;
        Destroy(componentPrefab);
        descriptionPanel.SetActive(false);
        cross.SetActive(false);
        titlePanel.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
    }

}
