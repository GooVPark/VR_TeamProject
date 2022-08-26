using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using HashTable = ExitGames.Client.Photon.Hashtable;


public class NetworkManager : MonoBehaviourPunCallbacks
{ 
    public delegate void ConnectedToMasterServerEvent();
    public static ConnectedToMasterServerEvent onConnectedToMasterServer;
    
    public delegate void JoinedLobbyEvent();
    public static JoinedLobbyEvent onJoinedLobby;

    public static NetworkManager Instance;

    private LogInData loginData;

    private static string userName;
    public static string UserName { get { return userName; } }

    private static Authority userLevel;
    public static Authority UserLevel { get { return userLevel; } }

    private static UserData userData;
    public static UserData UserData { get => userData; }

    private string gameVersion = "1.0";

    public GameObject roomListUI;
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> roomInfoDict = new Dictionary<string, GameObject>();
    public List<GameObject> rooms;
    public GameObject roomPrefab;


    public TMP_InputField chatInput;
    public TMP_Text[] chatContents;

    private bool isJoinRoom = false;
    private int roomIndex = -1;

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

        //for (int i = 0; i < rooms.Count; i++)
        //{
        //    roomInfoDict.Add($"Room_{i + 1:000}", rooms[i]);
        //    rooms[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Room_{i + 1:000} (0/11)";
        //}
    }

    #region LogIn

    //public bool OnTryLogin(string email, string password)
    //{
    //    if (email == string.Empty || password == string.Empty)
    //    {
    //        return false;
    //    }
    //    if (DataManager.Instance.FindUserData(email, password, out userData))
    //    {
    //        Connect(userData);
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    #endregion

    public void Connect(UserData _userData)
    {
        userData = _userData;
        userName = userData.name;
        userLevel = userData.authority;

        PhotonNetwork.LocalPlayer.NickName = userName;

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        onConnectedToMasterServer?.Invoke();
    }

    #region Lobby

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        onJoinedLobby?.Invoke();

        PhotonNetwork.LoadLevel("Loundge");
        //GameObject playerObject = PhotonNetwork.Instantiate("Player", transform.position, transform.rotation);
        //roomListUI.SetActive(true);
        //if (isJoinRoom)
        //{
        //    isJoinRoom = false;
        //    string roomName = $"Room_{roomIndex + 1:000}";
        //    if (!roomDict.ContainsKey(roomName))
        //    {
        //        RoomOptions roomOptions = new RoomOptions();
        //        roomOptions.IsOpen = true;
        //        roomOptions.IsVisible = true;
        //        roomOptions.MaxPlayers = 11;

        //        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        //    }
        //    else
        //    {
        //        PhotonNetwork.JoinRoom(roomName);
        //    }
        //}
    }
    #endregion

    #region Room

    //public void JoinRoom()
    //{
    //    PhotonNetwork.NickName = UserName;

    //    string roomName = $"Loundge";

    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.IsOpen = true;
    //    roomOptions.IsVisible = true;
    //    roomOptions.MaxPlayers = 11;

    //    PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    //}

    //public void JoinRoom(int roomNumber)
    //{
    //    isJoinRoom = true;
    //    roomIndex = roomNumber;
    //    PhotonNetwork.LeaveRoom(false);
    //}

    //public override void OnJoinedRoom()
    //{
    //    chatInput.text = "";
    //    for (int i = 0; i < chatContents.Length; i++)
    //    {
    //        chatContents[i].text = "";
    //    }

    //    if (!isJoinRoom)
    //    {
    //        GameObject playerObject = PhotonNetwork.Instantiate("Player", transform.position, transform.rotation);
    //        roomListUI.SetActive(false);
    //    }
    //}

    
    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    photonView.RPC(nameof(ChatRPC), RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "���� ���� �ϼ̽��ϴ�</color>");
    //    if (newPlayer != PhotonNetwork.LocalPlayer)
    //    {
    //        ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
    //    }
    //}

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    photonView.RPC(nameof(ChatRPC), RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "���� ���� �ϼ̽��ϴ�</color>");
    //}

    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    GameObject tempRoom = null;
    //    foreach (var room in roomList)
    //    {
    //        if (room.RemovedFromList == true)
    //        {
    //            roomDict.TryGetValue(room.Name, out tempRoom);
    //            Destroy(tempRoom);
    //            roomDict.Remove(room.Name);
    //        }
    //        else
    //        {
    //            if (!roomDict.ContainsKey(room.Name))
    //            {
    //                GameObject roomEntity = Instantiate(roomPrefab);

    //                roomEntity.GetComponent<RoomData>().SetRoomInfo(room, roomInfoDict[room.Name]);
    //                roomDict.Add(room.Name, roomEntity);
    //            }
    //            else
    //             {
    //                roomDict.TryGetValue(room.Name, out tempRoom);
    //                tempRoom.GetComponent<RoomData>().SetRoomInfo(room, roomInfoDict[room.Name]);
    //            }
    //        }
    //    }
    //}

    #endregion

    #region Chat

    public void SendChat()
    {
        string msg = PhotonNetwork.NickName + " : " + chatInput.text;
        photonView.RPC(nameof(ChatRPC), RpcTarget.All, msg);
        chatInput.text = "";
    }

    [PunRPC]
    private void ChatRPC(string msg)
    {
        bool isInput = false;

        for (int i = 0; i < chatContents.Length; i++)
        {
            if (chatContents[i].text == "")
            {
                isInput = true;
                chatContents[i].text = msg;
                break;
            }
        }

        if (!isInput)
        {
            for (int i = 1; i < chatContents.Length; i++)
            {
                chatContents[i - 1].text = chatContents[i].text;
            }

            chatContents[chatContents.Length - 1].text = msg;
        }
    }

    #endregion

    #region UserStatus

    public bool hasExtingusher = false;

    public void OnSelectExtingusher(bool hasExtingusher)
    {
        
    }

    #endregion
}