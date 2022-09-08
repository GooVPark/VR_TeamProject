using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomState_ClassEnd : RoomState
{
    [Header("Room State")]
    public RoomState_GoToB roomStateGoToB;
    [Space(5)]

    [Header("Toast")]
    public Toast classEndToast;
    [Space(5)]

    private float elapsedTime = 0f;
    [SerializeField] private float waitingTime = 2f;
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        classEndToast.Activate(2f);
    }

    public override void OnStateExit()
    {
        classEndToast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > waitingTime + 1)
        {
            roomSceneManager.RoomState = roomStateGoToB;
        }
    }
}
