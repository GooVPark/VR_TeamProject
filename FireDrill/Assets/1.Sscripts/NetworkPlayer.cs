using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

using TMPro;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    #region 동기화 프로퍼티
    [SerializeField] private int userID;
    public int UserID { get => userID; set => ActionRPC(nameof(SetUserIDRPC), value); }
    [PunRPC]
    private void SetUserIDRPC(int value)
    {
        userID = value;
    }

    [SerializeField] private string userName;
    public string UserName { get => userName; set => ActionRPC(nameof(SetUserNameRPC), value); }
    [PunRPC]
    private void SetUserNameRPC(string value)
    {
        userName = value;
        userNameUI.text = userName;
    }

    [SerializeField] private string userLevel;
    public string UserLevel { get => userLevel; set => ActionRPC(nameof(SetUserLevelRPC), value); }
    [PunRPC]
    private void SetUserLevelRPC(string value)
    {
        userLevel = value;
        userLevelUI.text = userLevel;
    }

    [SerializeField] private bool onVoiceChat;
    public bool OnVoiceChat { get => onVoiceChat; set => ActionRPC(nameof(SetOnVoiceChatRPC), value); }
    [PunRPC]
    private void SetOnVoiceChatRPC(bool value)
    {
        onVoiceChat = value;    
    }

    private void ActionRPC(string functionName, object value)
    {
        photonView.RPC(functionName, RpcTarget.All, value);
    }

    public void InvokeProperties()
    {
        UserID = UserID;
        UserName = UserName;
        UserLevel = UserLevel;
        OnVoiceChat = OnVoiceChat;
    }
    #endregion

    [SerializeField] private float speechBubbleViualizeDistance;

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;
    [SerializeField] private AudioSource audioSource;

    public GameObject userInfoUI;
    public TMP_Text userNameUI;
    public TMP_Text userLevelUI;
    public TMP_Text distanceUI;

    public GameObject speachBubble;

    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    private Camera headset;

    public Button requestVoiceChatButton;


    // Start is called before the first frame update
    void Start()
    {
        Transform parent = GameObject.Find("Players").transform;
        transform.SetParent(parent);
        photonView = GetComponent<PhotonView>();

        HUDController hudController = FindObjectOfType<HUDController>();
        hudController.showSpeechBubble += OnSendChatMessage;

        headset = Camera.main;
        leftController = GameObject.Find("LeftHand Controller").GetComponent<ActionBasedController>();
        rightController = GameObject.Find("RightHand Controller").GetComponent<ActionBasedController>();

        onVoiceChat = false;

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            head.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);

            MapPosition();
        }
    }

    public void Initialize()
    {
        if (photonView.IsMine)
        {
            UserID = NetworkManager.User.id;
            UserName = NetworkManager.User.name;
            UserLevel = NetworkManager.User.userType.ToString();

            userInfoUI.SetActive(false);
        }

        requestVoiceChatButton.onClick.AddListener(() => LoundgeSceneManager.Instance.RequsetVoiceChat(NetworkManager.User.id, UserID));
    }

    [PunRPC]
    public void InitializeRPC()
    {
        userNameUI.text = NetworkManager.User.name;
    }

    public void UpdateDistanceUI(float distance)
    {
        distanceUI.text = distance.ToString("0:0.0") + "m";
    }

    private void MapPosition()
    {
        Vector3 headsetPosition = headset.transform.position;
        Quaternion headsetRotation = headset.transform.rotation;

        head.position = headsetPosition;
        head.rotation = headsetRotation;

        Vector3 leftPosition = leftController.transform.position;
        Quaternion leftRotation = leftController.transform.rotation;

        leftHand.position = leftPosition;
        leftHand.rotation = leftRotation;

        Vector3 rightPosition = rightController.transform.position;
        Quaternion rightRotation = rightController.transform.rotation;

        rightHand.position = rightPosition;
        rightHand.rotation = rightRotation;
    }

    public void InteractionTest()
    {
        Debug.Log("On Cursor Hoverd");
    }

    #region Text Chat

    Coroutine popChat;
    IEnumerator PopChat(string message)
    {
        speachBubble.GetComponent<SpeachBubble>().ShowBubble(message);

        yield return new WaitForSeconds(10f);

        speachBubble.GetComponent<SpeachBubble>().HideBubble();
    }

    public void OnSendChatMessage(string message)
    {
        if(photonView.IsMine) photonView.RPC(nameof(OnSendChatMessageRPC), RpcTarget.All, message);
    }

    [PunRPC] public void OnSendChatMessageRPC(string message)
    {
        if(popChat != null)
        {
            StopCoroutine(popChat);
        }
        popChat = StartCoroutine(PopChat(message));
    }

    public void UpdateSpeechBuble(float distance)
    {
        if (distance > speechBubbleViualizeDistance)
        {
            speachBubble.SetActive(false);
        }
        else
        {
            speachBubble.SetActive(true);
        }
    }

    public void ShowSpeachBubble()
    {
        speachBubble.SetActive(true);
    }

    public void HideSpeachBubble()
    {
        speachBubble.SetActive(false);
    }



    #endregion

    #region Voice Chat

    public void VoiceOn()
    {
        audioSource.enabled = true;
    }

    public void VoiceOff()
    {
        audioSource.enabled = false;
    }

    #endregion

    #region Photon Interfaces

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    #endregion
}
