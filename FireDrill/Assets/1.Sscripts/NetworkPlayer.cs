using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using TMPro;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    #region 동기화 프로퍼티

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

    private void ActionRPC(string functionName, object value)
    {
        photonView.RPC(functionName, RpcTarget.All, value);
    }

    public void InvokeProperties()
    {
        UserName = UserName;
        UserLevel = UserLevel;
    }
    #endregion
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;

    public TMP_Text userNameUI;
    public TMP_Text userLevelUI;
    public TMP_Text distanceUI;

    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    private Camera headset;

    // Start is called before the first frame update
    void Start()
    {
        Transform parent = GameObject.Find("Players").transform;
        transform.SetParent(parent);
        photonView = GetComponent<PhotonView>();

        headset = Camera.main;
        leftController = GameObject.Find("LeftHand Controller").GetComponent<ActionBasedController>();
        rightController = GameObject.Find("RightHand Controller").GetComponent<ActionBasedController>();

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
            UserName = NetworkManager.UserName;
            UserLevel = NetworkManager.UserLevel.ToString();
        }
    }

    [PunRPC]
    public void InitializeRPC()
    {
        userNameUI.text = NetworkManager.UserName;
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

    #region Photon Interfaces

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    #endregion
}
