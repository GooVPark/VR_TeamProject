using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_InQuiz : RoomState
{
    [Header("Room State")]
    public RoomState_StartQuiz roomStateStartQuiz;
    [Space(5)]

    [Header("Toast")]
    public Toast lectureToast;
    public Toast studnetToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        roomSceneManager.onRoomStateEvent += OnQuizStart;
    }

    public override void OnStateExit()
    {
        roomSceneManager.onRoomStateEvent -= OnQuizStart;
        base.OnStateExit();
    }

    public override void OnUpdate()
    {

    }

    public void OnQuizStart()
    {
        roomSceneManager.RoomState = roomStateStartQuiz;
    }
}
