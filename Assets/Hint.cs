using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{
    public TheoryTab Answers;

    public void GoToAnswers()
    {
        StartCoroutine(waitasec());
    }

    IEnumerator waitasec()
    {
        yield return new WaitForSeconds(0.1f);
        Answers.GoToAnswers();
    }
}
