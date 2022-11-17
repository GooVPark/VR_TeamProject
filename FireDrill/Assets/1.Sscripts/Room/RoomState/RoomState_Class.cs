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
    public ToastOneButton lectureToast;
    public ToastTwoButtons lectureEndClassToast;
    public Toast studentToast;
    private GameObject currentToast;
    public Toast disMicToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        //��� ������ ���̽�ê�� �ؽ�Ʈ ê�� ��� �Ұ���
        //voiceChat.button.onClick -= roomSceneManager.ToggleVoiceChat;
        voiceChat.button.OnClick.RemoveAllListeners();
        //voiceChat.button.onClick += MicDisabled;
        voiceChat.button.OnClick.AddListener(() => MicDisabled());

        //textChat.button.onClick -= roomSceneManager.ToggleTextChat;

        NetworkManager.Instance.voiceChatDisabled = true;
        roomSceneManager.DeactivateVoiceChat();
        //roomSceneManager.DisableTextChat();

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            InputManager.leftSecondaryButton += NextPage;
            InputManager.leftPrimaryButton += PrevPage;

            currentToast = lectureToast.gameObject;
            lectureToast.gameObject.SetActive(true);
            //roomSceneManager.MegaphoneOn();
            //megaphone.button.onClick += roomSceneManager.MegaphoneToggle;
            //megaphone.button.OnClick.AddListener(() => roomSceneManager.MegaphoneToggle());
        }
        if(NetworkManager.User.userType == UserType.Student)
        {
            currentToast = studentToast.gameObject;
        }

        currentToast.SetActive(true);
    }

    public override void OnStateExit()
    {
        //voiceChat.UpdateState(ButtonState.Deactivate);
        NetworkManager.Instance.voiceChatDisabled = false;
        NetworkManager.Instance.onVoiceChat = false;
        
        //voiceChat.button.onClick += roomSceneManager.ToggleVoiceChat;
        //voiceChat.button.onClick -= MicDisabled;

        voiceChat.button.OnClick.RemoveAllListeners();
        voiceChat.button.OnClick.AddListener(() => roomSceneManager.ToggleVoiceChat());

        //textChat.UpdateState(ButtonState.Deactivate);
        //textChat.button.onClick += roomSceneManager.ToggleTextChat;

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            InputManager.leftSecondaryButton -= NextPage;
            InputManager.leftPrimaryButton -= PrevPage;
            //roomSceneManager.MegaphoneOff();
            //megaphone.ChangeIconState(ButtonState.IconState.Off);
        }
        if (NetworkManager.User.userType == UserType.Student)
        {
            currentToast = studentToast.gameObject;
        }

        currentToast.SetActive(false);
        lectureEndClassToast.gameObject.SetActive(false);
        lectureToast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        
    }

    public void OnEndClassButton()
    {
        if(lectureEndClassToast.gameObject.activeSelf)
        {
            lectureEndClassToast.gameObject.SetActive(false);
            lectureToast.gameObject.SetActive(true);
        }
        else
        {
            lectureToast.gameObject.SetActive(false);
            lectureEndClassToast.gameObject.SetActive(true);
        }
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
