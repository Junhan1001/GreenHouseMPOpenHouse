using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    private AudioManager audioManager;
    private CameraRotate cameraRotate;

    public TMP_Dropdown background;
    public TMP_Dropdown musicDrop;

    public Slider rotate;
    public Slider zoomSpeed;
    public Slider zoomSensitivity;
    public Slider fadeSlider;

    public Slider music;
    public Slider sound;

    public int defaultBackground;
    public Image skyboxThumbail;

    private int[] fadeSpeedArray = new[] { 1, 5, 51, 255 };

    [SerializeField]
    private Sprite[] skyboxThumbnails;

    // Start is called before the first frame update
    void Start()
    {
        skyboxThumbnails = Resources.LoadAll<Sprite>("SkyboxThumb");

        audioManager = FindObjectOfType<AudioManager>();
        cameraRotate = FindObjectOfType<CameraRotate>();
        background.value = PlayerPrefs.GetInt("backgroundIndex");
        musicDrop.value = PlayerPrefs.GetInt("musicIndex");

        SetBackground();

        if (!PlayerPrefs.HasKey("fadeValue"))
        {
            fadeSlider.value = 2f;
            PlayerPrefs.SetFloat("fadeValue", 2f);
        }
        else
            fadeSlider.value = PlayerPrefs.GetFloat("fadeValue");
    }

    public void SetMusic()
    {
        PlayerPrefs.SetInt("musicIndex", musicDrop.value);
        FindObjectOfType<AudioManager>().Stop(PlayerPrefs.GetString("musicName") + " (Music)");

        for(int i = 0; i < musicDrop.options.Count; i++)
        {
            if(i == PlayerPrefs.GetInt("musicIndex"))
            {
                PlayerPrefs.SetString("musicName", musicDrop.options[i].text);
            }
        }

        audioManager.Play(PlayerPrefs.GetString("musicName") + " (Music)");
    }

    public void SetBackground()
    {
        Debug.Log("set");
        PlayerPrefs.SetInt("backgroundIndex", background.value);
        skyboxThumbail.sprite = skyboxThumbnails[background.value];

        cameraRotate.CheckSkybox();
    }

    public void SetMusicVolume()
    {
        PlayerPrefs.SetFloat("musicValue", music.value);

        if (audioManager)
        {
            audioManager.UpdateVolume();
        }
    }

    public void SetSoundVolume()
    {
        PlayerPrefs.SetFloat("soundValue", sound.value);

        if (audioManager) 
        {
            audioManager.UpdateVolume();
        }
    }

    public void SetRotateSpeed()
    {
        PlayerPrefs.SetFloat("rotationSpeed", rotate.value);
    }

    public void SetZoomSpeed()
    {
        PlayerPrefs.SetFloat("zoomSpeed", zoomSpeed.value);
    }

    public void SetZoomSensitivity()
    {
        PlayerPrefs.SetFloat("zoomSensitivity", zoomSensitivity.value);
    }

    //public void SetFadeSpeed()
    //{
    //    fadeSlider.transform.parent.transform.parent.GetChild(fadeSlider.transform.parent.childCount + 1).GetComponent<Fade>().fadeSpeed =
    //        (byte)fadeSpeedArray[Mathf.RoundToInt(fadeSlider.value)];
    //    PlayerPrefs.SetFloat("fadeValue", fadeSlider.value);
    //    PlayerPrefs.SetFloat("fadeSpeed", fadeSpeedArray[Mathf.RoundToInt(fadeSlider.value)]);
    //}

    public void ResetValues()
    //resets all the slider values to the default values
    {
        //sets the base numbers for all the sliders
        PlayerPrefs.SetFloat("rotationSpeed", 60);
        PlayerPrefs.SetFloat("zoomSpeed", 75);
        PlayerPrefs.SetFloat("zoomSensitivity", 5);
        PlayerPrefs.SetFloat("fadeSpeed", 51);

        rotate.value = PlayerPrefs.GetFloat("rotationSpeed");
        zoomSpeed.value = PlayerPrefs.GetFloat("zoomSpeed");
        zoomSensitivity.value = PlayerPrefs.GetFloat("zoomSensitivity");
        fadeSlider.value = PlayerPrefs.GetFloat("fadeValue");
        fadeSlider.transform.parent.transform.parent.GetChild(fadeSlider.transform.parent.childCount + 1).GetComponent<Fade>().fadeSpeed =
            (byte)PlayerPrefs.GetFloat("fadeSpeed");
    }

}
