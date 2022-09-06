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
        NetworkManager.Instance.roomType = NetworkManager.RoomType.Room;

        if(photonView.IsMine) DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public void PhaseShift(int progress)
    {
        DataManager.Instance.UpdateRoomProgress(roomNumber, progress);
    }

    private void OnApplicationQuit()
    {
        if (photonView.IsMine) DataManager.Instance.UpdateRoomPlayerCount(roomNumber, PhotonNetwork.CurrentRoom.PlayerCount);
    }


}
