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
    public static VideoManager Instance;

    public Alert newVideoAlert;

    private VideoPlayer videoPlayer;
    private VideoClip currentVideo;

    public List<VideoClip> videos = new List<VideoClip>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        videoPlayer = GetComponent<VideoPlayer>();
    }

    public void SendSelectedVideo(int index)
    {
        //photonView.RPC(nameof(SendSelectedVideoRPC), RpcTarget.AllBufferedViaServer, index);
        currentVideo = videos[index];

        videoPlayer.clip = currentVideo;
        videoPlayer.Play();
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
