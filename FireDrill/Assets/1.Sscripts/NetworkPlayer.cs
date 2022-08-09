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

    private void ActionRPC(string functionName, object value)
    {
        photonView.RPC(functionName, RpcTarget.All, value);
    }

    public void InvokeProperties()
    {
        UserName = UserName;
    }
    #endregion
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;

    public TMP_Text userNameUI;

    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    private Camera headset;

    // Start is called before the first frame update
    void Start()
    {
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
            UserName = PhotonNetwork.LocalPlayer.NickName;
        }
    }

    [PunRPC]
    public void InitializeRPC()
    {
        userNameUI.text = NetworkManager.UserName;
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

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }
}
