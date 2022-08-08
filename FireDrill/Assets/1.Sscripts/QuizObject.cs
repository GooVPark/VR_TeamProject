using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quiz")]
public class QuizObject : ScriptableObject
{
    public int answer;
    public string[] question;

    [TextArea(30, 50)]
    public string contents;

    public bool IsAnswer(int answer)
    {
        if (answer == this.answer) return true;

        return false;
    }
}
