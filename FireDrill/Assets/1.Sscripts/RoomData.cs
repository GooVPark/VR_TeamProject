using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    private TextMeshProUGUI roomInfoText;
    private RoomInfo roomInfo;

    public void SetRoomInfo(RoomInfo room, GameObject obj)
    {
        roomInfo = room;
        roomInfoText = obj.GetComponentInChildren<TextMeshProUGUI>();
        roomInfoText.text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
    }

}
