using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class Instructions : MonoBehaviour
{
    public VideoPlayer video;

    public GameObject cameraHint;

    public List<VideoClip> clips = new List<VideoClip>();
    public List<string> tutorialText = new List<string>();
    public List<string> tutorialTitle = new List<string>();
    private int index = 0;
    public GameObject continueBtn;
    public GameObject nextBtn;
    public GameObject previousBtn;

    [SerializeField]
    private TextMeshProUGUI textArea;
    [SerializeField]
    private TextMeshProUGUI textTitleArea;

    public GameObject InstructionalPopUps;
    public static bool Read;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraHint) 
        {
            cameraHint.SetActive(false);
        }

        index = 0;
        continueBtn.SetActive(false);
        video.clip = clips[index];
        textArea.text = tutorialText[index];
        textTitleArea.text = tutorialTitle[index];

        if (index == 0)
        {
            previousBtn.SetActive(false);
        }
        if (Read == true)
        {
            InstructionalPopUps.SetActive(false);
        }

    }

    public void Next()
    {
        index++;
        if (index < clips.Count) 
        {
            video.Stop();
            video.clip = clips[index];
            video.Play();
            textArea.text = tutorialText[index];
            if ((index == 0 || index == 1) && tutorialTitle.Count != tutorialText.Count)
            {
                textTitleArea.text = tutorialTitle[0];
            }
            else if (tutorialTitle.Count == tutorialText.Count)
            {
                textTitleArea.text = tutorialTitle[index];
            }
            else 
            {
                textTitleArea.text = tutorialTitle[index - 1];
            }

            if (index == clips.Count - 1)
            {
                nextBtn.SetActive(false);
                continueBtn.SetActive(true);
            }
            else 
            {
                previousBtn.SetActive(true);
            }
        }
    }

    public void Reset()
    {
        index = 0;
        video.Stop();
        textTitleArea.text = tutorialTitle[index];
        video.clip = clips[index];
        nextBtn.SetActive(true);
        continueBtn.SetActive(false);
    }

    public void Previous() 
    {
        index--;
        if (index != -1)
        {
            previousBtn.SetActive(true);
            video.clip = clips[index];
            textArea.text = tutorialText[index];

            if ((index == 0 || index == 1) && clips.Count < 4)
            {
                textTitleArea.text = tutorialTitle[0];
            }
            else
            {
                textTitleArea.text = tutorialTitle[index];
            }
            if (index == 0)
            {
                previousBtn.SetActive(false);
            }
            else if (index < clips.Count) 
            {
                nextBtn.SetActive(true);
                continueBtn.SetActive(false);
            }
        }
    }

    public void ShowCamHint() 
    {
        cameraHint.SetActive(true);
    }
}
