using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// �뿡 ó�� ���� ����
/// ���̽�, �ý�Ʈ�� ��밡��
/// Ȯ����, ä���� ��� �Ұ���
/// </summary>
public class RoomState_WaitPlayer : RoomState
{
    public RoomState_GoToA roomStateGoToA;

    public ToastOneButton forceStartToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        voiceChat.ChangeIconState(ButtonState.IconState.Off);
        voiceChat.onButtonEvent += roomSceneManager.ToggleVoiceChat;

        textChat.ChangeIconState(ButtonState.IconState.Off);
        textChat.onButtonEvent += roomSceneManager.ToggleTextChat;

        if(user.userType == UserType.Lecture)
        {
            forceStartToast.gameObject.SetActive(true);
            roomSceneManager.onRoomStateEvent += ForceStart;
        }
    }

    public override void OnStateExit()
    {
        forceStartToast.gameObject.SetActive(false);
        roomSceneManager.onRoomStateEvent -= ForceStart;
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        if(roomSceneManager.IsReady())
        {
            roomSceneManager.RoomState = roomStateGoToA;
        }
    }

    public void ForceStart()
    {
        photonView.RPC(nameof(ForceStartRPC), RpcTarget.All);
    }

    [PunRPC]
    public void ForceStartRPC()
    {
        roomSceneManager.requiredPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
    }
}
