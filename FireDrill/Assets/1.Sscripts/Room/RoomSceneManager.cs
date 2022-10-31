using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using TMPro;
using Paroxe.PdfRenderer;

public class RoomSceneManager : GameManager
{
    #region Events

    public delegate void EventMessage(string message);
    public static EventMessage eventMessage;

    public delegate void RoomStateSimpleEvent();
    public RoomStateSimpleEvent onRoomStateEvent;
    public void OnRoomState() => onRoomStateEvent?.Invoke();

    #endregion
    private RoomState roomState;
    public RoomState RoomState
    {
        get { return roomState; }
        set
        {
            if(roomState != null)
            {
                roomState.OnStateExit();
            }
            roomState = value;
            roomState.OnStateEnter();
        }
    }
    public RoomState_WaitPlayer roomStateWaitPlayer;

    public static RoomSceneManager Instance;

    [SerializeField] private PDFViewer pdfViewr;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button prevPage;

    [Header("Lecture's Controller")]
    [SerializeField] private Button[] testButtons;

    [SerializeField] private GameObject scoreBoardUI;
    [SerializeField] private ScoreUI[] scoureRows;

    private float elapsedTime = 1f;
    private float interval = 1f;

    [SerializeField] private RoomData roomData;
    public int requiredPlayer = 0;

    [Header("Event Area")]
    [SerializeField] private EventArea eventAreaA;
    [SerializeField] private EventArea eventAreaB;
    [SerializeField] private EventArea eventAreaC;
    [Space(5)]

    [Header("Toast")]
    [SerializeField] private ToastTypeAndMessage toasts;
    [SerializeField] private GameObject forceExitToast;
    [SerializeField] private GameObject forceExitButton;

    [SerializeField] private EventSyncronizerRoom eventSyncronizer;

    public Transform spawnPivot;
    public int roomNumber;

    public Transform origin;

    public bool socreOrderBy = false;

    private int currentProcess;
    public int CurrentProcess
    {
        get { return currentProcess; }
        set
        {
            currentProcess = value;
            DataManager.Instance.UpdateRoomProgress(roomNumber, currentProcess);
        }
    }

    public bool isEventServerConnected;

    private void Awake()
    {
        if (Instance == null)
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
        NetworkManager.Instance.SetRoomNumber(roomNumber);
        NetworkManager.Instance.roomType = RoomType.Room;

        SetIdleMode(NetworkManager.User.idleMode);
    }

    private void Update()
    {
        RoomState?.OnUpdate();
    }

    public void PhaseShift(int progress)
    {
        DataManager.Instance.UpdateRoomProgress(roomNumber, progress);
    }

    public List<User> GetUsersInRoom(int number)
    {
        return DataManager.Instance.GetUsersListInRoom(number);
    }

    public RoomData GetRoomData()
    {
        return roomData;
    }

    public void SendEventMessage(string message)
    {
        eventMessage?.Invoke(message);
    }

    public void ShowScoreBoard()
    {
        if (scoreBoardUI.activeSelf)
        {
            scoreBoardUI.SetActive(false);
            NetworkManager.Instance.onScoreBoard = false;
            return;
        }

        NetworkManager.Instance.onScoreBoard = true;
        scoreBoardUI.SetActive(true);

        //UpdateScoreBoard();
    }

