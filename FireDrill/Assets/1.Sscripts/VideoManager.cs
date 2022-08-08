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
