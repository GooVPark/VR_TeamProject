using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea : MonoBehaviour
{
    public int playerCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("NetworkPlayer"))
        {
            playerCount++;
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
