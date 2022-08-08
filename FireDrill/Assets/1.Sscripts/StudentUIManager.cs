using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StudentUIManager : MonoBehaviour
{
    public Button personalVideoButton;
    public Button quizButton;

    public GameObject personalVideoWindow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPersonalVideoWindow()
    {
        personalVideoWindow.SetActive(true);
    }

    public void ShowCurrentQuizWindow()
    {

    }
}
