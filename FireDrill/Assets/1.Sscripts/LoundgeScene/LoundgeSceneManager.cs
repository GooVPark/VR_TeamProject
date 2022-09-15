using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using TMPro;

public class LoundgeSceneManager : GameManager
{
    public static LoundgeSceneManager Instance;

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
        NetworkManager.Instance.PullRoomList();
        LoadFirstPage();
        UpdateProgressBoard();
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

    public override void OnJoinedLobby()
    {
        
    }

    #endregion
}
