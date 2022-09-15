using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���簡 ���� ���Ḧ ���� ������ ����
/// 
/// 2������ ����ϰ� ���� ���·� �Ѿ
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
