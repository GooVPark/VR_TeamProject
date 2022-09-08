using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using HashTable = ExitGames.Client.Photon.Hashtable;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public enum RoomType { Login, Room, Loundge }
    public RoomType roomType;

    public static NetworkManager Instance;
    public Dictionary<string, RoomInfo> roomsByName = new Dictionary<string, RoomInfo>();
    public List<RoomInfo> roomList = new List<RoomInfo>();

    private static User user;
    public static User User => user; //�̱��� ������(��𼭵� ����) networkmanager.user�� �����

    private static int roomNumber;
    public static int RoomNumber => roomNumber;

    private string gameVersion = "1.0";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
    float elapsedTime = 0f;

    private void Update()
    {

    }

    public void SetUser(User _user)
    {
        user = _user;
    }

    public void SetRoomNumber(int _roomNumber)
    {
        roomNumber = _roomNumber;
    }

    #region Lobby

    #endregion

    #region Room

    public int GetPlayerCount(int roomNumber)
    {
        if(roomList.Count <= roomNumber)
        {
            return 0;
        }
        else
        {
            return roomList[roomNumber].PlayerCount;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC(nameof(ChatRPC), RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "���� ���� �ϼ̽��ϴ�</color>");

        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

    }

    public override void OnJoinedRoom()
    {

        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("NetworkManager : OnCreateRoom");
    }

    #endregion

    #region Chat

    public delegate void ChatCallbackEvent(string message);
    public static ChatCallbackEvent ChatCallback;

    public void SendChat(string msg)
    {
        photonView.RPC(nameof(ChatRPC), RpcTarget.All, msg);
    }

    [PunRPC]
    private void ChatRPC(string msg)
    {
        ChatCallback(msg);
    }

    #endregion

    #region UserStatus

    public bool hasExtingusher = false;

    public void OnSelectExtingusher(bool hasExtingusher)
    {
        
    }

    #endregion

    public void PullRoomList()
    {
        RoomPuller roomPoller = gameObject.AddComponent<RoomPuller>();
        roomPoller.OnGetRoomsInfo
        (
            (roomInfos) =>
            {
                // �븮��Ʈ�� �ް��� �۾� �ڵ� �ֱ�
                Debug.Log($"���� �� ���� : {roomInfos.Count}");
                for(int i = 0; i < roomInfos.Count; i++)
                {
                    Debug.Log($"�� {roomInfos[i].Name}�� �ο� ��: {roomInfos[i].PlayerCount}");
                }

                roomList = roomInfos;
                // �������� ������Ʈ �������ֱ�
                Destroy(roomPoller);
            }
        );
    }

    public override void OnConnectedToMaster()
    {
        DataManager.Instance.SetOnline(User.email);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DataManager.Instance.SetOffline(User.email);
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.SetOffline(User.email);
    }
}