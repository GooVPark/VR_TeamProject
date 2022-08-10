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

    public Button showQuizListView;

    public Transform quizContents;
    public List<Button> quizList = new List<Button>();
    public GameObject quizInfoView;


    public override void Initialize()
    {
        for (int i = 0; i < contents.childCount; i++)
        {
            videoList.Add(contents.GetChild(i).GetComponent<VideoElement>());
            int index = i;
            videoList[i].selectButton.onClick.AddListener(() => ViewManager.Show<VideoInfoView>(true));
            videoList[i].selectButton.onClick.AddListener(() => ShowVideoInfo(index));
        }

        closeButton.onClick.AddListener(() => ViewManager.ShowLast());
        showVideoListButton.onClick.AddListener(() => ViewManager.Show<VideoListView>(true));

        for (int i = 0; i < quizContents.childCount; i++)
        {
            quizList.Add(quizContents.GetChild(i).GetComponent<Button>());
            int index = i;
            quizList[i].onClick.AddListener(() => ViewManager.Show<QuizInfoView>(true));
            quizList[i].onClick.AddListener(() => ShowQuizInfo(index));
        }

        showQuizListView.onClick.AddListener(() => ViewManager.Show<QuizListView>(true));
    }

    public override void Hide()
    {
        base.Hide();
        InputManager.IsLeftHandStartContextValue = false;
    }

    public void ShowVideoInfo(int index)
    {
        videoInfoWindow.GetComponent<VideoInfoView>().currentVideoIndex = index;
    }

    public void ShowQuizInfo(int index)
    {
        quizInfoView.GetComponent<QuizInfoView>().currentQuizIndex = index;
    }

   
}
