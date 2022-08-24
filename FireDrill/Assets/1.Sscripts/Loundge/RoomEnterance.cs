using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnterance : MonoBehaviour
{
    public delegate void RoomEnteranceDelegate();
    public RoomEnteranceDelegate onShowRoomPanel;

    [SerializeField] private BoxCollider enteranceCollider;
    [SerializeField] private string targetTag;

    private void Awake()
    {
        enteranceCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(targetTag))
        {
            onShowRoomPanel?.Invoke();
        }
    }
}
