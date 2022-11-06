using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAreaMR : EventArea
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NetworkPlayerRoom"))
        {
            if(other.GetComponentInParent<NetworkPlayer>().HasExtinguisher)
            {
                playerCount++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NetworkPlayerRoom"))
        {
            if (!other.GetComponentInParent<NetworkPlayer>().HasExtinguisher)
            {
                playerCount--;
            }
        }
    }
}
