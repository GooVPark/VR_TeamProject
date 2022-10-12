using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ���簡 ���Ǹ� �������� ��Ȳ
/// ������ Ȯ���Ⱑ �ڵ����� ����
/// ������ UI���� �����ڷ� ���� ����
/// ���簡 ���� ���� ��ư�� ������ ���� ���·�
/// 
/// ���̽�, �ý�Ʈê ��� �Ұ���
/// Ȯ���� ��밡�� (�ڵ�)
/// ä���� ��� �Ұ���
/// 
/// �����Ȳ
/// �˸� �޼��� O
/// �����ڷ� ��Ʈ�� O
/// Ȯ���� ��� X
/// ���̽�, �ý�Ʈ �� ��� O
/// </summary>
public class RoomState_Class : RoomState
{
    [Header("Room State")]
    public RoomState_ClassEnd roomStateClassEnd;

    [Header("Toast")]
    public ToastThreeButton lectureToast;
    public Toast studentToast;
    private GameObject currentToast;
    public Toast disMicToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        //��� ������ ���̽�ê�� �ؽ�Ʈ ê�� ��� �Ұ���
        voiceChat.button.onClick -= roomSceneManager.ToggleVoiceChat;
        voiceChat.button.onClick += MicDisabled;

        //textChat.button.onClick -= roomSceneManager.ToggleTextChat;

        roomSceneManager.DeactivateVoiceChat();
        //roomSceneManager.DisableTextChat();

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            currentToast = lectureToast.gameObject;
            roomSceneManager.MegaphoneOn();
            megaphone.button.onClick += roomSceneManager.MegaphoneToggle;
        }
        if(NetworkManager.User.userType == UserType.Student)
        {
            currentToast = studentToast.gameObject;
        }

        currentToast.SetActive(true);
    }

    public override void OnStateExit()
    {
        voiceChat.UpdateState(ButtonState.Deactivate);
        voiceChat.button.onClick += roomSceneManager.ToggleVoiceChat;
        voiceChat.button.onClick -= MicDisabled;

        //textChat.UpdateState(ButtonState.Deactivate);
        //textChat.button.onClick += roomSceneManager.ToggleTextChat;

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            //roomSceneManager.MegaphoneOff();
            //megaphone.ChangeIconState(ButtonState.IconState.Off);
        }
        if (NetworkManager.User.userType == UserType.Student)
        {
            currentToast = studentToast.gameObject;
        }

        currentToast.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        
    }

    public void ClasssEnd()
    {
        photonView.RPC(nameof(ClassEndRPC), RpcTarget.All);
    }

    [PunRPC]
    public void ClassEndRPC()
    {
        roomSceneManager.RoomState = roomStateClassEnd;
    }

    public void NextPage()
    {
        photonView.RPC(nameof(NextPageRPC), RpcTarget.All);
    }

    [PunRPC]
    public void NextPageRPC()
    {
        roomSceneManager.NextPage();
    }

    public void PrevPage()
    {
        photonView.RPC(nameof(PrevPageRPC), RpcTarget.All);
    }

    [PunRPC]
    public void PrevPageRPC()
    {
        roomSceneManager.PrevPage();
    }

    public void MicDisabled()
    {
        disMicToast.Activate(2.0f);
    }
}
