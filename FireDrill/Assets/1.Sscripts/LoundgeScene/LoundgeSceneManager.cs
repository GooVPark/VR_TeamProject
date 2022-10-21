using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
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
    public NPCManager npcManager;
    public Transform NPCTransforms;
    public SpawnPosition spawnPosition;
    private Dictionary<string, User> usersByEmail = new Dictionary<string, User>();
    public Dictionary<string, LoundgeUser> spawnedNPC = new Dictionary<string, LoundgeUser>();
    public Dictionary<string, GameObject> spawnedNPCObject = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject npcPrefab;

    private bool isOnline = false;
    public bool isEventServerConnected = false;

    [SerializeField] private GameObject voiceChatErrorToast;

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
        Initialize();
        NetworkManager.Instance.roomType = RoomType.Loundge;

        InsertUserData();

        //NetworkManager.Instance.PullRoomList();
        LoadFirstPage();
        UpdateProgressBoard();

        initializer = StartCoroutine(Initializer());
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {

    }

    private Coroutine initializer;
    private IEnumerator Initializer()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.3f);
        while (!isOnline || !isEventServerConnected)
        {
            Debug.Log("Finding User");
            isOnline = DataManager.Instance.FindLobbyUser(NetworkManager.User);
            yield return waitForSeconds;
        }

        Debug.Log("Instantiate User Object");

        string message = $"{EventMessageType.SPAWN}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);

        for (int i = 0; i < playerCountsText.Length; i++)
        {
            UpdateRoomPlayerCount(i);
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

                if (loundgeUser.email.Equals(NetworkManager.User.email))
                {
                    npcObject.SetActive(false);
                }
            }
        }
    }

    public void JoinVoiceChatRoom(string userID)
    {
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

        spawnedNPC.Remove(email);
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
            progressUIs[i].sprite = progressImages[roomDatas[i].progress-1];
        }
    }
    public void UpdateLobbyPlayerCount()
    {
        playerCountText.text = DataManager.Instance.GetLoundgeUsers().Count.ToString();
    }
    public void UpdateRoomPlayerCount(int roomNumber)
    {
        RoomData roomData = DataManager.Instance.GetRoomData(roomNumber);

        int playerCount = roomData.currentPlayerCount;
        int maxPlayerCount = roomData.maxPlayerCount;

        playerCountsText[roomNumber].text = $"{playerCount}/{maxPlayerCount}";
    }


    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Loundge Maanger: Joined Lobby");
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
        textChatManager.DisconnectChat();
        eventSyncronizer.DisconnectChat();
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
        Debug.Log("Loundge: OnJoinedRoom");

        switch (NetworkManager.Instance.roomType)
        {
            case RoomType.Room:
                break;
            case RoomType.VoiceRoom:

                spawnedNPCObject[voiceManager.sender.email].SetActive(false);
                spawnedNPCObject[voiceManager.reciever.email].SetActive(false);

                voiceChatButton.button.onClick += voiceManager.DisconnectVoiceChat;
                voiceChatButton.UpdateState(ButtonState.Activate);

                foreach(string key in spawnedNPCObject.Keys)
                {
                    spawnedNPCObject[key].GetComponent<NPCController>().SetVoiceChatState(false);
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
                Debug.Log("OnLeftRoom");

                if(voiceManager.sender.email == NetworkManager.User.email)
                {
                    spawnedNPCObject[voiceManager.reciever.email].SetActive(true);
                }
                if(voiceManager.reciever.email == NetworkManager.User.email)
                {
                    spawnedNPCObject[voiceManager.sender.email].SetActive(true);
                }

                NetworkManager.Instance.roomType = RoomType.Loundge;
                voiceChatButton.UpdateState(ButtonState.Disable);
                voiceChatButton.button.onClick -= voiceManager.DisconnectVoiceChat;

                foreach (string key in spawnedNPCObject.Keys)
                {
                    spawnedNPCObject[key].GetComponent<NPCController>().SetVoiceChatState(true);
                }

                Debug.Log(PhotonNetwork.NetworkClientState);
                break;
            case RoomType.Loundge:
                break;
        }
    }

    public override void OnLeftLobby()
    {

    }
    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if (newPlayer != PhotonNetwork.LocalPlayer)
    //    {
    //        ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NPCController>().InvokeProperties();
    //    }
    //    //List<User> users = DataManager.Instance.GetUsersInRoom(roomNumber);

    //    //Debug.Log(users.Count);

    //    //foreach (User user in users)
    //    //{
    //    //    if (!usersByEmail.ContainsKey(user.email))
    //    //    {
    //    //        usersByEmail.Add(user.email, user);
    //    //        npcManager.SpawnNPC(user);
    //    //    }
    //    //}
    //}
    #endregion

    private void OnApplicationQuit()
    {
        string message = $"{EventMessageType.DISCONNECT}_{NetworkManager.User.email}";
        eventMesage?.Invoke(message);

        DataManager.Instance.DeleteLobbyUser(NetworkManager.User);
    }
}
