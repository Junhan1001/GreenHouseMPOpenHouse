using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Strings
{
    public static string TimeProfileCreated = "TimeProfileCreated";
    public static string Username = "Username";
    public static string ProfileImage = "ProfileImage";
    public static string Gender = "Gender";

    public static string ChapterOne = "ChapterOne";
    public static string ChapterTwo = "ChapterTwo";
    public static string ChapterThree = "ChapterThree";

    public static string ChapterOneProgressions = "ChapterOneProgressions";
    public static string ChapterTwoProgressions = "ChapterTwoProgressions";
    public static string ChapterThreeProgressions = "ChapterThreeProgressions";

    public static string ChapterOneLevelOneCompleted = "ChapterOneLevelOneCompleted";
    public static string ChapterOneLevelTwoCompleted = "ChapterOneLevelTwoCompleted";
    public static string ChapterOneLevelThreeCompleted = "ChapterOneLevelThreeCompleted";
    public static string ChapterTwoLevelOneCompleted = "ChapterTwoLevelOneCompleted";
    public static string ChapterTwoLevelTwoCompleted = "ChapterTwoLevelTwoCompleted";
    public static string ChapterTwoLevelThreeCompleted = "ChapterTwoLevelThreeCompleted";
    public static string ChapterThreeLevelOneCompleted = "ChapterThreeLevelOneCompleted";
    public static string ChapterThreeLevelTwoCompleted = "ChapterThreeLevelTwoCompleted";
    public static string ChapterThreeLevelThreeCompleted = "ChapterThreeLevelThreeCompleted";

    public static string ChapterOneBadge = "ChapterOneBadge";
    public static string ChapterTwoBadge = "ChapterTwoBadge";
    public static string ChapterThreeBadge = "ChapterThreeBadge";

    public static string ChapterOneBadgePath = "Badge_Beginner";
    public static string ChapterTwoBadgePath = "Badge_Advanced";
    public static string ChapterThreeBadgePath = "Badge_Competent";

    private static Dictionary<string, List<string>> ChaptersDictionary = new()
    {
        { ChapterOne,  new List<string>() { ChapterOneProgressions, ChapterOneBadge } },
        { ChapterTwo, new List<string>() { ChapterTwoProgressions, ChapterTwoBadge } },
        { ChapterThree, new List<string>() { ChapterThreeProgressions, ChapterThreeBadge } }
    };

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(ChapterOneProgressions);
        PlayerPrefs.DeleteKey(ChapterTwoProgressions);
        PlayerPrefs.DeleteKey(ChapterThreeProgressions);
        PlayerPrefs.DeleteKey(ChapterOneLevelOneCompleted);
        PlayerPrefs.DeleteKey(ChapterOneLevelTwoCompleted);
        PlayerPrefs.DeleteKey(ChapterOneLevelThreeCompleted);
        PlayerPrefs.DeleteKey(ChapterTwoLevelOneCompleted);
        PlayerPrefs.DeleteKey(ChapterTwoLevelTwoCompleted);
        PlayerPrefs.DeleteKey(ChapterTwoLevelThreeCompleted);
        PlayerPrefs.DeleteKey(ChapterThreeLevelOneCompleted);
        PlayerPrefs.DeleteKey(ChapterThreeLevelTwoCompleted);
        PlayerPrefs.DeleteKey(ChapterThreeLevelThreeCompleted);
        PlayerPrefs.DeleteKey("Reviewed");
        PlayerPrefs.DeleteKey("firstTime");
    }

    public static int[] GetChaptersProgressions()
    {
        return new int[3] { PlayerPrefs.GetInt(ChapterOneProgressions), PlayerPrefs.GetInt(ChapterTwoProgressions), PlayerPrefs.GetInt(ChapterThreeProgressions) };
    }

    public static bool IsFirstTime()
    {
        return (!PlayerPrefs.HasKey("firstTime"));
        //if (!PlayerPrefs.HasKey("firstTime"))
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    public static void ShowBadges(string chapter, string badgePath, Image badge)
    {
        if (PlayerPrefs.HasKey(ChaptersDictionary[chapter][0]))
        {
            int progress = PlayerPrefs.GetInt(ChaptersDictionary[chapter][0]);
            Debug.Log(progress);

            badge.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(badgePath);
            badge.fillAmount = (float)progress / 3;
            Debug.Log(badge.fillAmount);
            if (badge.fillAmount != 1)
            {
                badge.transform.GetChild(1).GetComponent<Image>().color = new Color32(0, 0, 0, 100);
            }
            else
            {
                badge.transform.GetChild(1).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }

            //Texture2D texture2D = new(1, 1);
            //texture2D.LoadImage(Convert.FromBase64String(PlayerPrefs.GetString(ChaptersDictionary[chapter][1])));
            //badge.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
        }
        else
        {
            badge.transform.GetChild(1).GetComponent<Image>().color = new Color32(0, 0, 0, 100);
        }
    }

    public static void ShowBadges(string chapter, int chapterProgressions, string badgePath, Image badge)
    {
        //if (PlayerPrefs.HasKey(ChaptersDictionary[chapter][0]))
        //{
            if (chapterProgressions != 3)
            {
                badge.color = new Color32(0, 0, 0, 100);
            }
            else
            {
                badge.color = new Color32(255, 255, 255, 255);
            }

            //Texture2D texture2D = new(1, 1);
            //texture2D.LoadImage(Convert.FromBase64String(PlayerPrefs.GetString(ChaptersDictionary[chapter][1])));
            //badge.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
        //}
        //else
        //{
        //    badge.color = new Color32(0, 0, 0, 100);
        //}
    }
}
