using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Level1Reward : MonoBehaviour
{

    public TextAsset level1RewardText;
    public GameObject instantiatePoint;
    public Camera inspectCamera;
    public LayerMask layerToCheck;

    [SerializeField]
    private GameObject LoadedObject;
    public Canvas canvasToHide;
    public TextMeshProUGUI componentTitle, partName;
    public GameObject titlePanel, panel, cross, backgroundPanel;
    public TextMeshProUGUI description;
    private GameObject inspectPrefab;

    private string[] tagArray;

    private List<string> lvl1RewardList = new List<string>();
    private List<string> descriptionList = new List<string>();

    private int descriptionListStart = 0;
    private int descriptionListEnd = 0;

    // Start is called before the first frame update
    void Start()
    {
        inspectCamera.gameObject.SetActive(false);

        string[] lines = level1RewardText.text.Split('\n');
        lvl1RewardList = lines.ToList();

        GetComponent<Level1Reward>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

        InspectComponent();
    }

    private void InspectComponent()
    {
        Ray ray = new();
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!inspectCamera.gameObject.activeSelf)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
            else
            {
                ray = inspectCamera.ScreenPointToRay(Input.GetTouch(0).position);
            }
        }

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerToCheck))
        {
            foreach (Touch touch in Input.touches) 
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) 
                {
                    return;
                }
            }

            inspectCamera.gameObject.SetActive(true);
            backgroundPanel.SetActive(true);


            if (hit.transform.tag.StartsWith("Component") && !LoadedObject) 
            {
                tagArray = hit.transform.tag.Split("/");
                Debug.Log("Prefabs/Components/" + tagArray[1] + "_Inspect");
                LoadedObject = Resources.Load<GameObject>("Prefabs/Components/" + tagArray[1] + "_Inspect");
            }
            if (LoadedObject && instantiatePoint.transform.childCount < 1)
            {
                inspectPrefab = Instantiate(LoadedObject, instantiatePoint.transform.position, Quaternion.identity);
                canvasToHide.gameObject.SetActive(false);
                inspectPrefab.GetComponentInChildren<Canvas>().worldCamera = inspectCamera;
                titlePanel.SetActive(true);
                componentTitle.text = inspectPrefab.GetComponent<InspectAttributes>().TitleText;

                inspectPrefab.transform.parent = instantiatePoint.transform;
                inspectPrefab.GetComponentInChildren<Canvas>().overrideSorting = true;
                inspectPrefab.transform.localPosition = Vector3.zero;

                foreach (Button btn in inspectPrefab.GetComponentsInChildren<Button>()) 
                {
                    btn.onClick.AddListener(delegate { DisplayDescription(); });
                }

                ComponentLinesAppend(tagArray[1], "End" + tagArray[1]);
                cross.SetActive(true);
            }
        }

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                //gets the previous touch position and checks the difference between the new position before applying that value as the rotation of the object
                instantiatePoint.transform.Rotate(Vector3.up, Input.GetTouch(0).deltaPosition.x, Space.World);
            }
        }

    }

    private void ComponentLinesAppend(string startString, string endString)
    //changes which part of the text file to take based on the text that seperates the segments
    {
        for (int i = 0; i < lvl1RewardList.Count; i++)
        {
            if (lvl1RewardList[i].StartsWith(startString))
            {
                descriptionListStart = i + 1;
            }
            if (lvl1RewardList[i].StartsWith(endString))
            {
                descriptionListEnd = i;
            }
        }

        for (int i = descriptionListStart; i < descriptionListEnd; i++)
        {
            descriptionList.Add(lvl1RewardList[i]);
        }

    }

    private void DisplayDescription()
    {
        string ClickedBtnName = EventSystem.current.currentSelectedGameObject.name;
        if (!panel.activeInHierarchy)
        {
            panel.SetActive(true);
            partName.text = ClickedBtnName;
            for (int i = 0; i < descriptionList.Count; i++)
            {
                //Debug.Log(descriptionList[i].StartsWith("[" + ClickedBtnName + "]"));
                if (descriptionList[i].StartsWith("[" + ClickedBtnName + "]"))
                {
                    int index = descriptionList[i].IndexOf("]");
                    description.text = descriptionList[i].Substring(index + 1);
                    break;
                }
            }
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void RemoveInspect() 
    {
        LoadedObject = null;
        Destroy(inspectPrefab);
        panel.SetActive(false);
        cross.SetActive(false);
        titlePanel.SetActive(false);
        backgroundPanel.SetActive(false);
        inspectCamera.gameObject.SetActive(false);
        canvasToHide.gameObject.SetActive(true);
    }
}
