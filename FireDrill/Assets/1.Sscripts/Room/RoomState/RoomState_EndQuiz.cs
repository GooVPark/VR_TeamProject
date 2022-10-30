using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_EndQuiz : RoomState
{
    [Header("Room State")]
    public RoomState_GoToC roomStateGoToC;
    public RoomState_SelectMRPlayer roomState_SelectMRPlayer;
    [Space(5)]

    [Header("Toast")]
    public Toast toast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 4);

            string message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
            SendEventMessage(message);

            toast.gameObject.SetActive(true);
            Invoke(nameof(SetState), 2f);
        }
    }

    public override void OnStateExit()
    {
        toast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public void SetState()
    {
        photonView.RPC(nameof(SetStateRPC), RpcTarget.All);
    }

    [PunRPC]
    public void SetStateRPC()
    {
        roomSceneManager.RoomState = roomStateGoToC;
    }
}