    private void UpdateScoreBoard()
    {
        List<User> users = GetUsersInRoom(NetworkManager.RoomNumber);

        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].userType == UserType.Lecture) continue;

            scoureRows[i].gameObject.SetActive(true);
            scoureRows[i].UpdateScore(users[i]);
        }
        for (int i = users.Count; i < scoureRows.Length; i++)
        {
            scoureRows[i].gameObject.SetActive(false);
        }
    }

    public bool IsReady()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount >= requiredPlayer;
    }
    public bool IsReady(int playerCount)
    {
        return playerCount >= requiredPlayer;
    }

    public void NextPage()
    {
        pdfViewr.GoToNextPage();
    }

    public void PrevPage()
    {
        pdfViewr.GoToPreviousPage();
    }

    public override void OnJoinedRoom()
    {
        Initialize();
        origin.position = SpawnPlayer(spawnPivot.position);
        localRecoder.TransmitEnabled = false;
        NetworkManager.Instance.roomType = RoomType.Room;
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            forceExitButton.SetActive(true);
        }
        RoomState = roomStateWaitPlayer;

        if (photonView.IsMine) DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
        roomData = DataManager.Instance.GetRoomData()[roomNumber];
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);
        requiredPlayer = roomData.requirePlayerCount;

        StartCoroutine(JoinVoice());
    }

    public void ForceExit()
    {
        photonView.RPC(nameof(ForceExitRPC), RpcTarget.All);
    }

    [PunRPC]
    public void ForceExitRPC()
    {
        LeaveRoom();
    }
    public void LeaveRoom()
    {
        DataManager.Instance.UpdateRoomPlayerCount(NetworkManager.RoomNumber, PhotonNetwork.CurrentRoom.PlayerCount - 1);
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 0)
        {
            DataManager.Instance.UpdateRoomProgress(roomNumber, 0);
            DataManager.Instance.UpdateRoomState(roomNumber, false);

            string message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomNumber}";
            SendEventMessage(message);
        }
        PhotonNetwork.LeaveRoom();
    }

    public void ForceExitPopUp()
    {
        if(forceExitToast.activeSelf)
        {
            forceExitToast.SetActive(false);
        }
        else
        {
            forceExitToast.SetActive(true);
        }
    }

    private IEnumerator JoinVoice()
    {
        Debug.Log("Test Joined");
        while(true)
        {
            if(isEventServerConnected)
            {
                //PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], new byte[0]);
                //PhotonVoiceNetwork.Instance.PrimaryRecorder.InterestGroup = 0;

                DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
                DataManager.Instance.InitializeQuizScore(NetworkManager.User.email);

                string message = $"{EventMessageType.NOTICE}_{NoticeEventType.JOIN}_{roomNumber}_{NetworkManager.User.email}";
                eventMessage?.Invoke(message);

                Debug.Log("Test Join Server");

                break;
            }
            yield return null;
        }
        Debug.Log("Test Ended");
    }

    public override void OnLeftRoom()
    {
        RoomData roomData = DataManager.Instance.GetRoomData(roomNumber);
        int playerCount = roomData.currentPlayerCount - 1;

        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 999);
        DataManager.Instance.UpdateRoomPlayerCount(roomNumber, playerCount);

        string message = $"{EventMessageType.NOTICE}_{NoticeEventType.DISCONNECT}_{roomNumber}_{NetworkManager.User.email}";
        eventMessage?.Invoke(message);

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            DataManager.Instance.UpdateRoomProgress(roomNumber, 0);
            DataManager.Instance.UpdateRoomState(roomNumber, false);

            message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomNumber}";
            SendEventMessage(message);

            message = $"{EventMessageType.UPDATEROOMSTATE}_{roomNumber}";
            SendEventMessage(message);
        }

        //eventSyncronizer.Disconnect();
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.LoadLevel("Loundge");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
        }
    }

    private void OnApplicationQuit()
    {
        RoomData roomData = DataManager.Instance.GetRoomData(roomNumber);
        int playerCount = roomData.currentPlayerCount - 1;

        DataManager.Instance.UpdateRoomPlayerCount(roomNumber, playerCount);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 999);
        DataManager.Instance.SetOffline(NetworkManager.User.email);

        string message = $"{EventMessageType.NOTICE}_{NoticeEventType.DISCONNECT}_{roomNumber}_{NetworkManager.User.email}";
        eventMessage?.Invoke(message);

        if(NetworkManager.User.userType == UserType.Lecture)
        {
            DataManager.Instance.UpdateRoomProgress(roomNumber, 0);
            DataManager.Instance.UpdateRoomState(roomNumber, false);

            message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomNumber}";
            SendEventMessage(message);

            message = $"{EventMessageType.UPDATEROOMSTATE}_{roomNumber}";
            SendEventMessage(message);
        }

        //eventSyncronizer.Disconnect();
        PhotonNetwork.SendAllOutgoingCommands();
    }
}
