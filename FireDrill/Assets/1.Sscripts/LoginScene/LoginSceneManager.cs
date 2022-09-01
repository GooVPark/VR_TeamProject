using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class LoginSceneManager : MonoBehaviourPunCallbacks
{
    #region Events

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

    private static User userData;
    public static User UserData { get => userData; }

    private string gameVersion = "1.0";

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

    }

    private void Start()
    {
        CurrentWindow = logoWindow;

    }

    #region Logo

    public void ShowLoginWindow()
    {
        CurrentWindow = loginWindow;
    }

    #endregion

    #region Login

    public void Login()
    {
        User userData;
        bool isContain = DataManager.UserTable.GetUser(emailInputField.text, passwordInputField.text, out userData);

        if (isContain)
        {
            //NetworkManager.Instance.Connect(userData);
            Connect(userData);
        }
        else
        {
            ShowLoginErrorWindow();
        }
    }

    public void Connect(User _userData)
    {
        NetworkManager.Instance.SetUser(_userData);

        PhotonNetwork.LocalPlayer.NickName = _userData.name;

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }


    public void ShowLoginErrorWindow()
    {
        CurrentWindow = loginErrorWindow;
    }

    public void ConfirmLoginError()
    {
        currentWindow = logoWindow;
    }

    #endregion

    #region Character Select

    public void ShowCharacterSelectWindow()
    {

    }

    public void SelectCharacter(int index)
    {
        
    }

    public void SelectExtingusher(bool isSelected)
    {
        Debug.Log("LoginManager : SelectExtingusher");
        PhotonNetwork.JoinLobby();
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("LoginManager : OnConnectedToMaster");
        CurrentWindow = extingusherSelectWindow;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("LoginManager : OnJoinedLobby");
        string roomName = $"Loundge";

        //NetworkManager.Instance.roomType = NetworkManager.RoomType.Loundge;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 0;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    #endregion
}
