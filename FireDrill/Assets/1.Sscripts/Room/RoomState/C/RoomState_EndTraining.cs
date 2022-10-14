using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_EndTraining : RoomState
{
    [Header("Toast")]
    public ToastOneButton lectureToast;
    public Toast studentToast;

    private float elapsedTime;
    private float time = 10f;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        switch (user.userType)
        {
            case UserType.Lecture:
                DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 6);
                DataManager.Instance.UpdateRoomState(roomSceneManager.roomNumber, false);

                string message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
                SendEventMessage(message);

                roomSceneManager.onRoomStateEvent += LeaveRoom;
                lectureToast.gameObject.SetActive(true);
                break;
            case UserType.Student:
                studentToast.gameObject.SetActive(true);
                break;
        }
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public void LeaveRoom()
    {
        DataManager.Instance.UpdateRoomPlayerCount(NetworkManager.RoomNumber, PhotonNetwork.CurrentRoom.PlayerCount - 1);
        DataManager.Instance.SetOffline(NetworkManager.User.email);
        DataManager.Instance.UpdateCurrentRoom(NetworkManager.User.email, 0);

        photonView.RPC(nameof(LeaveRoomRPC), RpcTarget.All);
    }

    [PunRPC]
    public void LeaveRoomRPC()
    {
        PhotonNetwork.LeaveRoom();
    }
}
