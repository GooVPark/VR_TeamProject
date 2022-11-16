using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_SelectMRPlayer : RoomState
{
    [Header("Room State")]
    public RoomState_EndQuiz roomStateEndQuiz;
    public RoomState_GoToC roomStateGoToC;
    [Space(5)]

    [Header("Toast")]
    public Toast studentToast;
    public Toast lectureToast;
    public ToastTwoButtons lectureSelectToast;

    [Header("Players")]
    public Transform playerTransform;

    private NetworkPlayer targetPlayer;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

       
        if (user.userType == UserType.Lecture)
        {
            lectureToast.gameObject.SetActive(true);

            foreach (Transform playerTransform in playerTransform)
            {
                NetworkPlayer player = playerTransform.GetComponent<NetworkPlayer>();
                player.onPlayerSelectEvent = null;
                player.onPlayerSelectEvent += ShowSelectPlayerToast;
                player.isHoverActivated = true;
            }

        }
        else if(user.userType == UserType.Student)
        {
            studentToast.gameObject.SetActive(true);
        }
    }

    public override void OnStateExit()
    {
        foreach (Transform playerTransform in playerTransform)
        {
            NetworkPlayer player = playerTransform.GetComponent<NetworkPlayer>();
            player.onPlayerSelectEvent = null;
            player.isHoverActivated = false;
        }

        switch (user.userType)
        {
            case UserType.Lecture:
                lectureToast.gameObject.SetActive(false);
                lectureSelectToast.gameObject.SetActive(false);
                break;
            case UserType.Student:
                studentToast.gameObject.SetActive(false);
                break;
        }
        base.OnStateExit();
    }

    public void ShowSelectPlayerToast(NetworkPlayer player)
    {
        lectureToast.gameObject.SetActive(false);
        lectureSelectToast.gameObject.SetActive(true);
        lectureSelectToast.message.text = $"{player.UserName}";
        targetPlayer = player;
    }

    public void Select()
    {
        targetPlayer.OnSelectedMRPlayer();

        photonView.RPC(nameof(SetRoomState), RpcTarget.All);
    }
    public void Cancel()
    {
        targetPlayer = null;
        lectureToast.gameObject.SetActive(true);
        lectureSelectToast.gameObject.SetActive(false);
    }

    [PunRPC]
    private void SetRoomState()
    {
        roomSceneManager.RoomState = roomStateGoToC;
    }
}
