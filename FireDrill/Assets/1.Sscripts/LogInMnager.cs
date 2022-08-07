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

    private bool isLoginButtonInteractable = false;

    public void LogIn()
    {
        if (emailField.text == string.Empty || passwordField.text == string.Empty)
        {
            return;
        }

        loginButton.interactable = false;

        //sum actions...
        LogInData logInData = new LogInData(emailField.text, passwordField.text); //DB �����ϸ� ����� ���� ���� ��Ƽ� ��Ʈ��ũ �Ŵ����� �Ѱ��ٰ���
        NetworkManager.Instance.Connect(logInData);
        
        loginUI.SetActive(false);
    }
}

[SerializeField]
public class LogInData
{
    private string email;
    private string password;

    public LogInData(string email, string password)
    {
        this.email = email;
        this.password = password;
    }

    public string GetUserName()
    {
        return email.Split('@')[0];
    }

    public string GetUserLevel()
    {
        return email.Split('@', '.')[1];
    }
}
