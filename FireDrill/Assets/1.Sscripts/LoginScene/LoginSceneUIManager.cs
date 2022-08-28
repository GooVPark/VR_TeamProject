using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class LoginSceneUIManager : MonoBehaviour
{
    #region Events

    public delegate void SelectExtingusherEvent(bool hasExtingusher);
    public static SelectExtingusherEvent onSelectExtingusher;

    #endregion

    [Header("Logo UI")]
    [SerializeField] private GameObject logoWindow;
    [SerializeField] private Button shoLoginWindowButton;
    [Space(5)]

    [Header("Log In UI")]
    [SerializeField] private GameObject loginWindow;
    [SerializeField] private GameObject virtualKeyboard;

    [SerializeField] private Button loginButton;

    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private GameObject loginErrorWindow;
    [SerializeField] private Button confirmLoginErrorButton;
    [Space(5)]

    [Header("Character Select UI")]
    [SerializeField] private GameObject CharacterSelectWindow;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previewButton;
    [Space(5)]


    [Header("Extingusher Select UI")]
    [SerializeField] private GameObject extingusherSelectWindow;
    [SerializeField] private Button selectExtingusherButton;
    [SerializeField] private Button deselectExtingusherButton;
    [Space(5)]

    private GameObject currentWindow;
    public GameObject CurrentWindow
    {
        get => currentWindow;
        set
        {
            if(currentWindow != null)
            {
                currentWindow.SetActive(false);
            }
            if (value != null)
            {
                currentWindow = value;
                currentWindow.SetActive(true);
            }
        }
    }

    private void Awake()
    {
        NetworkManager.onConnectedToMasterServer += OnConnectedToMasterServer;
        NetworkManager.onJoinedLobby += OnJoinedLobby;
    }

    private void Start()
    {
        CurrentWindow = logoWindow;
    }

    #region Login

    public void ShowLoginWindow()
    {
        CurrentWindow = loginWindow;
    }

    public void Login()
    {
        User userData;
        bool isContain = DataManager.UserTable.GetUser(emailInputField.text, passwordInputField.text, out userData);

        if (isContain)
        {
            NetworkManager.Instance.Connect(userData);
        }
        else
        {
            ShowLoginErrorWindow();
        }
    }


   
    public void ShowLoginErrorWindow()
    {
        CurrentWindow = loginErrorWindow;
    }

    public void ConfirmLoginError()
    {
        ShowLoginWindow();
    }

    #endregion

    public void ShowCharacterSelectWindow()
    {

    }

    public void SelectCharacter(int index)
    {
        
    }

    public void ShowExtingusherSelectWindow()
    {
        CurrentWindow = extingusherSelectWindow;
    }

    public void SelectExtingusher(bool isSelected)
    {
        onSelectExtingusher?.Invoke(isSelected);
        NetworkManager.UserData.hasExtingisher = isSelected;
        NetworkManager.Instance.JoinLobby();
    }

    public void OnConnectedToMasterServer()
    {
        ShowExtingusherSelectWindow();
    }

    public void OnJoinedLobby()
    {
        
    }
}
