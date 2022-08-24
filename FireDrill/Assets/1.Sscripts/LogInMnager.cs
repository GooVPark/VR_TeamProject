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
        LoginSceneUIManager.TryLogIn += TryLogIn;

        NetworkManager.onConnectedToMasterServer += OnConnectedToMasterServer;
        NetworkManager.onJoinedLobby += OnJoinedLobby;
    }

    public bool TryLogIn(string email, string password)
    {
        if (email == string.Empty || password == string.Empty)
        {
            return false;
        }

        if(DataManager.Instance.FindUserData(email, password, out userData))
        {
            NetworkManager.Instance.Connect(userData);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnConnectedToMasterServer()
    {
        Debug.Log("Connected To Master Server.");
    }

    public void OnJoinedLobby()
    {
        Debug.Log("Joined to Lobby.");
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
