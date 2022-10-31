using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    public GameObject greenLamp;
    public GameObject yellowLamp;
    public GameObject redLamp;

    private GameObject currentLamp;
    public GameObject CurrentLamp
    {
        set
        {
            if(currentLamp != null)
            {
                currentLamp.SetActive(false);
            }

            currentLamp = value;
            currentLamp.SetActive(true);
        }
    }

    public void UpdateLampState(RoomData roomData)
    {
        if (!roomData.isStarted)
        {
            if (roomData.currentPlayerCount < 1)
            {
                CurrentLamp = greenLamp;
            }
            else
            {
                CurrentLamp = yellowLamp;
            }
        }
        else
        {
            CurrentLamp = redLamp;
        }
    }
}
