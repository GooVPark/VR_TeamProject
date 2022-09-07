using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text index;
    [SerializeField] private TMP_Text userName;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text[] results;

    public void UpdateScore(User user)
    {
        userName.text = user.name;
        int score = 0;
        for(int i = 0; i < user.quizResult.Length; i++)
        {
            int result = user.quizResult[i];
            switch(result)
            {
                case 0:
                    results[i].text = "";
                    break;
                case 1:
                    results[i].text = "O";
                    score += 10;
                    break;
                case 2:
                    results[i].text = "X";
                    break;
            }
        }

        this.score.text = score.ToString();
    }
}
