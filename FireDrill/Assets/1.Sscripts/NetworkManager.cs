using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class NetworkManager : MonoBehaviourPunCallbacks
{
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
    

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(Instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            roomInfoDict.Add($"Room_{i + 1:000}", rooms[i]);
            rooms[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Room_{i + 1:000} (0/11)";
        }
    }

    public void Connect(UserData _userData)
    {
        userData = _userData;
        userName = userData.name;
        userLevel = userData.authority;

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        roomListUI.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        roomListUI.SetActive(false);
        GameObject playerObject = PhotonNetwork.Instantiate("Player", transform.position, transform.rotation);
        NetworkPlayer player = playerObject.GetComponent<NetworkPlayer>();
        Debug.Log("NetworkManager : Joined Room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(newPlayer != PhotonNetwork.LocalPlayer)
        {
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach (var room in roomList)
        {
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                if (!roomDict.ContainsKey(room.Name))
                {
                    GameObject roomEntity = Instantiate(roomPrefab);

                    roomEntity.GetComponent<RoomData>().SetRoomInfo(room, roomInfoDict[room.Name]);
                    roomDict.Add(room.Name, roomEntity);
                }
                else
                 {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().SetRoomInfo(room, roomInfoDict[room.Name]);
                }
            }
        }
    }

    public void JoinRoom(int roomNumber)
    {
        PhotonNetwork.NickName = UserName;

        string roomName = $"Room_{roomNumber + 1:000}";
        if (!roomDict.ContainsKey(roomName))
        {
            Debug.Log("Create");
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 10;

            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.Log("Join");
            PhotonNetwork.JoinRoom(roomName);
        }
    }
}