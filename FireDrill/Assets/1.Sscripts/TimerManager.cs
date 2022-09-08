using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;

    
    private void Start()
    {
        StartCoroutine(Timer(600));
    }
    IEnumerator Timer(int time)
    {
        while (time > 0)
        {
            int min = time / 60;
            int sec = time % 60;
            timerText.text = ("남은시간/ " + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec);
            yield return new WaitForSecondsRealtime(1.0f);
            time--;
        }
    }
}
