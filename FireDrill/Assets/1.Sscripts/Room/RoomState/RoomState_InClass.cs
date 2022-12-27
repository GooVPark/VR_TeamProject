using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// A������ ��� �л��� ����
/// ���簡 ���� ���� ��ư ������ ���� ���·� �Ѿ
/// 
/// ���̽�, �ý�Ʈ ê ��밡��
/// Ȯ����, ä���� ��� �Ұ���
/// 
/// ���� ��Ȳ
/// �˸� �޼��� O
/// ���翡 ���� ���� ��ȯ O
/// </summary>
public class RoomState_InClass : RoomState
{
    [Header("Room State")]
    public RoomState_Class roomStateClass;
    [Space(5)]

    [Header("Toast")]
    public Toast lectureToast;
    public Toast studentToast;
    public Toast disMicToast;
    private GameObject currentToast;
    [Space(5)]

    [Header("EventArea")]
    public EventArea eventArea;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        eventArea.gameObject.SetActive(false);

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            DataManager.Instance.UpdateRoomProgress(roomSceneManager.roomNumber, 1);

            string message = $"{EventMessageType.PROGRESS}_{ProgressEventType.UPDATE}_{roomSceneManager.roomNumber}";
            SendEventMessage(message);

            lectureToast.gameObject.SetActive(true);
            currentToast = lectureToast.gameObject;

            roomSceneManager.onRoomStateEvent += OnClassStart;
        }
        if (NetworkManager.User.userType == UserType.Student)
        {
            studentToast.gameObject.SetActive(true);
            currentToast = studentToast.gameObject;
        }

        DataManager.Instance.UpdateRoomProgress(user.email, 1);
    }

    public override void OnStateExit()
    {
        currentToast.SetActive(false);
        roomSceneManager.onRoomStateEvent -= OnClassStart;
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
     //On DisMic Toast;

    }

    public void OnClassStart()
    {
        photonView.RPC(nameof(ClassStartRPC), RpcTarget.All);
    }

    [PunRPC]
    public void ClassStartRPC()
    {
        roomSceneManager.RoomState = roomStateClass;
    }
}
