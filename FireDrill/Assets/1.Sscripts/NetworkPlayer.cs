using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

using TMPro;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

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

    [SerializeField] private UserType userLevel;
    public UserType UserLevel { get => userLevel; set => ActionRPC(nameof(SetUserLevelRPC), value); }
    [PunRPC]
    private void SetUserLevelRPC(UserType value)
    {
        userLevel = value;
        switch (userLevel)
        {
            case UserType.Lecture:
                userTypeLecture.SetActive(true);
                break;
            case UserType.Student:
                userTypeStudent.SetActive(true);
                break;
        }
    }

    [SerializeField] private bool hasExtinguisher;
    public bool HasExtinguisher { get => hasExtinguisher; set => ActionRPC(nameof(SetHasExtinguisher), value); }
    [PunRPC]
    private void SetHasExtinguisher(bool value)
    {
        hasExtinguisher = value;
        if(hasExtinguisher)
        {
            extingusiherEnabled.SetActive(true);
        }
        else
        {
            extinguisherDisabled.SetActive(false);
            //var collision = hoseWater.collision;
            //collision.enabled = false;
        }
    }

    [SerializeField] private bool onVoiceChat;
    public bool OnVoiceChat { get => onVoiceChat; set => ActionRPC(nameof(SetOnVoiceChatRPC), value); }
    [PunRPC]
    private void SetOnVoiceChatRPC(bool value)
    {
        onVoiceChat = value;    
    }

    [SerializeField] private bool onMegaphone;
    public bool OnMegaPhone { get => onMegaphone; set => ActionRPC(nameof(SetOnMegaPhoneRPC), value); }
    [PunRPC]
    private void SetOnMegaPhoneRPC(bool value)
    {
        if(value)
        {
            megaphoneEnabled.SetActive(true);
        }
        else
        {
            megaphoneEnabled.SetActive(false);
        }
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
        HasExtinguisher = HasExtinguisher;
        OnMegaPhone = OnMegaPhone;
        OnVoiceChat = OnVoiceChat;
    }
    #endregion

    [SerializeField] private float speechBubbleViualizeDistance;

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource megaphone;

    [SerializeField] private Speaker micSpeaker;
    //[SerializeField] private Speaker megaphoneSpeaker;

    [Header("Player UI")]
    public GameObject userInfoUI;
    public TMP_Text userNameUI;
    public TMP_Text userLevelUI;
    public TMP_Text distanceUI;
    public Image image;
    public GameObject speachBubble;

    [Header("User Type Icons")]
    public GameObject userTypeLecture;
    public GameObject userTypeStudent;
    [Space(5)]

    [Header("Extinguisher Enabled")]
    public GameObject extingusiherEnabled;
    public GameObject extinguisherDisabled;
    [Space(5)]

    [Header("Voice Chat Icons")]
    public GameObject voiceChatDisabled;
    public GameObject voiceChatEnabled;
    public GameObject voiceChatHoverd;
    public GameObject voiceChatOn;
    [Space(5)]

    [Header("Megaphotn Icon")]
    public GameObject megaphoneEnabled;
    [Space(5)]

    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;

    [SerializeField] private Transform leftWrist;
    [SerializeField] private Transform rightWrist;
    private Quaternion leftWristOrigin;
    private Quaternion rightWristOrigin;


    [SerializeField] private HandAnimationController leftHandAnimation;
    [SerializeField] private HandAnimationController rightHandAnimation;

    [SerializeField] private Animator leftHandAnimator;
    [SerializeField] private Animator rightHandAnimator;

    private Camera headset;

    public Button requestVoiceChatButton;
    private PhotonVoiceView voiceView;
    public GameObject headModel;

    [Header("Extinguisher")]
    public PinTrigger pinTrigger;
    public Extinguisher extinguisher;
    public ParticleSystem hoseWater;
    public GameObject hose;
    private GameObject extinguisherObject;

    private Dictionary<string, float[]> micSettings = new Dictionary<string, float[]>();

    [SerializeField] private AnimationCurve voiceAudioCurve;
    [SerializeField] private AnimationCurve megaphoneAudioCurve;

    // Start is called before the first frame update
    void Start()
    {
        voiceAudioCurve = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);

        Transform parent = GameObject.Find("Players").transform;
        transform.SetParent(parent);
        photonView = GetComponent<PhotonView>();

        GameManager gameManager = FindObjectOfType<GameManager>();
        //gameManager.showSpeechBubble += OnSendChatMessage;

        headset = Camera.main;
        leftController = GameObject.Find("Left Direct Interactor").GetComponent<ActionBasedController>();
        rightController = GameObject.Find("Right Direct Interactor").GetComponent<ActionBasedController>();

        leftHandAnimation = GameObject.Find("Left Direct Interactor").GetComponent<HandAnimationController>();
        rightHandAnimation = GameObject.Find("Right Direct Interactor").GetComponent<HandAnimationController>();

        leftWristOrigin = leftWrist.localRotation;
        rightWristOrigin = rightWrist.localRotation;

        onVoiceChat = false;

        voiceView = GetComponent<PhotonVoiceView>();
        voiceView.SpeakerInUse = micSpeaker;

        
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //head.gameObject.SetActive(false);
            //leftHand.gameObject.SetActive(false);
            //rightHand.gameObject.SetActive(false);

            MapPosition();
            SyncAnimation();
        }
    }

    public void OnExtinguisher(Vector3 position)
    {
        Debug.Log("OnExtinguisher");
        if (photonView.IsMine)
        {
            Debug.Log("OnExtinguisher Is Mine");

            extinguisherObject = PhotonNetwork.Instantiate("Extinguisher", position, Quaternion.identity);

            photonView.RPC(nameof(OnExtinguisherRPC), RpcTarget.All, extinguisherObject.name);
        }
    }
    [PunRPC]
    public void OnExtinguisherRPC(string name)
    {
        extinguisher = GameObject.Find(name).GetComponent<Extinguisher>();
        pinTrigger = extinguisher.pinTrigger;
        hose = extinguisher.hose;
        hoseWater = extinguisher.hoseWater;

        hose.SetActive(true);
        extinguisher.gameObject.SetActive(true);
    }

    public void OffExtinguisher()
    {
        hose.SetActive(false);
        extinguisher.gameObject.SetActive(false);
    }

    public void Spread(bool value)
    {
        if(photonView.IsMine)
        {
            photonView.RPC(nameof(SpreadRPC), RpcTarget.All, value);
        }
    }

    [PunRPC]
    public void SpreadRPC(bool value)
    {
        if(value)
        {
            hoseWater.Play();
        }
        else
        {
            hoseWater.Stop();
        }
        Debug.Log("Spread Value: " + value);
    }

    public void Initialize()
    {
        if (photonView.IsMine)
        {
            UserID = NetworkManager.User.id;
            UserName = NetworkManager.User.name;
            UserLevel = NetworkManager.User.userType;
            HasExtinguisher = NetworkManager.User.hasExtingisher;

            userInfoUI.SetActive(false);

            headModel.layer = 31;
            leftHand.gameObject.layer = 31;
            rightHand.gameObject.layer = 31;

            TextChatManager.sendChatMessage += OnSendChatMessage;
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

        Vector3 leftPosition = leftController.transform.GetChild(0).GetChild(0).position;
        Quaternion leftRotation = leftController.transform.GetChild(0).GetChild(0).rotation;

        leftHand.position = leftPosition;
        leftHand.rotation = leftRotation;

        Vector3 rightPosition = rightController.transform.GetChild(0).GetChild(0).position;
        Quaternion rightRotation = rightController.transform.GetChild(0).GetChild(0).rotation;

        rightHand.position = rightPosition;
        rightHand.rotation = rightRotation;
    }

    private void SyncAnimation()
    {
        if (rightHandAnimation.animator.GetInteger("Pose") == 0)
        {
            rightHandAnimator.SetFloat("Flex", rightHandAnimation.flex);
            rightHandAnimator.SetLayerWeight(2, rightHandAnimation.animator.GetLayerWeight(2));
            rightHandAnimator.SetLayerWeight(1, rightHandAnimation.animator.GetLayerWeight(1));

  //          Quaternion rightWristRotation = rightHandAnimation.transform.rotation;
//            rightWrist.localRotation = rightWristRotation;
        }

        if (leftHandAnimation.animator.GetInteger("Pose") == 0)
        {
            leftHandAnimator.SetFloat("Flex", leftHandAnimation.flex);
            leftHandAnimator.SetLayerWeight(2, leftHandAnimation.animator.GetLayerWeight(2));
            leftHandAnimator.SetLayerWeight(1, leftHandAnimation.animator.GetLayerWeight(1));
        }
    }

    public void InteractionTest()
    {
     //   Debug.Log("On Cursor Hoverd");
    }

    #region Haptic

    public void ExcuteHaptic(float amplitude, float duration)
    {
        rightController.SendHapticImpulse(amplitude, duration);
    }

    #endregion

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
        photonView.RPC(nameof(OnSendChatMessageRPC), RpcTarget.All, message);
    }

    [PunRPC]
    public void OnSendChatMessageRPC(string message)
    {

        if (popChat != null)
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
        if(photonView.IsMine)
        {
            photonView.RPC(nameof(VoiceOnRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    public void VoiceOnRPC()
    {
        audioSource.enabled = true;
    }

    public void VoiceOff()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(VoiceOffRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    public void VoiceOffRPC()
    {
        audioSource.enabled = false;
    }

    public void MegaphoneOn()
    {

        photonView.RPC(nameof(MegaphoneOnRPC), RpcTarget.All);

    }

    [PunRPC]
    public void MegaphoneOnRPC()
    {
        audioSource.maxDistance = 500f;
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, megaphoneAudioCurve);

        megaphoneEnabled.SetActive(true);

    }

    public void MegaphoneOff()
    {
        photonView.RPC(nameof(MegaphoneOffRPC), RpcTarget.All);
    }

    [PunRPC]
    public void MegaphoneOffRPC()
    {
        audioSource.maxDistance = 5f;
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, voiceAudioCurve);

        megaphoneEnabled.SetActive(false);
    }

    #endregion

    #region Photon Interfaces

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    #endregion
}
