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

        NetworkManager.Instance.voiceChatDisabled = false;
        NetworkManager.Instance.textChatDisabled = false;

        voiceChat.UpdateState(ButtonState.Deactivate);
        voiceChat.button.onClick += roomSceneManager.ToggleVoiceChat;

        textChat.UpdateState(ButtonState.Deactivate);
        textChat.button.onClick += roomSceneManager.ToggleTextChat;

        string message = $"{EventMessageType.LAMPUPDATE}";
        SendEventMessage(message);

        DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 0);

        if (user.userType == UserType.Lecture)
        {
            NetworkManager.Instance.megaphoneDisabled = false;

            message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
            SendEventMessage(message);

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
