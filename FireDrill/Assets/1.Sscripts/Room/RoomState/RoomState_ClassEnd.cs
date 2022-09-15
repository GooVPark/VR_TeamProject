using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 강사가 강의 종료를 누른 직후의 상태
/// 
/// 2초정도 대기하고 다음 상태로 넘어감
/// </summary>
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

        if(user.userType == UserType.Lecture)
        {
            DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 1);
        }
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
