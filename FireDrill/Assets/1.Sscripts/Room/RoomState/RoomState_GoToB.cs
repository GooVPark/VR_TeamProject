using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_GoToB : RoomState
{
    [Header("Room State")]
    public RoomState_InQuiz roomStateInQuiz;
    [Space(5)]

    [Header("Toast")]
    public Toast toast;
    [Space(5)]

    [Header("Event Area")]
    public EventArea eventArea;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        toast.gameObject.SetActive(true);
        megaphone.ChangeIconState(ButtonState.IconState.Off);
        voiceChat.ChangeIconState(ButtonState.IconState.Off);
    }

    public override void OnStateExit()
    {
        toast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        if(roomSceneManager.IsReady(eventArea.playerCount))
        {
            roomSceneManager.RoomState = roomStateInQuiz;
        }
    }
}
