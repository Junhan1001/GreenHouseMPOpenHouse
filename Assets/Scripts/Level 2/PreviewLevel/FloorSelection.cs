using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class FloorSelection : MonoBehaviour
{
    TMP_Dropdown dropDown;

    private List<string> floors = new() { "Building", "1<sup>st</sup> Floor", "2<sup>nd</sup> Floor", "3<sup>rd</sup> Floor" };

    public static bool FromPreviewLevel = false;
    private static int selectedFloor = 0;

    [SerializeField]
    private GameObject building;
    [SerializeField]
    private GameObject firstFloor;
    [SerializeField]
    private GameObject secondFloor;
    [SerializeField]
    private GameObject thirdFloor;

    private void Start()
    {
        dropDown = GetComponent<TMP_Dropdown>();
        dropDown.ClearOptions();
        dropDown.AddOptions(floors);
        OnFloorChange();
    }

    public void OnFloorChange()
    {
        Debug.Log("Changed floor");
        selectedFloor = dropDown.value;
        switch (selectedFloor)
        {
            case 0:
                BuildingSelected();
                break;
            case 1:
                FirstFloorSelected();
                break;
            case 2:
                SecondFloorSelected();
                break;
            case 3:
                ThirdFloorSelected();
                break;
        }
    }

    private void BuildingSelected()
    {
        building.SetActive(true);
        firstFloor.SetActive(false);
        secondFloor.SetActive(false);
        thirdFloor.SetActive(false);
    }

    private void FirstFloorSelected()
    {
        firstFloor.SetActive(true);
        building.SetActive(false);
        secondFloor.SetActive(false);
        thirdFloor.SetActive(false);
    }

    private void SecondFloorSelected()
    {
        secondFloor.SetActive(true);
        building.SetActive(false);
        firstFloor.SetActive(false);
        thirdFloor.SetActive(false);
    }

    private void ThirdFloorSelected()
    {
        thirdFloor.SetActive(true);
        building.SetActive(false);
        firstFloor.SetActive(false);
        secondFloor.SetActive(false);
    }

    public void GoToLevelSelect()
    {
        FromPreviewLevel = true;
    }

    public void EnableCameraZoom()
    {
        Camera.main.GetComponent<CameraMovement>().allowRotation = true;
        Camera.main.GetComponent<CameraMovement>().allowZoom = true;
    }

    public void DisableCameraZoom()
    {
        Camera.main.GetComponent<CameraMovement>().allowRotation = false;
        Camera.main.GetComponent<CameraMovement>().allowZoom = false;
    }
}
