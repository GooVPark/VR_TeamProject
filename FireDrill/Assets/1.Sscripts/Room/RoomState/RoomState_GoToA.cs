using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �л����� ��� ������
/// A �������� �̵� �˸�
/// 
/// ����, �ý�Ʈ ê ��밡��
/// Ȯ����, ä���� ��� �Ұ���
/// 
/// �����Ȳ
/// ����ǥ�� O ȭ��ǥǥ�� X
/// �ȳ� �޼��� O
/// </summary>
public class RoomState_GoToA : RoomState
{
    [Header("Room State")]
    public RoomState_InClass roomStateInClass;
    [Space(5)]

    [Header("Event Area")]
    public EventArea eventArea;
    [Space(5)]

    [Header("Toast")]
    public Toast toast;
    [Space(5)]

    [Header("Class Object")]
    public GameObject classObject;
    public UnityEngine.UI.Image image;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        eventArea.gameObject.SetActive(true);
        classObject.SetActive(true);

        toast.gameObject.SetActive(true);

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            image.raycastTarget = true;
            DataManager.Instance.UpdateRoomState(roomSceneManager.roomNumber, true);
            string message = $"{EventMessageType.UPDATEROOMSTATE}_{roomSceneManager.roomNumber}";
            roomSceneManager.SendEventMessage(message);
        }
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
            roomSceneManager.RoomState = roomStateInClass;
        }
    }
}
