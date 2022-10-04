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
    [SerializeField] private GameObject characterObjects;
    [SerializeField] private GameObject CharacterSelectWindow;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previewButton;
    [Space(5)]


    [Header("Extingusher Select UI")]
    [SerializeField] private GameObject extingusherSelectWindow;
    [SerializeField] private Button selectExtingusherButton;
    [SerializeField] private Button deselectExtingusherButton;
    [Space(5)]


    [Header("Sign-In UI")]
    [SerializeField] private TMP_InputField signInID;
    [SerializeField] private TMP_InputField signInPassword;
    [SerializeField] private TMP_InputField signInPasswordCheck;
    [SerializeField] private TMP_InputField signInName;

    [SerializeField] private GameObject signInWindow;
    [SerializeField] private Button idCheckButton;
    [SerializeField] private GameObject idCheckPopUp;
    [SerializeField] private TMP_Text idCheckText;
    [SerializeField] private bool idCheck = false;

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
        CharacterSelectManager.onCharacterSelect += SelectCharacter;
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
        bool isContain = DataManager.Instance.IsExistAccount(emailInputField.text, passwordInputField.text, out userData);

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
        CurrentWindow = loginWindow;
    }

    #endregion

    #region Character Select

    public void ShowCharacterSelectWindow()
    {
        CurrentWindow = CharacterSelectWindow;
        characterObjects.SetActive(true);
    }

    public void ShowExstinguisherSelect()
    {
        characterObjects.SetActive(false);
        CurrentWindow = extingusherSelectWindow;
    }

    public void SelectCharacter(int index)
    {
        NetworkManager.User.characterNumber = index;
        DataManager.Instance.UpdateUserData("email", NetworkManager.User.email, "characterNumber", index);
        characterObjects.SetActive(false);
        CurrentWindow = extingusherSelectWindow;
    }

    public void SelectExtingusher(bool isSelected)
    {
        Debug.Log("LoginManager : SelectExtingusher");
        NetworkManager.User.hasExtingisher = isSelected;
        PhotonNetwork.JoinLobby();
    }

    #endregion

    #region Sgin In

    public void ShowSignInWindow()
    {
        CurrentWindow = signInWindow;
    }

    public void SignInAccount()
    {
        Debug.Log("SignInAccount");

        if(signInPassword.text == string.Empty || signInPasswordCheck.text == string.Empty || signInID.text == string.Empty || signInName.text == string.Empty)
        {
            Debug.Log("Empty Slot Exist");
            return;
        }
        else if(!idCheck)
        {
            Debug.Log("Need ID Check");
            return;
        }
        else if(!signInPassword.text.Equals(signInPasswordCheck.text))
        {
            Debug.Log("Password Not Equal");
            return;
        }

        //아이디가 영어인지 검사
        Debug.Log("Set UserData");
        User member = new User
        {
            email = signInID.text,
            password = signInPassword.text,
            name = signInName.text,
            userType = UserType.Student,
            id = DataManager.Instance.GetUserCount()
        };

        Debug.Log("Insert Request");
        DataManager.Instance.InsertMember(member);

        CurrentWindow = loginWindow;
    }

    public void CheckID()
    {
        string id = signInID.text;

        if(DataManager.Instance.IsExistID(id) || id == string.Empty)
        {
            idCheckText.text = "사용 불가";
            idCheck = false;
        }
        else
        {
            idCheckText.text = "사용 가능";
            idCheck = true;
        }

        idCheckPopUp.SetActive(true);
    }

    public void IDCheckConfirm()
    {
        idCheckPopUp.SetActive(false);
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("LoginManager : OnConnectedToMaster");
        ShowCharacterSelectWindow();
        DataManager.Instance.SetOnline(NetworkManager.User.email);
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
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);

        PhotonNetwork.LoadLevel("Loundge");
    }

    #endregion
}
