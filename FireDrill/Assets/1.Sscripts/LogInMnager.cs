using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LogInMnager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;

    public Button loginButton;

    public GameObject loginUI;
    public GameObject networkManager;

    private UserData userData;

    private bool isLoginButtonInteractable = false;

    public void LogIn()
    {
        if (emailField.text == string.Empty || passwordField.text == string.Empty)
        {
            return;
        }

        if(DataManager.Instance.FindUserData(emailField.text, passwordField.text, out userData))
        {

        }
        else
        {
            return;
        }
        
        loginButton.interactable = false;

        //sum actions...
        NetworkManager.Instance.Connect(userData);
        
        loginUI.SetActive(false);
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
