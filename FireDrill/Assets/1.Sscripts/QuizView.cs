using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizView : View
{
    public List<QuizSelection> selections = new List<QuizSelection>();
    public TMP_Text content;

    public override void Initialize()
    {
        
    }

    public void SetQuizView(int index)
    {
        QuizObject quizObject = QuizManager.Instance.quizList[index];

        for(int i = 0; i < quizObject.question.Length; i++)
        {
            selections[i].gameObject.SetActive(true);
            selections[i].contents.text = quizObject.question[i];
        }

        content.text = quizObject.contents;
    }
}
