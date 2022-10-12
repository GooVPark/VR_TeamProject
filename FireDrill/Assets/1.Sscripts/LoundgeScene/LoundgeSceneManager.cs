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

    public EventSyncronizer eventSyncronizer;
    public NPCManager npcManager;
    public SpawnPosition spawnPosition;
    private Dictionary<string, User> usersByEmail = new Dictionary<string, User>();
    private Dictionary<string, LoundgeUser> spawnedNPC = new Dictionary<string, LoundgeUser>();

    [SerializeField] private GameObject npcPrefab;

    private bool isOnline = false;
    public bool isEventServerConnected = false;

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
        NetworkManager.Instance.roomType = NetworkManager.RoomType.Loundge;

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
        refreshTimeCount += Time.fixedDeltaTime;
        if(refreshTimeCount > refreshTime)
        {
            refreshTimeCount = 0;
            RefreshNoticeBoard();
        }

        progressBoardElapsedTime += Time.fixedDeltaTime;
        if(progressBoardElapsedTime > progressBoardUpdateInterval)
        {
            progressBoardElapsedTime = 0f;
            UpdateProgressBoard();
        }

        UpdateLobbyPlayerCount();
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

                if (loundgeUser.email.Equals(NetworkManager.User.email))
                {
                    continue;
                }

                Vector2 randomValue = Random.insideUnitCircle * 3f;
                Vector3 spawnPosition = this.spawnPosition.transform.position + new Vector3(randomValue.x, 0, randomValue.y);

                GameObject npcObject = Instantiate(npcPrefab, spawnPosition, Quaternion.identity, playerTransforms);
                NPCController npc = npcObject.GetComponent<NPCController>();
                npc.Initialize(loundgeUser);
                npc.eventMessage += eventSyncronizer.OnSendMessage;
            }
        }
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

    private float progressBoardElapsedTime = 0f;
    private float progressBoardUpdateInterval = 1f;

    private void UpdateProgressBoard()
    {
        roomDatas = DataManager.Instance.GetRoomData();

        for(int i = 0; i < roomDatas.Count; i++)
        {
            progresses[i].UpdateProgressUI(roomDatas[i]);
        }

        for(int i = roomDatas.Count; i < progresses.Count; i++)
        {
            progresses[i].UpdateProgressUI(null);
        }
    }
    private void UpdateLobbyPlayerCount()
    {
        playerCountText.text = (PhotonNetwork.CountOfPlayers - NetworkManager.Instance.GetPlayerCountOnRooms()).ToString();
    }



    #endregion

    #region Photon Callbacks

    public int roomNumber;
    private string roomName;
    private RoomOptions roomOptions;
    public void JoinRoom(int roomNumber)
    {
        Debug.Log("Join Room: " + roomNumber);
        textChatManager.DisconnectChat();
        NetworkManager.Instance.SetRoomNumber(roomNumber);
        roomName = roomNumber.ToString();

        //NetworkManager.Instance.roomType = NetworkManager.RoomType.Room;

        roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 0;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Loundge: OnJoinedRoom");
        //NetworkManager.Instance.SetRoomNumber(roomNumber);

        //Vector3 position = spawnPosition.GetPosition(PhotonNetwork.LocalPlayer.ActorNumber);

        //GameObject npcObject = PhotonNetwork.Instantiate("NPC", position, Quaternion.identity);
        //npcObject.GetComponent<NPCController>().Initialize(NetworkManager.User);
        //List<User> users = DataManager.Instance.GetUsersInRoom(roomNumber);

        //Debug.Log(users.Count);

        //foreach(User user in users)
        //{
        //    if(!usersByEmail.ContainsKey(user.email))
        //    {
        //        usersByEmail.Add(user.email, user);
        //        npcManager.SpawnNPC(user);
        //    }
        //}

        PhotonNetwork.LoadLevel("Room");
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
        DataManager.Instance.DeleteLobbyUser(NetworkManager.User);
    }
}
