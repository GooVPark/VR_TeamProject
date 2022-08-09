using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;

using TMPro;

public class ScoreBoard : MonoBehaviour
{
    public Transform content;
    public TableUI tableUI;

    int columCount = 0;
    private void Start()
    {
        //columCount = 1 + QuizManager.Instance.quizList.Count;
        
        
        //content.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "¿Ã∏ß";

        //for(int i = 1; i < columCount; i++)
        //{
        //    content.GetChild(0).GetChild(1).GetChild(i).GetComponent<TMP_Text>().text = $"{i:00}";
        //}
    }

    private void SetTableUI(Dictionary<string, List<Answer>> scores)
    {
        int index = 0;
        foreach(string key in scores.Keys)
        {
            content.GetChild(index).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = key;
            index++;
        }
    }

    public void UpdateScore(string name, int index, Answer answer)
    {
        
    }
}
