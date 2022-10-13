using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoomButton : MonoBehaviour
{
    public delegate void ButtonEvent(int roomNumber);
    public ButtonEvent onClick;

    public void OnSelect()
    {
        
    }
}
