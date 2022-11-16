using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_GoToC : RoomState
{
    [Header("Room State")]
    public RoomState_InTraning roomStateInTraning;
    [Space(5)]

    [Header("Toasts")]
    public Toast toast;
    [Space(5)]

    [Header("Event Area")]
    public EventArea eventAreaMR;
    public GameObject eventAreaMRMarker;
    public EventArea eventArea;
    public GameObject eventAreaMarker;
    [Space(5)]

    [Header("QuizObject")]
    public GameObject quizObject;

    [Header("Training Object")]
    public GameObject trainingObject;


    public override void OnStateEnter()
    {
        base.OnStateEnter();
        quizObject.SetActive(false);
        trainingObject.SetActive(true);

        eventAreaMR.gameObject.SetActive(true);
        eventArea.gameObject.SetActive(true);

        if (user.hasExtingisher)
        {
            eventAreaMarker.gameObject.SetActive(false);
            eventAreaMRMarker.gameObject.SetActive(true);
        }
        else
        {
            eventAreaMRMarker.gameObject.SetActive(false);
            eventAreaMarker.gameObject.SetActive(true);
        }

        if(roomSceneManager.isStarted)
        {
            roomSceneManager.requiredPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        toast.gameObject.SetActive(true);
    }

    public override void OnStateExit()
    {
        toast.gameObject.SetActive(false);

        eventAreaMarker.gameObject.SetActive(false);
        eventAreaMR.gameObject.SetActive(false);
        eventAreaMarker.gameObject.SetActive(false);
        eventArea.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        if(eventAreaMR.playerCount >= 1 && roomSceneManager.IsReady(eventArea.playerCount+1))
        {
            roomSceneManager.RoomState = roomStateInTraning;
        }
    }
}
