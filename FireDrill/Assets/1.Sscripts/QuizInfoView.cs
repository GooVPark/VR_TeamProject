using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizInfoView : View
{
    public int currentQuizIndex;

    public Button sendQuiz;

    public override void Initialize()
    {
    }

    public void SendQuiz()
    {
        QuizManager.Instance.SendSelectedQuiz(currentQuizIndex);
    }
}
