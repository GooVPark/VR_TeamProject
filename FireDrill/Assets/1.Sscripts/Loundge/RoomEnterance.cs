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

    [SerializeField] private Button joinRoomButton;

    [SerializeField] private TMP_Text roomInfo;

    private void Awake()
    {
        enteranceCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        roomInfo.text = $"참여 인원 ({NetworkManager.Instance.GetPlayerCount(roomNumber)} / 11)";
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(targetTag))
        {
            joinRoomButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(targetTag))
        {
            joinRoomButton.gameObject.SetActive(false);
        }
    }
}
