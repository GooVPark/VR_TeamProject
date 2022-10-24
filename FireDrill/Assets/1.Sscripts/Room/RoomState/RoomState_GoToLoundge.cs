using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_GoToLoundge : RoomState
{
    [Header("Toast")]
    public Toast goToLoundgeToast;
    [Space(5)]

    [Header("EventArea")]
    public GameObject eventArea;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        goToLoundgeToast.gameObject.SetActive(true);
        eventArea.SetActive(true);
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public void LeaveRoom()
    {
        DataManager.Instance.UpdateRoomPlayerCount(NetworkManager.RoomNumber, PhotonNetwork.CurrentRoom.PlayerCount - 1);
        PhotonNetwork.LeaveRoom();
    }
}
