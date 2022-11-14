using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class RoomEnterance : MonoBehaviour
{
    [SerializeField] public int roomNumber;

    [SerializeField] private BoxCollider enteranceCollider;
    [SerializeField] private string targetTag;

    [SerializeField] private GameObject cantJoinRoomToast;
    [SerializeField] private ButtonInteractor joinRoomButton;
    [SerializeField] private GameObject joinRoomError;

    [SerializeField] private TMP_Text roomInfo;

    [SerializeField] private LoundgeSceneManager loundgeSceneManager;
    public bool isStarted = false;
    public GameObject interactionArea;

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
        //if (other.CompareTag("NetworkPlayerRoom"))
        //{

        //    if (other.GetComponentInParent<PhotonView>().IsMine)
        //    {
        //        joinRoomButton.gameObject.SetActive(false);
        //        joinRoomError.gameObject.SetActive(true);
        //    }
        //}
        //else if(other.CompareTag("NetworkPlayer"))
        //{
        //    joinRoomError.gameObject.SetActive(false);
        //    joinRoomButton.gameObject.SetActive(true);
        //}
        if(other.CompareTag(targetTag))
        {
            if(isStarted)
            {
                joinRoomButton.gameObject.SetActive(false);
                joinRoomError.SetActive(false);
                cantJoinRoomToast.SetActive(true);
                return;
            }
            else
            {
                cantJoinRoomToast.SetActive(false);
            }
            if(NetworkManager.Instance.onVoiceChat)
            {
                joinRoomButton.gameObject.SetActive(false);
                joinRoomError.gameObject.SetActive(true);
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
            cantJoinRoomToast.SetActive(false);
        }
    }

    public void JoinRoom()
    {
        if(DataManager.Instance.GetRoomProgressState(roomNumber))
        {
            joinRoomError.gameObject.SetActive(false);
            joinRoomButton.gameObject.SetActive(false);
            return;
        }
        loundgeSceneManager.JoinRoom(roomNumber);

    }

}
