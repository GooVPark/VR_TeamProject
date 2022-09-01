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
    }
}
