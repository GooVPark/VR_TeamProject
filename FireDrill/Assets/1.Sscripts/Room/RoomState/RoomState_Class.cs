using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 강사가 강의를 진행중인 상황
/// 강사의 확성기가 자동으로 켜짐
/// 강사의 UI에서 강의자료 조작 가능
/// 강사가 강의 종료 버튼을 누르면 다음 상태로
/// 
/// 보이스, 택스트챗 사용 불가능
/// 확성기 사용가능 (자동)
/// 채점판 사용 불가능
/// 
/// 진행상황
/// 알림 메세지 O
/// 강의자료 컨트롤 O
/// 확성기 기능 X
/// 보이스, 택스트 쳇 토글 O
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

        //모든 유저의 보이스챗과 텍스트 챗을 사용 불가로
        voiceChat.ChangeIconState(ButtonState.IconState.Disable);
        voiceChat.onButtonEvent -= roomSceneManager.ToggleVoiceChat;
        voiceChat.onButtonEvent += MicDisabled;

        textChat.ChangeIconState(ButtonState.IconState.Disable);
        textChat.onButtonEvent -= roomSceneManager.ToggleTextChat;

        roomSceneManager.DisableVoiceChat();
        roomSceneManager.DisableTextChat();

        textChat.ChangeIconState(ButtonState.IconState.Disable);

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            currentToast = lectureToast.gameObject;

            roomSceneManager.MegaphoneOn();
            megaphone.ChangeIconState(ButtonState.IconState.On);
            megaphone.onButtonEvent += roomSceneManager.MegaphoneToggle;
        }
        if(NetworkManager.User.userType == UserType.Student)
        {
            currentToast = studentToast.gameObject;
        }

        currentToast.SetActive(true);
    }

    public override void OnStateExit()
    {
        voiceChat.ChangeIconState(ButtonState.IconState.Off);
        voiceChat.onButtonEvent += roomSceneManager.ToggleVoiceChat;
        voiceChat.onButtonEvent -= MicDisabled;

        textChat.ChangeIconState(ButtonState.IconState.Off);
        textChat.onButtonEvent += roomSceneManager.ToggleTextChat;

        if (NetworkManager.User.userType == UserType.Lecture)
        { 
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
