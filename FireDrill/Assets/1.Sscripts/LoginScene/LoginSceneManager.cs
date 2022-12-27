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
    public GameObject logoWindow;
    public Button shoLoginWindowButton;
    [Space(5)]

    [Header("Log In UI")]
    public GameObject loginWindow;
    public GameObject virtualKeyboard;

    public Button loginButton;

    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;

    public GameObject loginErrorWindow;
    public Button confirmLoginErrorButton;

    public GameObject loginErrorWindow2;
    public Button confirmLoginErrorButton2;

    [Space(5)]

    [Header("Character Select UI")]
    public GameObject characterObjects;
    public GameObject CharacterSelectWindow;
    public Button nextButton;
    public Button previewButton;
    [Space(5)]


    [Header("Extingusher Select UI")]
    public GameObject extingusherSelectWindow;
    public Button selectExtingusherButton;
    public Button deselectExtingusherButton;
    [Space(5)]


    [Header("Sign-In UI")]
    public TMP_InputField signInID;
    public TMP_InputField signInPassword;
    public TMP_InputField signInPasswordCheck;
    public TMP_InputField signInName;

    public GameObject signInWindow;
    public Button idCheckButton;
    public GameObject idCheckPopUp;
    public Button idCheckErrorButton;
    public GameObject idCheckErrorPopUp;
    public TMP_Text idCheckText;
    public bool idCheck = false;

    public Toggle lectureToggle;
    [Space(5)]

    [Header("Idle Mode UI")]
    [SerializeField] private GameObject setIdleModeWindow;

    private static User userData;
    public static User UserData { get => userData; }

    private bool isLecture = false;
    private bool isStudent = true;

    [SerializeField ]private string gameVersion = "1";

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
        bool isContain = DataManager.Instance.IsExistAccount(emailInputField.text, passwordInputField.text, out userData);
        if (isContain)
        {
            if(userData.isOnline)
            {
                CurrentWindow = loginErrorWindow2;
                return;
            }
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
        
        LoadingSceneController.LoadScene("Loundge (BG3)");
    }

    public void SelectExtingusher(bool isSelected)
    {
        //Debug.Log("LoginManager : SelectExtingusher");
        NetworkManager.User.hasExtingisher = isSelected;
        PhotonNetwork.JoinLobby();
    }

    #endregion

    #region Sgin In

    public void ShowSignInWindow()
    {
        CurrentWindow = signInWindow;

        isLecture = false;
        isStudent = true;

        lectureToggle.isOn = false;

        signInID.text = "";
        signInPassword.text = "";
        signInPasswordCheck.text = "";
        signInName.text = "";
    }

    public void SignInAccount()
    {
        //Debug.Log("SignInAccount");

        if(signInPassword.text == string.Empty || signInPasswordCheck.text == string.Empty || signInID.text == string.Empty || signInName.text == string.Empty)
        {
            //Debug.Log("Empty Slot Exist");
            return;
        }
        else if(!idCheck)
        {
            //Debug.Log("Need ID Check");
            return;
        }
        else if(!signInPassword.text.Equals(signInPasswordCheck.text))
        {
            //Debug.Log("Password Not Equal");
            return;
        }

        //아이디가 영어인지 검사
        //Debug.Log("Set UserData");
        User member = new User
        {
            email = signInID.text,
            password = signInPassword.text,
            name = signInName.text,
            userType = isLecture ? UserType.Lecture : UserType.Student
            //_id = DataManager.Instance.GetUserCount() + 100000
        };

        //Debug.Log("Insert Request");
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
            idCheckPopUp.SetActive(false);
            idCheckErrorPopUp.SetActive(true);
        }
        else
        {
            idCheckText.text = "사용 가능";
            idCheck = true;
            idCheckErrorPopUp.SetActive(false);
            idCheckPopUp.SetActive(true);
        }
    }

    public void IDCheckConfirm()
    {
        idCheckPopUp.SetActive(false);
        idCheckErrorPopUp.SetActive(false);
    }

    public void SetLecture()
    {
        isLecture = !isLecture;
       
        lectureToggle.isOn = isLecture;
    }

    #endregion

    #region Set Idle Mode

    public void SetIdleMode()
    {
        IdleMode idleMode = setIdleModeWindow.GetComponent<SitStandSettButton>().mode;
        NetworkManager.User.idleMode = idleMode;
        DataManager.Instance.UpdateUserData("email", NetworkManager.User.email, "idleMode", idleMode);

        CurrentWindow = null;

        PhotonNetwork.JoinLobby();
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        StartCoroutine(FPSLogLoop());

        ShowCharacterSelectWindow();
        DataManager.Instance.SetOnline(NetworkManager.User.email);
    }

    private IEnumerator FPSLogLoop()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(300);
        while(true)
        {
            DataManager.Instance.UpdateLoginFPS(NetworkManager.Instance.fps, PhotonNetwork.GetPing().ToString(), NetworkManager.User.email);

            yield return waitForSeconds;
        }
    }
    #endregion

    public void Quit()
    {
        Application.Quit();
    }

    public void Home()
    {
        CurrentWindow = logoWindow;
    }
}
