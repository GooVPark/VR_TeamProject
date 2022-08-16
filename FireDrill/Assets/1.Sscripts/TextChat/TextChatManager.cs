using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class TextChatManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        InputManager.SecondaryReaded += GetSecondaryValue;
    }

    private void GetSecondaryValue(bool value)
    {
        if(value)
        {
            ViewManager.Show<TextChatView>(true);
        }
        else
        {
            ViewManager.ShowLast();
        }
    }
}
