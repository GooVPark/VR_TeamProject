using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// lectureUI���� SelectVideoButton(int)�� SendSelectVideoȣ��
/// button�� index�� currentVideo�� Resources���� ���� ������
/// RPC�� ���� ��� VideoManager���� ���� ���
/// </summary>

public class VideoManager : MonoBehaviourPunCallbacks
{
    public Alert newVideoAlert;
    private VideoClip currentVideo;

    public void SendSelectedVideo(int index)
    {
        photonView.RPC(nameof(SendSelectedVideoRPC), RpcTarget.AllBufferedViaServer, index);
    }

    [PunRPC]
    public void SendSelectedVideoRPC(int index)
    {
        newVideoAlert.OnAlert();
    }

    public void ShowVideoInfo(int index)
    {
        
    }
}
