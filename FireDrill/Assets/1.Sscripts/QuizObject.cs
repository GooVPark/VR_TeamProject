using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;

[CreateAssetMenu(menuName = "Quiz")]
public class QuizObject : ScriptableObject
{
    public int answer;
    public string[] question;

    [TextArea(30, 50)]
    public string contents;

    [TextArea(30, 20)]
    public string quizInfo;

    public QuizState IsAnswer(int answer)
    {
        if (answer == this.answer) return QuizState.Correct;

        return QuizState.Incorrect;
    }
}

public enum QuizType { Selection, Sequence, OX }
public class QuizJson
{
    public ObjectId _id;

    public QuizType type;
    public int quizIndex;
    public string question;
    public string[] selections;
    public int[] answer;
    public int code;

    public string[] solutions;
}
