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

public enum RoomType { Login, Room, VoiceRoom, Loundge }

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public RoomType roomType;

    public static NetworkManager Instance;
    public Dictionary<string, RoomInfo> roomsByName = new Dictionary<string, RoomInfo>();
    public List<RoomInfo> roomList = new List<RoomInfo>();
    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private static User user;
    public static User User => user;

    private static LoundgeUser loundgeUser;
    public static LoundgeUser LoundgeUser => loundgeUser;

    public bool inFireControl = false;

    private static int roomNumber;
    public static int RoomNumber => roomNumber;

    private string gameVersion = "1.0";

    private bool onChangeRoom = false;

    #region 플레이어 화면의 메인 UI 버튼 파라미터

    public bool onMegaphone = false;
    public bool megaphoneDisabled = false;

    public bool onScoreBoard = false;
    public bool scoreBoardDisabled = false;

    public bool onVoiceChat = false;
    public bool voiceChatDisabled = false;

    public bool onTextChat = false;
    public bool textChatDisabled = false;

    #endregion

    public string fpsInfo;
    public string fps;
    public string ping;

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
    float deltaTime = 0.0f;

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    public void SetUser(User _user)
    {
        user = _user;
    }

    public void SetLoundgeUser(LoundgeUser _loundgeUser)
    {
        loundgeUser = _loundgeUser;
    }
    public void SetRoomNumber(int _roomNumber)
    {
        roomNumber = _roomNumber;
        //DataManager.Instance.UpdateCurrentRoom(user.email, _roomNumber);
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

    #region UserStatus

    public bool hasExtingusher = false;

    public void OnSelectExtingusher(bool hasExtingusher)
    {
        
    }

    #endregion

    public bool IsMine(string email)
    {
        if(photonView.IsMine  && email.Equals(user.email))
        {
            return true;
        }

        return false;
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

    public void ServerDownEvent()
    {
        Application.Quit();
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        fpsInfo = text;
        ping = msec.ToString();
        this.fps = fps.ToString();
        GUI.Label(rect, text, style);
    }
}