using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomEnterance : MonoBehaviour
{
    [SerializeField] private int roomNumber;

    [SerializeField] private BoxCollider enteranceCollider;
    [SerializeField] private string targetTag;

    [SerializeField] private ButtonInteractor joinRoomButton;

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(targetTag))
        {
            joinRoomButton.gameObject.SetActive(true);
            joinRoomButton.onClick += JoinRoom;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(targetTag))
        {
            joinRoomButton.onClick -= JoinRoom;
            joinRoomButton.gameObject.SetActive(false);
        }
    }

    public void JoinRoom()
    {
        if (!loundgeSceneManager.spawnedNPC[NetworkManager.User.email].onVoiceChat)
        {
            loundgeSceneManager.JoinRoom(roomNumber);
        }
        else
        {
            loundgeSceneManager.VoiceChatToRoomErrorToast();
        }
    }
}
