using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EventArea : MonoBehaviour
{
    public int playerCount = 0;
    public Dictionary<int, bool> targetObjects = new Dictionary<int, bool>();

    private void OnEnable()
    {
        playerCount = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("NetworkPlayerRoom"))
        {
            int key = other.gameObject.GetInstanceID();
            if(!targetObjects.ContainsKey(key))
            {
                targetObjects.Add(key, true);
            }
            else
            {
                targetObjects[key] = true;
            }

            int count = 0;
            foreach(int instanceID in targetObjects.Keys)
            {
                if(targetObjects[instanceID])
                {
                    count++;
                }
            }

            playerCount = count;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("NetworkPlayerRoom"))
        {
            int key = other.gameObject.GetInstanceID();
            if (targetObjects.ContainsKey(key))
            {
                targetObjects[key] = false;
            }

            int count = 0;
            foreach (int instanceID in targetObjects.Keys)
            {
                if (targetObjects[instanceID])
                {
                    count++;
                }
            }

            playerCount = count;
        }
    }
}
