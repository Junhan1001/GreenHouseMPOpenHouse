using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class OpenHouseLevel2 : MonoBehaviour
{
    public List<GameObject> connectionPoint1;
    public List<GameObject> connectionSubPoint1;
    public List<GameObject> connectionPoint2;
    public List<GameObject> connectionPoint3;
    public List<GameObject> connectionSubPoint3;
    public List<GameObject> connectionPoint4;

    public bool HotPipes;
    public bool CoolPipes;
    public bool connectionsChecked;
    public bool connectionHot;
    public bool connectionCool;

    

    public GameObject correctPanel;
    public GameObject wrongPanel;
    public static bool showPopUp;

    GameObject[] HotAns;
    GameObject[] CoolAns;

    GameObject connection;
    public Fade fade;

    private void Start()
    {
        HotAns = CoolAns = null;
        showPopUp = true;
        connection = GameObject.Find("Connections");


        connectionsChecked = false;
    }
    public void AnswerCheck()
    {
        ConnectionCheck();
        if (showPopUp)
        {
            Debug.Log("PRESSED");
            if (connectionHot && connectionCool && connectionsChecked)
            {
                connection.GetComponent<Connection>().enabled = false;
                correctPanel.SetActive(true);
                if (!PlayerPrefs.HasKey(Strings.ChapterTwoLevelTwoCompleted))
                {
                    if (PlayerPrefs.HasKey(Strings.ChapterTwoProgressions))
                    {
                        int progress = PlayerPrefs.GetInt(Strings.ChapterTwoProgressions);
                        progress++;
                        PlayerPrefs.SetInt(Strings.ChapterTwoProgressions, progress);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(Strings.ChapterTwoProgressions, 1);
                    }
                    PlayerPrefs.SetInt(Strings.ChapterTwoLevelTwoCompleted, 1);
                }
                fade.ShowChapterTwoBadge();
                Debug.Log("Correct");
                showPopUp = false;
            }
            else
            {
                wrongPanel.SetActive(true);
                Debug.Log("wrong");
                showPopUp = true;
                connectionsChecked = false;
                HotAns = CoolAns = null;
            }
        }
    }
    void ConnectionCheck()
    {
        CoolAns = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Pipe CHWR(Clone)" || obj.name == "Connection 1" || obj.name == "Connection 2").ToArray();
        HotAns = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Pipe CWR(Clone)" || obj.name == "Connection 3" || obj.name == "Connection 4").ToArray();

        Debug.Log(CoolAns.Count());




        if (HotAns.Count() == 3)
        {
            connectionHot = true;
            //Debug.Log("CHWR Correct");
        }
        else
        {
            connectionHot = false;
        }

        if (CoolAns.Count() == 3)
        {
            connectionCool = true;
            //Debug.Log("CHWS Correct");
        }
        else
        {
            connectionCool = false;
        }

    }


    public bool ListComparison(List<GameObject> playerList)
    {

        foreach (GameObject g in playerList)
        {

            if (g.transform.name == "Connection Point 1" && CoolPipes)
            {
                Debug.Log("Reached 1");
                if (!connectionPoint1.Contains(g))
                {
                    Debug.Log("no position match");
                    return false;
                }
                else
                {
                    Debug.Log("Matches set 1");
                    return true;
                }
            }

            else if (g.transform.name == "Connection SubPoint 1" && CoolPipes)
            {
                Debug.Log("checking s1");
                if (!connectionSubPoint1.Contains(g))
                {
                    Debug.Log("no position match");
                    return false;
                }
                else
                {
                    Debug.Log("Matches sub set 1");
                    return true;
                }
            }

            else if (g.transform.name == "Connection Point 2" && CoolPipes)
            {
                //Debug.Log("checking 2");
                if (!connectionPoint2.Contains(g))
                {
                    Debug.Log("no position match");
                    return false;
                }
                else
                {
                    Debug.Log("Matches set 2");
                    return true;
                }
            }


            //Debug.Log("checking 3");
            else if (g.transform.name == "Connection Point 3" && HotPipes)
            {
                if ((!connectionPoint3.Contains(g)))
                {
                    Debug.Log("no position match");
                    return false;
                }
                else
                {
                    Debug.Log("Matches set 3");
                    return true;
                }
            }

            //Debug.Log("checking s3");
            else if (g.transform.name == "Connection SubPoint 3" && HotPipes)
            {
                if ((!connectionSubPoint3.Contains(g)))
                {
                    Debug.Log("no position match");
                    return false;
                }
                else
                {
                    Debug.Log("Matches sub set 3");
                    return true;
                }
            }


            else if (g.transform.name == "Connection Point 4" && HotPipes)
            {
                //Debug.Log("checking 4"); 
                if (!connectionPoint4.Contains(g))
                {
                    Debug.Log("no position match");
                    return false;
                }
                else
                {
                    Debug.Log("position mactches");
                    Debug.Log("Matches set 4");
                    return true;
                }
            }
        }

        return false;
    }
}
