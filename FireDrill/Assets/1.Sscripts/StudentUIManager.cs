using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudentUIManager : View
{
    public Button personalVideoButton;
    public Button quizButton;

    public GameObject personalVideoWindow;
    public GameObject quizView;

    public Button closeButton;


    public override void Initialize()
    {
        quizButton.onClick.AddListener(() => ShowCurrentQuizWindow());
        closeButton.onClick.AddListener(() => ViewManager.ShowLast());
    }

    public void ShowPersonalVideoWindow()
    {
        personalVideoWindow.SetActive(true);
    }

    public void ShowCurrentQuizWindow()
    {
        QuizView quizView = this.quizView.GetComponent<QuizView>();
        quizView.SetQuizView(QuizManager.Instance.currentQuizIndex);

        this.quizView.SetActive(true);
    }

}
