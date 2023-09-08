using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    //public bool settings;
    //public byte maxFade;
    //public byte fadeAmount;
    public byte fadeSpeed;
    public static bool canFade;

    private Animator fadeAnim;
    public Animator sectionAnim;

    public bool sessionEnd;
    //public byte red, green, blue;
    //Image fadeImage;
    //public bool fadeIn;
    //public bool fadeOut;
    //public bool canDarken;
    [HideInInspector]
    public bool darken;
    public bool exit;
    public Image profileImage;
    public TextMeshProUGUI username;

    public GameObject proceedButton;

    private string transitionScene, previousScene;

    public GameObject Section;
    public GameObject Level;
    public GameObject GameMenu;
    //public GameObject PreLevelItems;
    public GameObject objectiveitems;
    public static bool Previewed;

    public Button btnLevelSelect;

    [SerializeField]
    private GameObject pausePanel;
    public Animator pauseAnim;
    [SerializeField]
    private GameObject instructionPanel;

    public bool PauseCheck;

    [SerializeField]
    private Image progressionChapter1;
    [SerializeField]
    private Image progressionChapter2;
    [SerializeField]
    private Image progressionChapter3;

    [SerializeField]
    private Image badge;

    [SerializeField]
    private List<Image> chapterImages = new();

    public void ShowObjective()
    {
        if ((SceneManager.GetActiveScene().name == "Main Menu") && !Previewed)
        {
            objectiveitems.SetActive(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "CharacterSelect")
        {
            canFade = false;
        }
        else 
        {
            canFade = true;
        }


        fadeSpeed = (byte)PlayerPrefs.GetFloat("fadeValue");
        previousScene = PlayerPrefs.GetString("previousScene");
        fadeAnim = GetComponent<Animator>();

        if (fadeSpeed != 0)
        {
            fadeAnim.speed = fadeSpeed;
        }
        else
        {
            fadeAnim.speed = 1;
        }

        if (SceneManager.GetActiveScene().name == "Main Menu" && !PlayerPrefs.HasKey(Strings.Username) && !PlayerPrefs.HasKey(Strings.ProfileImage))
        {
            SceneManager.LoadScene("CharacterSelect");
            return;
        }
        else 
        {
            if (SceneManager.GetActiveScene().name == "SelectLevelChapter") 
            {
                ShowAllBadges();
            }

            //if (!PlayerPrefs.HasKey("fadeSpeed"))
            //{
            //    PlayerPrefs.SetFloat("fadeSpeed", 51);
            //}

            if (PlayerPrefs.HasKey(Strings.Username) && PlayerPrefs.HasKey(Strings.ProfileImage) && profileImage)
            {
                Texture2D texture2D = new(1, 1);
                texture2D.LoadImage(Convert.FromBase64String(PlayerPrefs.GetString(Strings.ProfileImage)));
                profileImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
                //ObjProfileImg.sprite = ProfileImage.sprite;
                //profilestuff.SetActive(false);
                //if (!objectiveitems.activeSelf)
                //    GetComponentInChildren<Fade>().GameMenu.SetActive(true);
                if(username)    
                    username.text = PlayerPrefs.HasKey(Strings.Username) ? PlayerPrefs.GetString(Strings.Username) : string.Empty;
            }

        }

        //fadeImage = GetComponent<Image>();
        //fadeImage.color = new Color32(0, 0, 0, fadeAmount);

        //if (Previewed)
        //{
        //    objectiveitems.SetActive(false);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //Darken();

        ////when fading in is true
        //// and the fade amount(fade alpha) is more than 0
        //if (fadeIn && fadeAmount > 0)
        //{
        //    //disables fading out
        //    //sets the alpha to the fade amount
        //    //reduces fade alpha over update
        //    //fadeOut = false;
        //    //fadeImage.color = new Color32(red, green, blue, fadeAmount);
        //    //fadeAmount -= fadeSpeed;

        //    if (fadeAmount <= 0)
        //    //once fade alpha is zero
        //    //disables the fading in and the fade image 
        //    {
        //        fadeIn = false;
        //        fadeImage.enabled = false;
        //    }
        //}

        //if (fadeOut && fadeAmount >= 255)
        ////when fading out of scene
        ////and fade amount(fade alpha) is less than 255(max alpha number)
        //{
        //    //disables fading in and renables the fade image
        //    //sets the fade amount as the alph
        //    //begins increasing fade alpha
        //    //fadeIn = false;
        //    //fadeImage.enabled = true;
        //    //fadeImage.color = new Color32(red, green, blue, fadeAmount);
        //    fadeAmount += fadeSpeed;
        //    if (fadeAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        //    //once fade alpha hits the limit
        //    {
        //    }
        //}
    }

    //void Darken()
    //{
    //    if (canDarken)
    //    {
    //        if (darken)
    //        {
    //            if (fadeAmount < maxFade)
    //            {
    //                fadeImage.enabled = true;
    //                fadeImage.color = new Color32(red, green, blue, fadeAmount);
    //                fadeAmount += fadeSpeed;
    //            }
    //        }
    //        else
    //        {
    //            if (fadeAmount > 0)
    //            {
    //                fadeImage.color = new Color32(red, green, blue, fadeAmount);
    //                fadeAmount -= fadeSpeed;

    //                if (fadeAmount <= 0)
    //                {
    //                    if (proceedButton != null)
    //                    {
    //                        proceedButton.SetActive(true);
    //                    }
    //                    fadeImage.enabled = false;
    //                }
    //            }
    //        }
    //    }
    //}

    public void FadeEvent() 
    {
        if (exit)
        //quits the game if true
        {
            /*Profile.ProfileSet = */
            //Instructions.Read = Previewed = false;
            //PlayerPrefs.DeleteKey("ProfileImage");
            //PlayerPrefs.DeleteKey("Username");
            //PlayerPrefs.DeleteKey("TimeProfileCreated");
            //PlayerPrefs.DeleteKey("Gender");
            //Strings.ResetProgress();
            Application.Quit();
        }
        else if (sessionEnd) 
        {
            Instructions.Read = Previewed = false;
            PlayerPrefs.DeleteKey("ProfileImage");
            PlayerPrefs.DeleteKey("Username");
            PlayerPrefs.DeleteKey("TimeProfileCreated");
            PlayerPrefs.DeleteKey("Gender");
            Strings.ResetProgress();
            SceneManager.LoadScene("CharacterSelect");
        }
        else if (!string.IsNullOrEmpty(transitionScene))
        //loads the next scene if transition scene is specificed, set in editor
        {
            SceneManager.LoadScene(transitionScene);
        }
    }

    public void Transition(string scene)
    //takes in a string as the scene destination
    {
        //if (!canFade) return;

        //resets time scale while playing click sound
        //fadeOut = true;
        Time.timeScale = 1;
        FindObjectOfType<AudioManager>()?.Play("Click");

        //specifies the scene to transition to for update
        transitionScene = scene;

        //resets fadeout and read(past tense) for instructions
        //fadeOut = true;
        fadeAnim.SetTrigger("FadeAway");
        Instructions.Read = false;
    }

    public void TransitionNoSceneChange() 
    //fades in and out but does not change the scene
    {
        FindAnyObjectByType<AudioManager>()?.Play("Click");
    }

    public void PreviousScene()
    {
        FindObjectOfType<AudioManager>().Play("Click");

        Debug.Log(PlayerPrefs.GetString("previousScene"));

        transitionScene = previousScene;
        fadeAnim.SetTrigger("FadeAway");

    }

    public void Quit()
    //quits the game after click sound
    {
        FindObjectOfType<AudioManager>().Play("Click");
        exit = true;
        fadeAnim.SetTrigger("FadeAway");
        //fadeOut = true;
    }

    public void EndSession() 
    {
        FindObjectOfType<AudioManager>().Play("Click");
        sessionEnd = true;
        fadeAnim.SetTrigger("FadeAway");
    }

    //Pause Menus
    public void PauseGame() 
    {
        if (!pausePanel.activeInHierarchy)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            pauseAnim.SetBool("Pause", true);
            PauseCheck = true;
        }
        else 
        {
            pauseAnim.SetBool("Pause", false);
            StartCoroutine(StopShowPanel(pauseAnim));
        }
    }

    private IEnumerator StopShowPanel(Animator anim)
    {
        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("PauseLeave"))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    Time.timeScale = 1;
                    pausePanel.SetActive(false);
                    PauseCheck = false;
                }
            }
            yield return null;
        }
    }

    public void InstructionBtn() 
    {
        instructionPanel.SetActive(true);
    }


    //Section Selector / Level Selector

    public void PreviewTrue()
    {
        Previewed = true;
    }

    public void PlayButton()
    {
        GameMenu.SetActive(false);
        Section.SetActive(true);
        ShowAllBadges();
    }

    public void SectionSelect(int chapter)
    {
        //open up the level select popup
        //PreLevelItems.SetActive(true);
        //Level.SetActive(true);
        Level.transform.GetChild(0).transform.Find("ImgChapterSelected").GetComponent<Image>().sprite = chapterImages[chapter].sprite;
        sectionAnim.SetBool("SlideIn", true);
        //if (!Previewed)
        //{
        //    Level.GetComponentsInChildren<UnityEngine.UI.Button>().ToList().ForEach(x =>
        //    {
        //        if (x.name != "BtnLevel0" && x.name != "Back")
        //        {
        //            x.interactable = false;
        //            Color color = x.GetComponentInChildren<TextMeshProUGUI>().color;
        //            x.GetComponentInChildren<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 0.5f);
        //        }
        //    });
        //}
        //Section.SetActive(false);
        //objectiveitems.SetActive(false);
    }

    public void LevelSelect()
    {
        if (Previewed)
        {
            //PreLevelItems.SetActive(false);
            Level.SetActive(true);
            objectiveitems.SetActive(false);
        }
    }

    public void SectionBack()
    {
        GameMenu.SetActive(true);
        Section.SetActive(false);
        objectiveitems.SetActive(false);    
    }

    public void LevelBack()
    {
        Level.SetActive(false);
        //PreLevelItems.SetActive(true);
        btnLevelSelect.interactable = true;
        objectiveitems.SetActive(false);
    }

    public void PrelevelBack()
    {
        //PreLevelItems.SetActive(false);
        //Level.SetActive(false);
        //Section.SetActive(true);
        sectionAnim.SetBool("SlideIn", false);
        ShowAllBadges();
    }

    private void ShowAllBadges()
    {
        Strings.ShowBadges(Strings.ChapterOne, Strings.ChapterOneBadgePath, progressionChapter1);
        Strings.ShowBadges(Strings.ChapterTwo, Strings.ChapterTwoBadgePath, progressionChapter2);
        Strings.ShowBadges(Strings.ChapterThree, Strings.ChapterThreeBadgePath, progressionChapter3);
    }

    public void ShowChapterTwoBadge()
    {
        Debug.Log("Show level 2 badges");
        Strings.ShowBadges(Strings.ChapterTwo, Strings.ChapterTwoBadgePath, badge);
    }

    public void ShowObject(GameObject objectToShow) 
    {
        objectToShow.SetActive(true);
    }
}
