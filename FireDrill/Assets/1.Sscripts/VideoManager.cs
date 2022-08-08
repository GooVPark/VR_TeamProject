using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// lectureUI에서 SelectVideoButton(int)로 SendSelectVideo호출
/// button의 index로 currentVideo에 Resources에서 비디오 가져옴
/// RPC로 룸의 모든 VideoManager에서 영상 재생
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
