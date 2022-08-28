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

    private void Awake()
    {
        enteranceCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(targetTag))
        {

        }
    }
}
