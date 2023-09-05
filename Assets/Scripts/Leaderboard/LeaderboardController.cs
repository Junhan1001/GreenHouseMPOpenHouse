using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class SerializableList<T>
{
    public List<T> list;
}

public class LeaderboardController : MonoBehaviour
{
    //if profile image not showing, start scene from where the profile script is then go to leaderboard

    public Transform playerEntryContainer;
    public GameObject playerEntryPrefab;

    public Image profilePicture;
    public TextMeshProUGUI NameText;
    public Image[] badge;

    [SerializeField]
    private SerializableList<PlayerData> serializablePlayerDataList = new();
    private const string PlayerDataFileName = "playerdata.json";
    private Sprite profileSprite;

    private Coroutine scrollToTopCoroutine;
    private Coroutine scrollToBottomCoroutine;
    private ScrollRect scrollRect;

    private string path;

    // Load saved player data from JSON file
    private void LoadPlayerData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            //List<PlayerData> data = JsonUtility.FromJson<List<PlayerData>>(json);
            //playerDataList = new(data);
            serializablePlayerDataList = JsonUtility.FromJson<SerializableList<PlayerData>>(json);
        }
    }

    // Save current player data to JSON file
    private void SavePlayerData()
    {
        string json = JsonUtility.ToJson(serializablePlayerDataList);
        File.WriteAllText(path, json);
    }

    // Method to add a player's data to the leaderboard
    public void AddPlayerDataToLeaderboard(string playerName, int[] chapterProgressions, string gender)
    {
        PlayerData playerData = new()
        {
            playerName = playerName,
            chapterProgression = chapterProgressions,
            gender = gender
        };
        serializablePlayerDataList.list.Add(playerData);
        SavePlayerData();
    }

    // Instantiate and populate player entry prefabs in the scroll view
    private void PopulateLeaderboard()
    {
        ClearLeaderboard();

        int positionIndex = 1;

        foreach (PlayerData playerData in serializablePlayerDataList.list)
        {
            GameObject playerEntry = Instantiate(playerEntryPrefab, playerEntryContainer, false);
            TextMeshProUGUI playerNameText = playerEntry.transform.GetChild(0).Find("Body").GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI positionText = playerEntry.transform.GetChild(0).Find("Position").GetComponentInChildren<TextMeshProUGUI>();
            Image profileImage = playerEntry.transform.GetChild(0).Find("Body").GetComponentsInChildren<Image>()[1];

            Image badge1 = playerEntry.transform.GetChild(0).Find("Body").Find("Badges").GetComponentsInChildren<Image>()[0];
            Image badge2 = playerEntry.transform.GetChild(0).Find("Body").Find("Badges").GetComponentsInChildren<Image>()[1];
            Image badge3 = playerEntry.transform.GetChild(0).Find("Body").Find("Badges").Find("Chapter3Badge").GetComponentsInChildren<Image>()[0];

            switch (playerData.gender)
            {
                case "Male":
                    profileSprite = Resources.Load<Sprite>("Game UI/MaleWorkerPortrait");
                    break;
                case "Female":
                    profileSprite = Resources.Load<Sprite>("Game UI/FemaleWorkerPortrait");
                    break;
                default:
                    Debug.Log("No gender saved");
                    profileSprite = Resources.Load<Sprite>("Texture/TransparentSprite");
                    break;
            }
            
            profileImage.sprite = profileSprite;
            playerNameText.text = playerData.playerName;
            positionText.text = positionIndex.ToString();

            Strings.ShowBadges(Strings.ChapterOne, playerData.chapterProgression[0], Strings.ChapterOneBadgePath, badge1);
            Strings.ShowBadges(Strings.ChapterTwo, playerData.chapterProgression[1], Strings.ChapterTwoBadgePath, badge2);
            Strings.ShowBadges(Strings.ChapterThree, playerData.chapterProgression[2], Strings.ChapterThreeBadgePath, badge3);

            AssignToIntro(profileSprite, playerData);

            positionIndex++;
        }
    }

    // Clear previous player entry prefabs
    private void ClearLeaderboard()
    {
        foreach (Transform child in playerEntryContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // Method to display the leaderboard
    public void DisplayLeaderboard()
    {
        PopulateLeaderboard();
    }

    // Example usage
    private void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        path = Path.Combine(Application.persistentDataPath, PlayerDataFileName);

        LoadPlayerData();
        AddPlayerDataToLeaderboard(PlayerPrefs.GetString(Strings.Username), Strings.GetChaptersProgressions(), PlayerPrefs.GetString(Strings.Gender));
        DisplayLeaderboard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (scrollToBottomCoroutine != null)
                StopCoroutine(scrollToBottomCoroutine);
            scrollToTopCoroutine = StartCoroutine(ScrollToTop());
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (scrollToTopCoroutine != null)
                StopCoroutine(scrollToTopCoroutine);
            scrollToBottomCoroutine = StartCoroutine(ScrollToBottom());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DeleteJSON();
        }
    }

    public void DeleteJSON()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private void AssignToIntro(Sprite profileSprite, PlayerData playerData) 
    {
        profilePicture.sprite = profileSprite;
        NameText.text = playerData.playerName;

       Strings.ShowBadges(Strings.ChapterOne, playerData.chapterProgression[0], Strings.ChapterOneBadgePath, badge[0]);
       Strings.ShowBadges(Strings.ChapterTwo, playerData.chapterProgression[1], Strings.ChapterTwoBadgePath, badge[1]);
       Strings.ShowBadges(Strings.ChapterThree, playerData.chapterProgression[2], Strings.ChapterThreeBadgePath, badge[2]);

    }

    private IEnumerator ScrollToTop()
    {
        float scrollSpeed = (1 - scrollRect.verticalNormalizedPosition) / 2f;
        while (scrollRect.verticalNormalizedPosition != 1)
        {
            if (scrollRect.verticalNormalizedPosition >= 1)
            {
                scrollRect.verticalNormalizedPosition = 1;
                yield break;
            }
            Debug.Log("scrolling to top");
            scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ScrollToBottom()
    {
        float scrollSpeed = scrollRect.verticalNormalizedPosition / 2f;
        while (scrollRect.verticalNormalizedPosition != 0)
        {
            if (scrollRect.verticalNormalizedPosition <= 0)
            {
                scrollRect.verticalNormalizedPosition = 0;
                yield break;
            }
            Debug.Log("scrolling to bottom");
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
