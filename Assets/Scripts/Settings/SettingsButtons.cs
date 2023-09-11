using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject bgButton;
    public GameObject volumeButton;
    public GameObject sensitivityButton;
    public GameObject resetButton;
    public GameObject creditButton;

    [Header("ON / OFF")]
    public GameObject bgOn;
    public GameObject bgOff;
    public GameObject volOn;
    public GameObject volOff;

    [Header ("Displays")]
    public GameObject bgPanel;
    public GameObject volPanel;
    public GameObject sensPanel;
    public GameObject creditsPanel;

    [Space(10)]
    public GameObject errorShow;
    public GameObject btnClearLogFile;
    public GameObject btnShowLogFile;

    private GameObject previousShowed;

    // Start is called before the first frame update
    void Start()
    {
#if DEBUG
        //btnShowLogFile.SetActive(true);
        //btnClearLogFile.SetActive(true);
#endif
        //if (PlayerPrefs.GetString(Strings.Username) == "Admin")
        //{
        //    btnClearLogFile.SetActive(true);
        //}
        //ClickOnBg();
    }

    //sets the buttons for the settings when in the game to turn the buttons on
    public void ClickOnBg()
    {
        bgButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        volumeButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        sensitivityButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        creditButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        bgPanel.SetActive(true);
        volPanel.SetActive(false);
        sensPanel.SetActive(false);
        resetButton.SetActive(false);
        previousShowed = bgPanel;
    }

    //sets the panels for the volume settings to active
    public void ClickOnVolume() 
    {
        bgButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        volumeButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        sensitivityButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        creditButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        bgPanel.SetActive(false);
        volPanel.SetActive(true);
        sensPanel.SetActive(false);
        resetButton.SetActive(false);
        previousShowed = volPanel;
    }
    

    //sets the panels for the sensitivity settings to active
    public void ClickOnSensitivity() 
    {
        bgButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        volumeButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        sensitivityButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        creditButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        bgPanel.SetActive(false);
        volPanel.SetActive(false);
        sensPanel.SetActive(true);
        resetButton.SetActive(true);
        previousShowed = sensPanel;
    }

    public void ClickOnCredits()
    {
        //bgButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        //volumeButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        //sensitivityButton.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        //creditButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        //bgPanel.SetActive(false);
        //volPanel.SetActive(false);
        //sensPanel.SetActive(false);
        //resetButton.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BgOffButton() 
    {
        bgOn.SetActive(true);
        bgOff.SetActive(false);
    }

    public void BgOnButton() 
    {
        bgOn.SetActive(false);
        bgOff.SetActive(true);
    }

    public void VolOffButton()
    {
        volOn.SetActive(true);
        volOff.SetActive(false);
    }

    public void VolOnButton()
    {
        volOn.SetActive(false);
        volOff.SetActive(true);
    }

    public void ShowLogFile()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/errorLog.txt"))
        {
            try
            {
#if UNITY_STANDALONE
                Application.OpenURL("file://" + Application.persistentDataPath + "/errorLog.txt");
#endif
#if UNITY_ANDROID
            //Application.OpenURL("file://" + Application.persistentDataPath "+ "/errorLog.txt");
            string dataType = "application/txt";
            string documentURL = Application.persistentDataPath + "/errorLog.txt";
            UnityAndroidOpenUrl.AndroidOpenUrl.OpenFile(documentURL, dataType);
#endif
            }
            catch (Exception e)
            {
                Debug.Log("Failed to open error log file. " + e.Message);
            }
        }
        else
        {
            {
                StartCoroutine(ShowError());
            }
        }
    }

    private IEnumerator ShowError()
    {
        GameObject error = Instantiate(errorShow, this.transform);
        error.GetComponentInChildren<TextMeshProUGUI>().text = "No log file found";
        yield return new WaitForSecondsRealtime(2f);
        Destroy(error);
    }

    public void ClearLogFile()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/errorLog.txt"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/errorLog.txt");
        }
    }
}
