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

public class NetworkPlayer : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    #region 동기화 프로퍼티
    [SerializeField] private string userID;
    public string UserID { get => userID; set => ActionRPC(nameof(SetUserIDRPC), value); }
    [PunRPC]
    private void SetUserIDRPC(string value)
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
                studentIcon.gameObject.SetActive(false);
                lectureIcon.gameObject.SetActive(true);
                break;
            case UserType.Student:
                lectureIcon.gameObject.SetActive(false);
                studentIcon.gameObject.SetActive(true);
                //uiGroup.sizeDelta = new Vector2(220, 0) + uiGroup.sizeDelta;
                break;
        }
        //uiGroup.sizeDelta = new Vector2(100, 0) + uiGroup.sizeDelta;
    }

    [SerializeField] private int currentCharacter;
    public int CurrentCharacter { get => currentCharacter; set => ActionRPC(nameof(SetCurrentCharacterRPC), value); }
    [PunRPC]
    private void SetCurrentCharacterRPC(int value)
    {
        currentCharacter = value;
        SetCurrentCharacter(value);
    }

    [SerializeField] private bool hasExtinguisher;
    public bool HasExtinguisher { get => hasExtinguisher; set => ActionRPC(nameof(SetHasExtinguisher), value); }
    [PunRPC]
    private void SetHasExtinguisher(bool value)
    {
        if (NetworkManager.RoomNumber == 999)
        {
            hasExtinguisher = false;
        }
        else
        {
            hasExtinguisher = value;
            if (hasExtinguisher)
            {
                extinguisherIcon.gameObject.SetActive(true);
                //uiGroup.sizeDelta = new Vector2(100, 0) + uiGroup.sizeDelta;
            }
            else
            {
                extinguisherIcon.gameObject.SetActive(false);
                //var collision = hoseWater.collision;
                //collision.enabled = false;
            }
        }
    }

    [SerializeField] private int quizScore;
    public int QuizScore { get => quizScore; set => ActionRPC(nameof(SetQuizScoreRPC), value); }
    [PunRPC]
    private void SetQuizScoreRPC(int value)
    {
        quizScore = value;
        scoreUI.gameObject.SetActive(false);
        if (UserLevel == UserType.Student)
        {
            if (NetworkManager.Instance.roomType == RoomType.Room)
            {
                if (value < 0)
                {
                    scoreUI.text = $" - ";
                }
                else
                {
                    scoreUI.text = $"{value}/100";
                }
                scoreUI.gameObject.SetActive(true);
            }
            else
            {
                scoreUI.text = "-";
                scoreUI.gameObject.SetActive(false);
            }
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
        if (value)
        {
            megaphoneEnabled.SetActive(true);
        }
        else
        {
            megaphoneEnabled.SetActive(false);
        }
    }

    [SerializeField] private bool onPersonalChat;
    public bool OnPersonalChat { get => onPersonalChat; set => ActionRPC(nameof(SetOnPersonalChatRPC), value); }
    [PunRPC]
    private void SetOnPersonalChatRPC(bool value)
    {
        onPersonalChat = value;
        if (value)
        {
            personalVoice.SetActive(true);
        }
        else
        {
            personalVoice.SetActive(false);
        }
    }

    private void ActionRPC(string functionName, object value)
    {
        if (value != null)
        {
            photonView.RPC(functionName, RpcTarget.All, value);
        }
    }

    public void InvokeProperties()
    {
        UserID = UserID;
        UserName = UserName;
        UserLevel = UserLevel;
        HasExtinguisher = HasExtinguisher;
        OnMegaPhone = OnMegaPhone;
        OnVoiceChat = OnVoiceChat;
        CurrentCharacter = CurrentCharacter;
        QuizScore = QuizScore;
    }
    #endregion

    [SerializeField] private float speechBubbleViualizeDistance;

    public delegate void OnPlayerSelectEvent(NetworkPlayer player);
    public OnPlayerSelectEvent onPlayerSelectEvent;

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource megaphone;

    [SerializeField] private Speaker micSpeaker;
    //[SerializeField] private Speaker megaphoneSpeaker;

    [Header("Character Model")]
    public Mesh[] meshs;
    public Material[] materials;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererMale;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererFemale;
    [Space(5)]

    [Header("Player UI")]
    public GameObject userInfoUI;
    public RectTransform uiGroup;
    public TMP_Text userNameUI;
    public GameObject lectureIcon;
    public GameObject studentIcon;
    public GameObject extinguisherIcon;
    public Image megaphoneIcon;
    public TMP_Text scoreUI;

    public GameObject speachBubble;
    [Space(5)]

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

    public GameObject personalVoice;
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

    [SerializeField] private GameObject femaleHelmet;
    [SerializeField] private GameObject maleHelmet;

    [SerializeField] private GameObject femaleRig;
    [SerializeField] private GameObject maleRig;

    [SerializeField] private GameObject maleMesh;
    [SerializeField] private GameObject femaleMesh;

    [SerializeField] private HandAnimationController leftHandAnimation;
    [SerializeField] private HandAnimationController rightHandAnimation;

    [SerializeField] private Animator leftHandAnimator;
    [SerializeField] private Animator rightHandAnimator;

    [SerializeField] private Animator genderLeftHandAnimator;
    [SerializeField] private Animator genderRightHandAnimator;
    [SerializeField] private Animator femaleLeftHandAnimator;
    [SerializeField] private Animator femaleRightHandAnimator;
    [SerializeField] private Animator maleLeftHandAnimator;
    [SerializeField] private Animator maleRightHandAnimator;

    private Camera headset;

    public Button requestVoiceChatButton;
    private PhotonVoiceView voiceView;
    public GameObject headModel;

    [Header("Extinguisher")]
    public PinTrigger pinTrigger;
    public Extinguisher extinguisher;
    public ParticleSystem hoseWater;
    public GameObject hose;
    public Nozzle nozzle;
    private GameObject extinguisherObject;
    private Transform extinguisherPivot;
    [SerializeField] private Transform extinguisherPivotMale;
    [SerializeField] private Transform extinguisherPivotFemale;

    public bool onExtinguisher = false;
    [Space(5)]

    [Header("Outline")]
    private SkinnedMeshRenderer outlineObject;
    [SerializeField] private SkinnedMeshRenderer outlineMale;
    [SerializeField] private SkinnedMeshRenderer outlineFemale;

    private bool isHovered = false;
    public bool isHoverActivated = false;

    private Dictionary<string, float[]> micSettings = new Dictionary<string, float[]>();

    [SerializeField] private AnimationCurve voiceAudioCurve;
    [SerializeField] private AnimationCurve megaphoneAudioCurve;

    [SerializeField] private XRSimpleInteractable interactable;

    // Start is called before the first frame update
    void Start()
    {
        voiceAudioCurve = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);

        Transform parent = GameObject.Find("Players").transform;
        transform.SetParent(parent);

        GameManager gameManager = FindObjectOfType<GameManager>();
        //gameManager.showSpeechBubble += OnSendChatMessage;

        headset = Camera.main;
        leftController = GameObject.Find("Left Direct Interactor").GetComponent<ActionBasedController>();
        rightController = GameObject.Find("Right Direct Interactor").GetComponent<ActionBasedController>();

        leftHandAnimation = GameObject.Find("Left Direct Interactor").GetComponent<HandAnimationController>();
        rightHandAnimation = GameObject.Find("Right Direct Interactor").GetComponent<HandAnimationController>();

        rightHandAnimation.animator.SetInteger("Pose", 0);
        rightHandAnimation.animator.SetFloat("Flex", 0f);
        leftHandAnimation.animator.SetInteger("Pose", 0);
        leftHandAnimation.animator.SetFloat("Flex", 0f);


        if (photonView.IsMine)
        {
            if(NetworkManager.Instance.roomType == RoomType.Room)
            {
                leftWrist.gameObject.SetActive(true);
                rightWrist.gameObject.SetActive(true);
            }

            interactable.enabled = false;
        }

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

            if (onExtinguisher)
            {
                extinguisherObject.transform.position = extinguisherPivot.position;
                extinguisherObject.transform.rotation = extinguisherPivot.rotation;
            }
        }
    }

    public void OutlineEnabled()
    {
        if (isHoverActivated)
        {
            outlineObject.gameObject.SetActive(true);
            isHovered = true;
        }
    }

    public void OutlineDisabled()
    {
        outlineObject.gameObject.SetActive(false);
        isHovered = false;
    }

    public void OnSelectMRPlayer()
    {
        if (isHovered)
        {
            onPlayerSelectEvent?.Invoke(this);
        }
    }

    public void OnSelectedMRPlayer()
    {
        photonView.RPC(nameof(OnSelectedMRPlayerRPC), RpcTarget.All);
    }

    [PunRPC]
    private void OnSelectedMRPlayerRPC()
    {
        if (photonView.IsMine)
        {
            HasExtinguisher = true;
            NetworkManager.User.hasExtingisher = true;
            Debug.Log("Set Extingusher is true");
        }
    }

    public void OnExtinguisher(Transform pivot)
    {
        Debug.Log("OnExtinguisher");
        if (photonView.IsMine)
        {
            Debug.Log("OnExtinguisher Is Mine");

            extinguisherObject = PhotonNetwork.Instantiate("Extinguisher", pivot.position, Quaternion.identity);
            extinguisherObject.GetComponent<Extinguisher>().extinguisherOrigin = pivot;
            extinguisherObject.GetComponent<InteractableObject>().enabled = true;
            //onExtinguisher = true;
            photonView.RPC(nameof(OnExtinguisherRPC), RpcTarget.All, extinguisherObject.name);
        }
    }
    [PunRPC]
    public void OnExtinguisherRPC(string name)
    {
        extinguisher = FindObjectOfType<Extinguisher>();
        pinTrigger = extinguisher.pinTrigger.GetComponent<PinTrigger>();
        hose = extinguisher.hose;
        hoseWater = extinguisher.hoseWater;
        nozzle = extinguisher.nozzle.GetComponent<Nozzle>();
        hose.SetActive(true);
        extinguisher.gameObject.SetActive(true);
    }

    public void OffExtinguisher()
    {
        photonView.RPC(nameof(OffExtinguisherRPC), RpcTarget.All);
    }

    [PunRPC]
    private void OffExtinguisherRPC()
    {
        if (photonView.IsMine)
        {
            extinguisher.gameObject.SetActive(false);
            hose.gameObject.SetActive(false);
        }
        else
        {
            extinguisher = FindObjectOfType<Extinguisher>();
            nozzle = FindObjectOfType<Nozzle>();

            extinguisher.gameObject.SetActive(false);
            nozzle.gameObject.SetActive(false);
        }
    }

    public void Spread(bool value)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(SpreadRPC), RpcTarget.All, value);
        }
    }

    [PunRPC]
    public void SpreadRPC(bool value)
    {
        if (hoseWater == null || nozzle == null)
        {
            return;
        }
        if (value)
        {
            if (!nozzle.isSelected)
            {
                hoseWater.Stop();
            }
            else
            {
                hoseWater.Play();
            }
        }
        else
        {
            hoseWater.Stop();
        }
    }

    public void Initialize()
    {
        if (photonView.IsMine)
        {
            UserID = NetworkManager.User.email;
            UserName = NetworkManager.User.name;
            UserLevel = NetworkManager.User.userType;
            HasExtinguisher = NetworkManager.User.hasExtingisher;
            CurrentCharacter = NetworkManager.User.characterNumber;

            userInfoUI.SetActive(false);

            maleHelmet.transform.GetChild(0).gameObject.layer = 31;
            maleHelmet.transform.GetChild(1).gameObject.layer = 31;
            femaleHelmet.transform.GetChild(0).gameObject.layer = 31;
            femaleHelmet.transform.GetChild(1).gameObject.layer = 31;
            maleMesh.layer = 31;
            femaleMesh.layer = 31;
            headModel.layer = 31;
            leftHand.gameObject.layer = 31;
            rightHand.gameObject.layer = 31;
            outlineObject.gameObject.layer = 31;

            //if (NetworkManager.Instance.roomType == RoomType.Room)
            //{
            //    FindObjectOfType<TextChatManager>().sendChatMessage += OnSendChatMessage;
            //}
            FindObjectOfType<TextChatManager>().sendChatMessage = null;
            FindObjectOfType<TextChatManager>().sendChatMessage += OnSendChatMessage;
        }

        //requestVoiceChatButton.onClick.AddListener(() => LoundgeSceneManager.Instance.RequsetVoiceChat(NetworkManager.User.id, UserID));
    }

    private void OnDestroy()
    {
        FindObjectOfType<TextChatManager>().sendChatMessage = null;
    }

    [PunRPC]
    public void InitializeRPC()
    {
        userNameUI.text = NetworkManager.User.name;
    }
    public void SetExtinguisher()
    {
        onExtinguisher = true;
        extinguisherObject = PhotonNetwork.Instantiate("Extinguisher", extinguisherPivot.position, extinguisherPivot.rotation);
    }
    public void UpdateDistanceUI(float distance)
    {

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
            rightHandAnimator.SetInteger("Pose", 0);
            rightHandAnimator.SetFloat("Flex", rightHandAnimation.flex);
            rightHandAnimator.SetLayerWeight(2, rightHandAnimation.animator.GetLayerWeight(2));
            rightHandAnimator.SetLayerWeight(1, rightHandAnimation.animator.GetLayerWeight(1));

            genderRightHandAnimator.SetInteger("Pose", 0);
            genderRightHandAnimator.SetFloat("Flex", rightHandAnimation.flex);
            genderRightHandAnimator.SetLayerWeight(2, rightHandAnimation.animator.GetLayerWeight(2));
            genderRightHandAnimator.SetLayerWeight(1, rightHandAnimation.animator.GetLayerWeight(1));
        }
        else
        {
            int pose = rightHandAnimation.poseIndex;

            rightHandAnimator.SetInteger("Pose", pose);
            rightHandAnimator.SetLayerWeight(2, 0);
            rightHandAnimator.SetLayerWeight(1, 0);

            genderRightHandAnimator.SetInteger("Pose", pose);
            genderRightHandAnimator.SetLayerWeight(2, 0);
            genderRightHandAnimator.SetLayerWeight(1, 0);
        }

        if (leftHandAnimation.animator.GetInteger("Pose") == 0)
        {
            leftHandAnimator.SetInteger("Pose", 0);
            leftHandAnimator.SetFloat("Flex", leftHandAnimation.flex);
            leftHandAnimator.SetLayerWeight(2, leftHandAnimation.animator.GetLayerWeight(2));
            leftHandAnimator.SetLayerWeight(1, leftHandAnimation.animator.GetLayerWeight(1));

            genderLeftHandAnimator.SetInteger("Pose", 0);
            genderLeftHandAnimator.SetFloat("Flex", leftHandAnimation.flex);
            genderLeftHandAnimator.SetLayerWeight(2, leftHandAnimation.animator.GetLayerWeight(2));
            genderLeftHandAnimator.SetLayerWeight(1, leftHandAnimation.animator.GetLayerWeight(1));
        }
        else
        {
            int pose = leftHandAnimation.poseIndex;

            leftHandAnimator.SetInteger("Pose", pose);
            leftHandAnimator.SetLayerWeight(2, 0);
            leftHandAnimator.SetLayerWeight(1, 0);

            genderLeftHandAnimator.SetInteger("Pose", pose);
            genderLeftHandAnimator.SetLayerWeight(2, 0);
            genderLeftHandAnimator.SetLayerWeight(1, 0);
        }
    }

    public void InteractionTest()
    {
     //   Debug.Log("On Cursor Hoverd");
    }

    private void SetCurrentCharacter(int value)
    {
        if (value < meshs.Length / 2)
        {
            //male
            femaleRig.SetActive(false);
            maleRig.SetActive(true);

            outlineObject = outlineMale;
            extinguisherPivot = extinguisherPivotMale;
            
            skinnedMeshRenderer = skinnedMeshRendererMale;

            genderLeftHandAnimator = maleLeftHandAnimator;
            genderRightHandAnimator = maleRightHandAnimator;
        }
        else
        {
            //female
            maleRig.SetActive(false);
            femaleRig.SetActive(true);

            outlineObject = outlineFemale;
            extinguisherPivot = extinguisherPivotFemale;

            skinnedMeshRenderer = skinnedMeshRendererFemale;

            genderLeftHandAnimator = femaleLeftHandAnimator;
            genderRightHandAnimator = femaleRightHandAnimator;
        }

        skinnedMeshRenderer.sharedMaterial = materials[value];
        skinnedMeshRenderer.sharedMesh = meshs[value];
        outlineObject.sharedMesh = meshs[value];
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
            popChat = null;
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

        megaphoneIcon.gameObject.SetActive(true);
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

        megaphoneIcon.gameObject.SetActive(false);
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
