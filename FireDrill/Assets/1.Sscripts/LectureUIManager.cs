using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LectureUIManager : View
{
    public Button showVideoListButton;
    public Button closeButton;

    public GameObject videoListWindow;
    public GameObject videoInfoWindow;

    public Transform contents;
    public List<VideoElement> videoList = new List<VideoElement>();

    public View videoListView;

    public override void Initialize()
    {
        for (int i = 0; i < contents.childCount; i++)
        {
            videoList.Add(contents.GetChild(i).GetComponent<VideoElement>());
            int index = i;
            videoList[i].selectButton.onClick.AddListener(() => ViewManager.Show<VideoInfoView>(true));
        }

        closeButton.onClick.AddListener(() => ViewManager.ShowLast());
        showVideoListButton.onClick.AddListener(() => ViewManager.Show<VideoListView>(true));
    }

    public void ShowVideoList()
    {
        videoListWindow.SetActive(true);
    }

    public void ShowVideoInfo(int index)
    {
        videoInfoWindow.SetActive(true);
    }

    public void CloseVideoInfo()
    {
        videoInfoWindow.SetActive(false);
    }

   
}
