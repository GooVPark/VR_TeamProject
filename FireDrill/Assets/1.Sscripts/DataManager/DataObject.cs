using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Authority { Lecture, Student }

[CreateAssetMenu(menuName = "UserData")]
public class DataObject : ScriptableObject
{
    public List<UserData> userDB= new List<UserData>();

    public bool IsContain(string email, string password, out UserData userData)
    {
        for(int i = 0; i < userDB.Count; i++)
        {
            if(userDB[i].email == email)
            {
                if (userDB[i].password == password)
                {
                    userData = userDB[i];
                    userData.id = i;
                    return true;
                }
                else
                {
                    userData = null;
                    return false;
                }
            }
        }

        userData = null;
        return false;
    }

    [ContextMenu("Initialize User Quiz Data")]
    public void InitializeValue()
    {
        foreach(UserData userData in userDB)
        {
            if(userData.answers.Count > 0)
            {
                userData.answers.Clear();
            }

            userData.InitializeQuizAnswers();
        }
    }
}

[System.Serializable]
public class UserData
{
    public int id;
    public string email;
    public string password;
    public string name;
    public Authority authority;

    public bool isNew = true;

    public List<Answer> answers = new List<Answer>();

    public void InitializeQuizAnswers()
    {
        for(int i = 0; i < QuizManager.Instance.quizList.Count; i++)
        {
            answers.Add(new Answer(QuizState.NotYet, -1));
        }
    }

    public void UpdateAnswer(int quizID, int answer, QuizState state)
    {
        answers[quizID].Update(state, answer);
    }
}

[System.Serializable]
public class RunTimeUserData
{
    public string email;
    public string name;
    public Authority authority;

    public List<Answer> answers = new List<Answer>();
}
