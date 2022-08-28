using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class LogInMnager : MonoBehaviour
{
    private UserData userData;

    private void Awake()
    {
    

    }


}

[SerializeField]
public class LogInData
{
    private string email;
    public string Email { get => email; }
    private string name;
    public string Name { get => name; }
    private Authority authority;
    public Authority Authority { get => authority; }
    private List<Answer> answers = new List<Answer>();

    public LogInData(UserData userData)
    {
        this.email = userData.email;
        this.name = userData.name;
        this.authority = userData.authority;

        if(userData.isNew)
        {
            this.answers = new List<Answer>(QuizManager.Instance.quizList.Count);
            userData.answers = this.answers;
        }
        else
        {
            this.answers = userData.answers;
        }
    }


}
