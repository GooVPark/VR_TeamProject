using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class RoomState_StartQuiz : RoomState, IPunObservable
{
    [Header("Room State")]
    public RoomState_Quiz roomStateQuiz;
    [Space(5)]

    [Header("Toast")]
    public ToastTwoText toast;
    [Space(5)]

    private float time = 3f;

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        toast.gameObject.SetActive(true);
    }

    public override void OnStateExit()
    {
        toast.gameObject.SetActive(false);
        base.OnStateExit();
    }

    public override void OnUpdate()
    {
        toast.text.text = $"{(int)time + 1}";

        if (NetworkManager.User.userType == UserType.Lecture)
        {
            time -= Time.deltaTime;

            if(time < 0)
            {
                photonView.RPC(nameof(StartQuizRPC), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void StartQuizRPC()
    {
        roomSceneManager.RoomState = roomStateQuiz;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(NetworkManager.User.userType == UserType.Lecture)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(time);
            }
        }
        else
        {
            if(stream.IsReading)
            {
                time = (float)stream.ReceiveNext();
            }
        }

    }
}
