using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea : MonoBehaviour
{
    [SerializeField] private int playerCount = 0;
    [SerializeField] private int requiredCount = 2;

    public delegate void ToastEvent();
    public ToastEvent onToastEvent;

    private void Start()
    {
        ToastEventHandler toastEventHandler = gameObject.GetComponent<ToastEventHandler>();
        if (toastEventHandler != null) onToastEvent += toastEventHandler.OnToastEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NetworkPlayer"))
        {
            playerCount++;
            onToastEvent();

            if(playerCount >= requiredCount)
            {

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("NetworkPlayer"))
        {
            playerCount--;
        }
    }
}
