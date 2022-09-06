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
    public static RoomSceneManager Instance;

    [SerializeField] private GameObject PDFViewr;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button prevPage;

    [Header("Lecture's Controller")]
    [SerializeField] private Button[] testButtons;
    [SerializeField] private ScoreUI[] scoureRows;

    private float elapsedTime = 1f;
    private float interval = 1f;

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
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, NetworkManager.RoomNumber);
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

    private void OnApplicationQuit()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        DataManager.Instance.UpdateRoomPlayerCount(NetworkManager.RoomNumber, playerCount - 1);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 0);
    }

}
