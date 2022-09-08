using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomState_Class : RoomState
{
    [Header("Room State")]
    public RoomState_ClassEnd roomStateClassEnd;

    [Header("Toast")]
    public ToastThreeButton lectureToast;
    public Toast studentToast;
    private GameObject currentToast;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            currentToast = lectureToast.gameObject;
        }
        if(NetworkManager.User.userType == UserType.Student)
        {
            currentToast = studentToast.gameObject;
        }

        currentToast.SetActive(true);
    }

    public override void OnStateExit()
    {

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
}
