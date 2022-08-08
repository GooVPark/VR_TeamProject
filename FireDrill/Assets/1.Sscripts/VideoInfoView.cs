using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoInfoView : View
{
    public int currentVideoIndex;
    public Button playClipButton;

    public override void Initialize()
    {
        playClipButton.onClick.AddListener(() => PlayVideoClip());
    }

    public void PlayVideoClip()
    {
        VideoManager.Instance.SendSelectedVideo(currentVideoIndex);
    }
}
