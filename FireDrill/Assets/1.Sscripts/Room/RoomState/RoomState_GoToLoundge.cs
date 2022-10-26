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
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 0)
        {
            DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 0);
            DataManager.Instance.UpdateRoomState(roomSceneManager.roomNumber, false);

            string message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
            SendEventMessage(message);
        }
        PhotonNetwork.LeaveRoom();
    }
}
