using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using HashTable = ExitGames.Client.Photon.Hashtable;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public enum RoomType { Room, Loundge }
    public RoomType roomType;

    public static NetworkManager Instance;
    public Dictionary<string, RoomInfo> roomsByName = new Dictionary<string, RoomInfo>();
    public List<RoomInfo> roomList = new List<RoomInfo>();

    private static User user;
    public static User User => user;

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
        elapsedTime += Time.deltaTime;
        
        if(elapsedTime > 1f)
        {
            PullRoomList();
            elapsedTime = 0f;
        }
    }

    public void SetUser(User _user)
    {
        user = _user;
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            if (!roomsByName.ContainsKey(roomList[i].Name))
            {
                roomsByName.Add(roomList[i].Name, roomList[i]);
            }
            
        }
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
}