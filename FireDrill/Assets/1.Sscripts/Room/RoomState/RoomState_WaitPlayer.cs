using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 룸에 처음 들어온 상태
/// 보이스, 택스트쳇 사용가능
/// 확성기, 채점판 사용 불가능
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
