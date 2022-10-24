using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class RoomEnterance : MonoBehaviour
{
    [SerializeField] private int roomNumber;

    [SerializeField] private BoxCollider enteranceCollider;
    [SerializeField] private string targetTag;

    [SerializeField] private ButtonInteractor joinRoomButton;
    [SerializeField] private GameObject joinRoomError;

    [SerializeField] private TMP_Text roomInfo;

    [SerializeField] private LoundgeSceneManager loundgeSceneManager;

    private void Awake()
    {
        enteranceCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //roomInfo.text = $"참여 인원 ({NetworkManager.Instance.GetPlayerCount(roomNumber)} / 16)";
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (other.GetComponentInParent<PhotonView>() != null)
            {
                if (NetworkManager.Instance.onVoiceChat && other.GetComponentInParent<PhotonView>().IsMine)
                {
                    joinRoomButton.gameObject.SetActive(false);
                    joinRoomError.gameObject.SetActive(true);
                }
            }
            else
            {
                joinRoomError.gameObject.SetActive(false);
                joinRoomButton.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(targetTag))
        {
            joinRoomError.gameObject.SetActive(false);
            joinRoomButton.gameObject.SetActive(false);
        }
    }

    public void JoinRoom()
    {

        loundgeSceneManager.JoinRoom(roomNumber);

    }
}
