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
    [Space(5)]

    [Header("Class Object")]
    public GameObject classObject;

    [Header("Quiz Object")]
    public GameObject quizObject;

    [Header("Quiz NPC")]
    public GameObject npcObject;
    public Animator npcAnimator;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        classObject.SetActive(false);
        quizObject.SetActive(true);
        eventArea.gameObject.SetActive(true);
        toast.gameObject.SetActive(true);

        npcObject.SetActive(true);
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
