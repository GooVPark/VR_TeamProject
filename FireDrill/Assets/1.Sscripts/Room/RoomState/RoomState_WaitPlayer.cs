using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �뿡 ó�� ���� ����
/// ���̽�, �ý�Ʈ�� ��밡��
/// Ȯ����, ä���� ��� �Ұ���
/// </summary>
public class RoomState_WaitPlayer : RoomState
{
    public RoomState_GoToA roomStateGoToA;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        voiceChat.ChangeIconState(ButtonState.IconState.Off);
        voiceChat.onButtonEvent += roomSceneManager.ToggleVoiceChat;

        textChat.ChangeIconState(ButtonState.IconState.Off);
        textChat.onButtonEvent += roomSceneManager.ToggleTextChat;
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        if(roomSceneManager.IsReady())
        {
            roomSceneManager.RoomState = roomStateGoToA;
        }
    }
}
