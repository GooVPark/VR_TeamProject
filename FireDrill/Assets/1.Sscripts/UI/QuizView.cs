using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizView : View
{
    public List<QuizSelection> selections = new List<QuizSelection>();
    public TMP_Text content;

    private int currentQuizIndex = -1;

    public override void Initialize()
    {
        for(int i = 0; i < selections.Count; i++)
        {
            int index = i;
            selections[i].GetComponent<Button>().onClick.AddListener(() => Submit(index));
        }
    }

    public void SetQuizView(int index)
    {

        for (int i = 0; i < selections.Count; i++)
        {
            selections[i].gameObject.SetActive(false);
        }

        if (index < 0)
        {
            content.text = "아직 풀 문제가 없습니다...!";
            return;
        }

        QuizObject quizObject = QuizManager.Instance.quizList[index];
        currentQuizIndex = index;

        for (int i = 0; i < quizObject.question.Length; i++)
        {
            selections[i].gameObject.SetActive(true);
            selections[i].contents.text = quizObject.question[i];
        }

        content.text = quizObject.contents;
    }

    public void Submit(int index)
    {
        QuizManager.Instance.SubmitAnswer(NetworkManager.UserData.id, currentQuizIndex, index);
    }
}
