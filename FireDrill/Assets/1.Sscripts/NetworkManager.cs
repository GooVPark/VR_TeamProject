using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Chat.UtilityScripts;
using TMPro;
using HashTable = ExitGames.Client.Photon.Hashtable;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public enum RoomType { Login, Room, Loundge }
    public RoomType roomType;

    public static NetworkManager Instance;
    public Dictionary<string, RoomInfo> roomsByName = new Dictionary<string, RoomInfo>();
    public List<RoomInfo> roomList = new List<RoomInfo>();
    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private static User user;
    public static User User => user;

    private static int roomNumber;
    public static int RoomNumber => roomNumber;

    private string gameVersion = "1.0";

    private bool onChangeRoom = false;

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
        DataManager.Instance.UpdateCurrentRoom(user.email, _roomNumber);
    }

    #region Lobby
    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }
    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    #endregion

    #region Room

    public int GetPlayerCountOnRooms()
    {
        int playerCount = 0;
        foreach(string key in cachedRoomList.Keys)
        {
            playerCount += cachedRoomList[key].PlayerCount;
        }
        
        return playerCount;
    }

    public int GetPlayerCount(int roomNumber)
    {
        string roomName = roomNumber.ToString();
        if (cachedRoomList.ContainsKey(roomName))
        {
            return cachedRoomList[roomName].PlayerCount;
        }
        else
        {
            return 0;
        }
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
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
                // 룸리스트를 받고나서 작업 코드 넣기
                Debug.Log($"현재 방 갯수 : {roomInfos.Count}");
                for(int i = 0; i < roomInfos.Count; i++)
                {
                    Debug.Log($"룸 {roomInfos[i].Name}의 인원 수: {roomInfos[i].PlayerCount}");
                }

                roomList = roomInfos;
                // 마지막엔 오브젝트 제거해주기
                Destroy(roomPoller);
            }
        );
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DataManager.Instance.SetOffline(User.email);
        cachedRoomList.Clear();
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.SetOffline(User.email);
    }
}