using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizInfoView : View
{
    public int currentQuizIndex;

    public Button sendQuiz;
    public TMP_Text quizInfo;

    public override void Initialize()
    {
        sendQuiz.onClick.AddListener(() => SendQuiz());
    }

    public void SendQuiz()
    {
        QuizManager.Instance.SendSelectedQuiz(currentQuizIndex);
    }
}
