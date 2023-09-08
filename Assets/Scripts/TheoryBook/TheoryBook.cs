using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class TheoryBook : MonoBehaviour
//Uses TheoryBookFormat.txt to process text info to place into the theorybook
//make sure there is always an image for both the left and right pages before adding a new page
//Avoid & and | inside your titles and description
{

    private int currentLine;
    private int currentImage;

    private List<string> theoryBookList = new List<string>();
    private List<string> componentsLines = new List<string>();

    private int componentStartLine = 0;
    private int componentEndLine = 0;

    #region TextFile
    [Header("Theory Book Text Reference")]
    [Tooltip("Found at Resources/TextFiles")]
    [SerializeField]
    private TextAsset theoryBookFormat;
    #endregion

    #region Theory Book Components
    [Header("Theory Book")]
    public GameObject theoryBookTemplate;
    public Image theoryBookComponents, theoryBookPipes, theoryBookDiagrams;
    public Sprite selectedTab, unselectedTab;

    public GameObject previousBtn, nextBtn;

    public TextMeshProUGUI leftHeader, rightHeader;
    #endregion

    #region Page Components
    [Header("Pages")]
    [SerializeField]
    private List<Sprite> componentSprite = new List<Sprite>();
    [SerializeField]
    private List<Sprite> pipeSprite = new List<Sprite>();
    [SerializeField]
    private List<Sprite> diagramSprite = new List<Sprite>();

    public TextMeshProUGUI leftTitle, rightTitle;
    public TextMeshProUGUI leftDescription, rightDescription;
    public Image leftIamge, rightImage;

    private string[] leftArray;
    private string[] rightArray;
    private string[] leftRightArray;
    #endregion

    [Header("Poloroid Frame")]
    public GameObject leftFrame;

    [Header("For Main Menu Use")]
    [SerializeField] private GameObject quitBtn;

    private void Start()
    {
        string[] lines = theoryBookFormat.text.Split('\n');
        theoryBookList = lines.ToList();

        Initialize();
    }

    public void OpenTheoryBook()
    //open theory book
    {
        if (quitBtn)
        //closes the quit button that is in main menu
        { 
            quitBtn.SetActive(false);
        }

        theoryBookTemplate.SetActive(true);
    }

    public void CloseTheoryBook()
    //close theorybook
    {
        if (quitBtn)
        //turns back on the quit button in main menu
        {
            quitBtn.SetActive(true);
        }

        //TheoryBookComponent.SetActive(false);
        //TheoryBookPipes.SetActive(false);
        theoryBookTemplate.SetActive(false);
    }

    public void NextPage() 
    //updates the name, description and image when incrementing upwards in pages
    {
        currentLine++;
        currentImage += 2;

        if (!previousBtn.activeInHierarchy)
            previousBtn.SetActive(true);

        leftRightArray = componentsLines[currentLine].Split(" & ");

        leftArray = leftRightArray[0].Split(" | ");
        rightArray = leftRightArray[1].Split(" | ");

        TextAssign(leftArray, leftTitle, leftDescription);
        TextAssign(rightArray, rightTitle, rightDescription);

        if (theoryBookComponents.GetComponent<Image>().sprite == selectedTab)
        {
            ImageAssign(leftIamge, componentSprite, currentImage);
            SetRightPageBlank(componentSprite);
        }
        else if(theoryBookPipes.GetComponent<Image>().sprite == selectedTab)
        {
            ImageAssign(leftIamge, pipeSprite, currentImage);
            SetRightPageBlank(pipeSprite);
        }
        else if (theoryBookDiagrams.GetComponent<Image>().sprite == selectedTab)
        {
            ImageAssign(leftIamge, diagramSprite, currentImage);
            SetRightPageBlank(diagramSprite);
        }
    }

    public void PreviousPage() 
    //updates the name, description and image when incrementing downwards in pages
    {
        currentLine--;
        currentImage -= 2;

        if (currentLine - 1 < 0)
        {
            previousBtn.SetActive(false);
        }
        if (!nextBtn.activeInHierarchy)
        {
            nextBtn.SetActive(true);
            leftFrame.SetActive(true);
            rightImage.gameObject.SetActive(true);
        }

        leftRightArray = componentsLines[currentLine].Split(" & ");

        leftArray = leftRightArray[0].Split(" | ");
        rightArray = leftRightArray[1].Split(" | ");

        TextAssign(leftArray, leftTitle, leftDescription);
        TextAssign(rightArray, rightTitle, rightDescription);

        if (theoryBookComponents.GetComponent<Image>().sprite == selectedTab)
        {
            rightHeader.text = "COMPONENTS";
            ImageAssign(leftIamge, componentSprite, currentImage);
            ImageAssign(rightImage, componentSprite, currentImage + 1);
        }
        else if (theoryBookPipes.GetComponent<Image>().sprite == selectedTab)
        {
            rightHeader.text = "PIPES";
            ImageAssign(leftIamge, pipeSprite, currentImage);
            ImageAssign(rightImage, pipeSprite, currentImage + 1);
        }
        else if (theoryBookDiagrams.GetComponent<Image>().sprite == selectedTab) 
        {
            rightHeader.text = "DIAGRAMS";
            ImageAssign(leftIamge, diagramSprite, currentImage);
            ImageAssign(rightImage, diagramSprite, currentImage + 1);
        }
    }

    private void TextAssign(string[] array, TextMeshProUGUI title, TextMeshProUGUI description) 
    //assigns the text bassed on specific "checkpoints" in the array
    {
        foreach (string item in array)
        {
            if (item.Contains("ComponentName"))
            {
                int index = item.IndexOf(":");
                title.text = item.Substring(index + 1);

            }
            else if (item.Contains("ComponentDescription"))
            {
                int index = item.IndexOf(":");
                description.text = item.Substring(index + 1);
            }
        }
    }

    private void ImageAssign(Image image, List<Sprite> spriteList, int index)
    //assings images based on paramteres
    {
        image.sprite = spriteList[index];
    }

    public void Initialize() 
    //for initializing the theorybook whenever changing scenes or changing books
    {
        componentsLines.Clear();
        currentLine = 0;
        currentImage = 0;
        previousBtn.SetActive(false);
        nextBtn.SetActive(true);
        leftFrame.SetActive(true);
        rightImage.gameObject.SetActive(true);

        if (theoryBookComponents.GetComponent<Image>().sprite == selectedTab)
        {
            ComponentLinesAppend("Components", "EndComponents");
        }
        else if (theoryBookPipes.GetComponent<Image>().sprite == selectedTab)
        {
            ComponentLinesAppend("Pipes", "EndPipes");
        }
        else if (theoryBookDiagrams.GetComponent<Image>().sprite == selectedTab) 
        {
            ComponentLinesAppend("Diagrams", "EndDiagrams");
        }

        leftRightArray = componentsLines[currentLine].Split(" & ");

        leftArray = leftRightArray[0].Split(" | ");
        rightArray = leftRightArray[1].Split(" | ");

        TextAssign(leftArray, leftTitle, leftDescription);
        TextAssign(rightArray, rightTitle, rightDescription);

        if (theoryBookComponents.GetComponent<Image>().sprite == selectedTab)
        {
            leftHeader.text = rightHeader.text = "COMPONENTS";
            ImageAssign(leftIamge, componentSprite, currentImage);
            SetRightPageBlank(componentSprite);
        }
        else if(theoryBookPipes.GetComponent<Image>().sprite == selectedTab)
        {
            leftHeader.text = rightHeader.text = "PIPES";
            ImageAssign(leftIamge, pipeSprite, currentImage);
            SetRightPageBlank(pipeSprite);
        }
        else if (theoryBookDiagrams.GetComponent<Image>().sprite == selectedTab)
        {
            leftHeader.text = rightHeader.text = "DIAGRAMS";
            ImageAssign(leftIamge, diagramSprite, currentImage);
            SetRightPageBlank(diagramSprite);
        }
    }

    private void ComponentLinesAppend(string startString, string endString)
    //changes which part of the text file to take based on the text that seperates the segments
    {
        for (int i = 0; i < theoryBookList.Count; i++)
        {
            if (theoryBookList[i].StartsWith(startString))
            {
                componentStartLine = i + 1;
            }
            if (theoryBookList[i].StartsWith(endString))
            {
                componentEndLine = i;
            }
        }

        for (int i = componentStartLine; i < componentEndLine; i++)
        {
            componentsLines.Add(theoryBookList[i]);
        }

    }

    private void SetRightPageBlank(List<Sprite> spriteList) 
    {
        if (currentLine + 1 > componentsLines.Count - 1)
        {
            nextBtn.SetActive(false);
        }

        if (currentImage + 2 <= spriteList.Count)
        {
            ImageAssign(rightImage, spriteList, currentImage + 1);
        }
        else
        {
            rightHeader.text = "";
            leftFrame.SetActive(false);
            rightImage.gameObject.SetActive(false);
        }
    }

}
