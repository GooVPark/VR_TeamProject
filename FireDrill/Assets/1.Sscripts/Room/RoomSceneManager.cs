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

    [SerializeField] private Button nextPage;
    [SerializeField] private Button prevPage;

    [Header("Lecture's Controller")]
    [SerializeField] private Button[] testButtons;

    [SerializeField] private GameObject scoreBoardUI;
    [SerializeField] private ScoreUI[] scoureRows;
    [SerializeField] private SimplePDFViwer pdfViwer;

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
    public bool isStarted = false;

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

    //룸에서 단계를 진행하기 위해 필요한 사람 수를 체크함
    //강의 시작 이후 부터는 현재 플레이어의 수가 필요한 사람 수로 바뀜. 기본은 16명
    public bool IsReady()//방에 사람이 16명이 다 찼을경우 단계를 진행시킴
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            return PhotonNetwork.CurrentRoom.PlayerCount >= requiredPlayer;
        }
        return false;
    }
    public bool IsReady(int playerCount)//필요한 인원수가 가변적으로 변하는 상황에 사용
    {
        if(isStarted)
        {
            requiredPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        return playerCount >= requiredPlayer;
    }

    public override void OnJoinedRoom()
    {
        NetworkManager.Instance.SetRoomNumber(roomNumber);
        NetworkManager.Instance.roomType = RoomType.Room;
        SetIdleMode(IdleMode.STAND);

        Initialize();

        origin.position = SpawnPlayer(spawnPivot.position);
        localRecoder.TransmitEnabled = false;
        NetworkManager.Instance.roomType = RoomType.Room;
        NetworkManager.Instance.onTextChat = false;
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            forceExitButton.SetActive(true);
        }
        RoomState = roomStateWaitPlayer;

        if (photonView.IsMine) DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
        roomData = DataManager.Instance.GetRoomData()[roomNumber];
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, roomNumber);
        DataManager.Instance.InitializeQuizScore(NetworkManager.User.email);
        DataManager.Instance.InsertRoomUser(NetworkManager.User);

        requiredPlayer = roomData.requirePlayerCount;
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
        StartCoroutine(ExcuteLeaveRoom());   
    }

    private IEnumerator ExcuteLeaveRoom()
    {
        Debug.Log("Leave Room");
        DataManager.Instance.UpdateRoomPlayerCount(NetworkManager.RoomNumber, PhotonNetwork.CurrentRoom.PlayerCount - 1);
        DataManager.Instance.InsertLobbyUser(NetworkManager.User);

        yield return new WaitForSeconds(1f);

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
        while(true)
        {
            if(isEventServerConnected)
            {
                DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
                DataManager.Instance.InitializeQuizScore(NetworkManager.User.email);

                string message = $"{EventMessageType.NOTICE}_{NoticeEventType.JOIN}_{roomNumber}_{NetworkManager.User.email}";
                eventMessage?.Invoke(message);
                break;
            }
            yield return null;
        }
    }

    public override void OnLeftRoom()
    {
        RoomData roomData = DataManager.Instance.GetRoomData(roomNumber);
        int playerCount = roomData.currentPlayerCount - 1;

        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 999);
        DataManager.Instance.DeleteRoomUser(NetworkManager.User);

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
    }



    public override void OnDisconnected(DisconnectCause cause)
    {

    }

    public override void OnConnectedToMaster()
    {
        LoadingSceneController.LoadScene("Loundge (BG3)");
    }

    public override void OnJoinedLobby()
    {
       
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<NetworkPlayer>().InvokeProperties();
        }
    }

    public void NextPage()
    {
        photonView.RPC(nameof(NextPageRPC), RpcTarget.All);
    }
    [PunRPC]
    public void NextPageRPC()
    {
        pdfViwer.NextPage();
    }

    public void PrevPage()
    {
        photonView.RPC(nameof(PrevPageRPC), RpcTarget.All);
    }
    [PunRPC]
    public void PrevPageRPC()
    {
        pdfViwer.PrevPage();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(newMasterClient.IsMasterClient)
        {
            DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    private void OnApplicationQuit()
    {
        RoomData roomData = DataManager.Instance.GetRoomData(roomNumber);
        int playerCount = roomData.currentPlayerCount - 1;

        DataManager.Instance.UpdateRoomPlayerCount(roomNumber, playerCount);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 999);
        DataManager.Instance.DeleteRoomUser(NetworkManager.User);
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

            message = $"{EventMessageType.FORCEEXIT}";
            SendEventMessage(message);

        }
    }
    //private void OnApplicationPause()
    //{
    //    RoomData roomData = DataManager.Instance.GetRoomData(roomNumber);
    //    int playerCount = roomData.currentPlayerCount - 1;

    //    DataManager.Instance.UpdateRoomPlayerCount(roomNumber, playerCount);
    //    DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 999);
    //    DataManager.Instance.DeleteRoomUser(NetworkManager.User);
    //    DataManager.Instance.SetOffline(NetworkManager.User.email);

    //    string message = $"{EventMessageType.NOTICE}_{NoticeEventType.DISCONNECT}_{roomNumber}_{NetworkManager.User.email}";
    //    eventMessage?.Invoke(message);

    //    if (NetworkManager.User.userType == UserType.Lecture)
    //    {
    //        DataManager.Instance.UpdateRoomProgress(roomNumber, 0);
    //        DataManager.Instance.UpdateRoomState(roomNumber, false);

    //        message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomNumber}";
    //        SendEventMessage(message);

    //        message = $"{EventMessageType.UPDATEROOMSTATE}_{roomNumber}";
    //        SendEventMessage(message);

    //        message = $"{EventMessageType.FORCEEXIT}";
    //        SendEventMessage(message);
    //    }

    //    eventSyncronizer.Disconnect();
    //}
}
