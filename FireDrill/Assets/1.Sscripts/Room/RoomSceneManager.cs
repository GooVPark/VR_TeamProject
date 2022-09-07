using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using TMPro;

public class RoomSceneManager : GameManager
{
    #region Events

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

    public static RoomSceneManager Instance;

    [SerializeField] private GameObject PDFViewr;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button prevPage;

    [Header("Lecture's Controller")]
    [SerializeField] private Button[] testButtons;

    [SerializeField] private GameObject scoreBoardUI;
    [SerializeField] private ScoreUI[] scoureRows;

    private float elapsedTime = 1f;
    private float interval = 1f;

    [SerializeField] private RoomData roomData;

    [Header("Event Area")]
    [SerializeField] private EventArea eventAreaA;
    [SerializeField] private EventArea eventAreaB;
    [SerializeField] private EventArea eventAreaC;
    [Space(5)]

    [Header("Toast")]
    [SerializeField] private ToastTypeAndMessage toasts;
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
        Initialize();
        SpawnPlayer();
        NetworkManager.Instance.roomType = NetworkManager.RoomType.Room;

        if(photonView.IsMine) DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
        roomData = DataManager.Instance.GetRoomData()[roomNumber-1];
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, NetworkManager.RoomNumber);
    }

    private void Update()
    {
        RoomState.OnUpdate();
    }

    public void PhaseShift(int progress)
    {
        DataManager.Instance.UpdateRoomProgress(roomNumber, progress);
    }

    private List<User> GetUsersInRoom(int number)
    {
        return DataManager.Instance.GetUsersListInRoom(number);
    }

    public void ShowScoreBoard()
    {
        if (scoreBoardUI.activeSelf)
        {
            scoreBoardUI.SetActive(false);
            return;
        }

        scoreBoardUI.SetActive(true);

        List<User> users = GetUsersInRoom(NetworkManager.RoomNumber);

        for(int i = 0; i < users.Count; i++)
        {
            if (users[i].userType == UserType.Lecture) continue;

            scoureRows[i].gameObject.SetActive(true);
            scoureRows[i].UpdateScore(users[i]);
        }
        for(int i = users.Count; i < scoureRows.Length; i++)
        {
            scoureRows[i].gameObject.SetActive(false);
        }
    }

    public bool IsReady()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount >= roomData.requirePlayerCount;
    }
    public bool IsReady(int playerCount)
    {
        return playerCount >= roomData.requirePlayerCount;
    }

    private void OnApplicationQuit()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        DataManager.Instance.UpdateRoomPlayerCount(NetworkManager.RoomNumber, playerCount - 1);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 0);
    }

}
