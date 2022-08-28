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

    public delegate void ConnectedToMasterServerEvent();
    public static ConnectedToMasterServerEvent onConnectedToMasterServer;
    
    public delegate void JoinedLobbyEvent();
    public static JoinedLobbyEvent onJoinedLobby;

    public static NetworkManager Instance;

    private LogInData loginData;

    private static string userName;
    public static string UserName { get { return userName; } }

    private static UserType userLevel;
    public static UserType UserLevel { get { return userLevel; } }

    private static User userData;
    public static User UserData { get => userData; }

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



    public void Connect(User _userData)
    {
        userData = _userData;
        userName = userData.name;
        userLevel = userData.userType;

        PhotonNetwork.LocalPlayer.NickName = userName;

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        onConnectedToMasterServer?.Invoke();
        //JoinLobby();
    }

    #region Lobby

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        onJoinedLobby?.Invoke();
        roomType = RoomType.Loundge;

        string roomName = $"Loundge";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 0;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    #endregion

    #region Room

    public void JoinRoom()
    {
        string roomName = $"Loundge";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 0;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public void JoinRoom(int roomNumber)
    {
        isJoinRoom = true;
        roomIndex = roomNumber;
        PhotonNetwork.LeaveRoom(false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("NetworkMnanager : OnJoinedRoom");
        switch (roomType)
        {
            case RoomType.Room:
                break;
            case RoomType.Loundge:
                PhotonNetwork.LoadLevel("Loundge");
                break;
        }
        chatInput.text = "";
        for (int i = 0; i < chatContents.Length; i++)
        {
            chatContents[i].text = "";
        }

        //if (!isJoinRoom)
        //{
        //    GameObject playerObject = PhotonNetwork.Instantiate("Player", transform.position, transform.rotation);
        //    roomListUI.SetActive(false);
        //}
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC(nameof(ChatRPC), RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "¥‘¿Ã ¬¸∞° «œºÃΩ¿¥œ¥Ÿ</color>");
        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
        }
    }

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    photonView.RPC(nameof(ChatRPC), RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "¥‘¿Ã ≈¿Â «œºÃΩ¿¥œ¥Ÿ</color>");
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
}