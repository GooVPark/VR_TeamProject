using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoundgeSceneManager : GameManager
{
    public delegate void Eventmessage(string message);
    public static Eventmessage eventMesage;

    public static LoundgeSceneManager Instance;

    public VoiceManager voiceManager;
    public EventSyncronizer eventSyncronizer;
    public Transform NPCTransforms;
    public SpawnPosition spawnPosition;

    public Dictionary<string, LoundgeUser> spawnedNPC = new Dictionary<string, LoundgeUser>();
    public Dictionary<string, GameObject> spawnedNPCObject = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject npcPrefab;

    private bool isOnline = false;
    public bool isEventServerConnected = false;

    [SerializeField] private GameObject voiceChatErrorToast;
    [SerializeField] private GameObject megaphoneErrorToast;
    [SerializeField] private GameObject scoreboardErrorToast;

    [SerializeField] private List<RoomEnterance> roomEnterances;

    [SerializeField] private Announcement announcement;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
        }
    }

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        UpdateRoomPlayerCount(0);
    }
    
    private Coroutine initializer;
    private IEnumerator Initializer()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.3f);
        while (!isOnline || !isEventServerConnected)
        {
            isOnline = DataManager.Instance.FindLobbyUser(NetworkManager.User);
            yield return waitForSeconds;
        }
        
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);

        NetworkManager.Instance.SetRoomNumber(roomNumber);
        NetworkManager.Instance.roomType = RoomType.Loundge;
        NetworkManager.Instance.megaphoneDisabled = true;
        NetworkManager.Instance.voiceChatDisabled = true;
        NetworkManager.Instance.onVoiceChat = false;
        NetworkManager.Instance.hasExtingusher = false;
        NetworkManager.User.hasExtingisher = false;

        string message = $"{EventMessageType.SPAWN}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);

        for (int i = 0; i < playerCountsText.Length; i++)
        {
            UpdateRoomPlayerCount(i);
        }

        for (int i = 0; i < roomEnterances.Count; i++)
        {
            UpdateRoomEnterence(i);
        }

        StopCoroutine(initializer);
    }

    public void SpawnNPC()
    {
        List<LoundgeUser> loundgeUsers = DataManager.Instance.GetLoundgeUsers();
        foreach(LoundgeUser loundgeUser in loundgeUsers)
        {
            if(!spawnedNPC.ContainsKey(loundgeUser.email))
            {
                spawnedNPC.Add(loundgeUser.email, loundgeUser);

                Vector2 randomValue = Random.insideUnitCircle * 3f;
                Vector3 spawnPosition = this.spawnPosition.transform.position + new Vector3(randomValue.x, 0, randomValue.y);

                GameObject npcObject = Instantiate(npcPrefab, spawnPosition, Quaternion.identity, NPCTransforms);
                spawnedNPCObject.Add(loundgeUser.email, npcObject);
                NPCController npc = npcObject.GetComponent<NPCController>();
                npc.Initialize(loundgeUser);
                npc.eventMessage += eventSyncronizer.OnSendMessage;

                if(NetworkManager.Instance.onVoiceChat)
                {
                    npc.senderIsOnVoiceChat = true;
                }

                if (loundgeUser.email.Equals(NetworkManager.User.email))
                {
                    npcObject.SetActive(false);
                }
            }
        }
    }

    public void JoinVoiceChatRoom(string userID)
    {
        if(!NetworkManager.Instance.onVoiceChat)
        {
            return;
        }
        Debug.Log("LoundgeManager: JoinVoiceChatRoom");
        
        string roomName = $"VoiceChatRoom_{userID}";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        //roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        NetworkManager.Instance.roomType = RoomType.VoiceRoom;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }
    public void VoiceChatToRoomErrorToast()
    {
        voiceChatErrorToast.gameObject.SetActive(true);
    }

    public void LeaveVoiceChatAndEnterRoom()
    {
        voiceChatErrorToast.SetActive(false);
        voiceManager.DisconnectVoiceChat();
    }

    public void KeepVoiceChat()
    {
        voiceChatErrorToast.SetActive(false);
    }


    public void RemoveNPCObject(string email)
    {
        Destroy(spawnedNPCObject[email]);

        if(spawnedNPC.ContainsKey(email))
        {
            spawnedNPC.Remove(email);
        }
        if (spawnedNPCObject.ContainsKey(email))
        {
            spawnedNPCObject.Remove(email);
        }

        DataManager.Instance.DeleteLobbyUser(email);
    }

    public void LeaveVoiceChatRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public LoundgeUser GetLoundgeUser(string email)
    {
        if(spawnedNPC.ContainsKey(email))
        {
            return spawnedNPC[email];
        }
        else
        {
            return null;
        }
    }

    public void MegaphoneSelected()
    {
        megaphoneErrorToast.SetActive(true);
    }

    public void ScoreBoardSelected()
    {
        scoreboardErrorToast.SetActive(true);
    }

    private void CloseToast()
    {
        megaphoneErrorToast.SetActive(false);
    }

    public void UpdateRoomEnterence(int roomNumber)
    {
        bool isStarted = DataManager.Instance.GetRoomProgressState(roomNumber);
        roomEnterances[roomNumber].isStarted = isStarted;
    }

    #region Database

    private void InsertUserData()
    {
        DataManager.Instance.InsertLobbyUser(NetworkManager.User);
    }
    
    private void DeleteUserData()
    {

    }

    #endregion


    #region Notice Board

    [Header("Notice Board")]
    [SerializeField] private List<PostUI> posts = new List<PostUI>();
    [SerializeField] private int currentPage = 0;
    [SerializeField] private int totalPage = 0;
    [SerializeField] private float refreshTime = 10f;
    [SerializeField] private float refreshTimeCount = 0f;
    [Space(5)]

    [Header("Post Window")]
    [SerializeField] private PostContext postWindow;
    [SerializeField] private Button returnToBoardButton;

    [Space(5)]

    [Header("Post Write")]
    [SerializeField] private GameObject postingWindow;
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField bodyInput;

    [Space(5)]

    [SerializeField] private Button postCheckButton;
    [SerializeField] private GameObject postCheck;
    [SerializeField] private Button confirmPostButton;
    [SerializeField] private Button canclePostButton;

    [Space(5)]

    [SerializeField] private Button canclePostCheckButton;
    [SerializeField] GameObject cancleCheck;
    [SerializeField] private Button confirmCancleButton;
    [SerializeField] private Button cancleCancleButton;


    public void ShowNextPage()
    {
        if (currentPage == totalPage - 1) return;
        currentPage++;

        RefreshNoticeBoard();
    }

    public void ShowPrevPage()
    {
        if (currentPage == 0) return;
        currentPage--;

        RefreshNoticeBoard();
    }

    public void LoadFirstPage()
    {
        currentPage = 0;
        RefreshNoticeBoard();
    }

    public void RefreshNoticeBoard()
    {
        List<Post> currentPosts = DataManager.Instance.GetPostInCurrentPage(currentPage, posts.Count);
        totalPage = (int)Mathf.Ceil((float)DataManager.Instance.GetLastPostNumber() / (float)posts.Count);
        totalPage = Mathf.Clamp(totalPage, 1, totalPage);

        for(int i = 0; i < currentPosts.Count; i++)
        {
            posts[i].gameObject.SetActive(true);
            posts[i].UpdatePost(currentPosts[i]);
        }

        for(int i = currentPosts.Count; i < posts.Count; i++)
        {
            posts[i].gameObject.SetActive(false);
        }
    }

    public void ShowPost(Post post)
    {
        postWindow.gameObject.SetActive(true);
        postWindow.UpdatePost(post);
    }

    public void BackFromPost()
    {
        postWindow.gameObject.SetActive(false);
    }


    public void WritePost()
    {
        postingWindow.gameObject.SetActive(true);
        titleInput.text = "";
        bodyInput.text = "";
    }


    public void UploadPost()
    {
        postCheck.SetActive(true);
    }

    public void CheckUploadPost(bool value)
    {
        postCheck.SetActive(false);

        if (value)
        {
            int postNumber = DataManager.Instance.GetLastPostNumber();
            string title = titleInput.text;
            string body = bodyInput.text;

            string writer = NetworkManager.User.name;
            string uploadDate = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

            Post post = new Post();
            post.uploadTime = uploadDate;
            post.postNumber = postNumber;
            post.writer = writer;
            post.title = title;
            post.body = body;

            DataManager.Instance.UploadPost(post);
            LoadFirstPage();

            postingWindow.SetActive(false);
        }
    }

    public void CanclePosting()
    {
        cancleCheck.SetActive(true);
    }

    public void CheckCanclePosting(bool value)
    {
        cancleCheck.SetActive(false);
        if(value) postingWindow.SetActive(false);
    }



    #endregion

    #region Training Progress Boards

    [Header("Progress Board")]
    [SerializeField] private List<RoomData> roomDatas;
    [SerializeField] private List<ProgressUI> progresses;
    [SerializeField] private TMP_Text playerCountText;

    [SerializeField] private Image[] progressUIs;
    [SerializeField] private Sprite[] progressImages;

    [SerializeField] private TMP_Text[] playerCountsText;

    private float progressBoardElapsedTime = 0f;
    private float progressBoardUpdateInterval = 1f;

    public void UpdateProgressBoard()
    {
        roomDatas = DataManager.Instance.GetRoomData();

        for(int i = 0; i < roomDatas.Count; i++)
        {
            progressUIs[i].sprite = progressImages[roomDatas[i].progress];
        }
    }
    public void UpdateLobbyPlayerCount()
    {
        playerCountText.text = DataManager.Instance.GetLoundgeUsers().Count.ToString();
    }
    public void UpdateRoomPlayerCount(int roomNumber)
    {
        //RoomData roomData = DataManager.Instance.GetRoomData(roomNumber);

        //int playerCount = roomData.currentPlayerCount < 0 ? 0 : roomData.currentPlayerCount;
        //int maxPlayerCount = roomData.maxPlayerCount;
        int playerCount = 0;
        if (cachedRoomList.ContainsKey(roomNumber.ToString()))
        {
            playerCount = cachedRoomList[roomNumber.ToString()].PlayerCount;
        }

        playerCountsText[roomNumber].text = $"{playerCount}/16";
    }
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
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

    #endregion

    #region Room State Lamp

    public Lamp[] lamps;

    public void UpdateRoomStateLamp()
    {
        roomDatas = DataManager.Instance.GetRoomData();

        for (int i = 0; i < roomDatas.Count; i++)
        {
            lamps[i].UpdateLampState(roomDatas[i]);
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        Debug.Log("Loundge Maanger: Joined Lobby");

        Initialize();

        NetworkManager.Instance.inFireControl = false;
        NetworkManager.Instance.voiceChatDisabled = true;
        NetworkManager.Instance.scoreBoardDisabled = true;
        NetworkManager.Instance.onTextChat = false;
        NetworkManager.Instance.roomType = RoomType.Loundge;
        NetworkManager.Instance.SetRoomNumber(roomNumber);

        SetIdleMode(IdleMode.STAND);

        InsertUserData();

        //NetworkManager.Instance.PullRoomList();
        LoadFirstPage();
        UpdateProgressBoard();

        initializer = StartCoroutine(Initializer());
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public int roomNumber;
    private string roomName;
    private RoomOptions roomOptions;
    public void JoinRoom(int roomNumber)
    {
        string message = $"{EventMessageType.DISCONNECT}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);

        DataManager.Instance.DeleteLobbyUser(NetworkManager.User);

        Debug.Log("Join Room: " + roomNumber);
        //textChatManager.DisconnectChat();
        //eventSyncronizer.DisconnectChat();
        NetworkManager.Instance.SetRoomNumber(roomNumber);
        roomName = roomNumber.ToString();

        //NetworkManager.Instance.roomType = NetworkManager.RoomType.Room;

        roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 0;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnJoinedRoom()
    {
        switch (NetworkManager.Instance.roomType)
        {
            case RoomType.Room:
                break;
            case RoomType.VoiceRoom:

                spawnedNPCObject[voiceManager.sender.email].SetActive(false);
                spawnedNPCObject[voiceManager.reciever.email].SetActive(false);

                voiceChatButton.button.OnClick.AddListener(() => voiceManager.DisconnectVoiceChat());
                NetworkManager.Instance.voiceChatDisabled = false;
                NetworkManager.Instance.onVoiceChat = true;
                FindObjectOfType<Photon.Voice.Unity.Recorder>().TransmitEnabled = true;

                announcement.StopAudio();

                foreach(string key in spawnedNPCObject.Keys)
                {
                    spawnedNPCObject[key].GetComponent<NPCController>().SetVoiceChatState(true);
                }

                break;
            case RoomType.Loundge:
                break;
        }

        PhotonNetwork.Instantiate("Player", Vector3.one, Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        switch (NetworkManager.Instance.roomType)
        {
            case RoomType.Room:
                break;
            case RoomType.VoiceRoom:

                if(voiceManager.sender.email == NetworkManager.User.email)
                {
                    GameObject npcObject = null;
                    if (spawnedNPCObject.TryGetValue(voiceManager.reciever.email, out npcObject))
                    {
                        npcObject.SetActive(true);
                    }
                }
                if(voiceManager.reciever.email == NetworkManager.User.email)
                {
                    GameObject npcObject = null;
                    if (spawnedNPCObject.TryGetValue(voiceManager.sender.email, out npcObject))
                    {
                        npcObject.SetActive(true);
                    }
                }

                NetworkManager.Instance.roomType = RoomType.Loundge;
                NetworkManager.Instance.onVoiceChat = false;
                NetworkManager.Instance.voiceChatDisabled = true;
                FindObjectOfType<Photon.Voice.Unity.Recorder>().TransmitEnabled = false;

                announcement.PlayAudio();

                voiceChatButton.button.OnClick.RemoveAllListeners();

                foreach (string key in spawnedNPCObject.Keys)
                {
                    spawnedNPCObject[key].GetComponent<NPCController>().SetVoiceChatState(false);
                }
                break;
            case RoomType.Loundge:
                break;
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        //eventSyncronizer.DisconnectChat();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        string message = $"{EventMessageType.DISCONNECT}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);
        //eventSyncronizer.DisconnectChat();

        DataManager.Instance.SetOffline(NetworkManager.User.email);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);
        DataManager.Instance.DeleteLobbyUser(NetworkManager.User);
    }
    private void OnApplicationPause()
    {
        string message = $"{EventMessageType.DISCONNECT}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);
        //eventSyncronizer.DisconnectChat();

        DataManager.Instance.SetOffline(NetworkManager.User.email);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);
        DataManager.Instance.DeleteLobbyUser(NetworkManager.User);
    }
}
