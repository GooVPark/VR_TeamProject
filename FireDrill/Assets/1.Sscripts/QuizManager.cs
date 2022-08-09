using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;
using Photon.Pun;

public class QuizManager : MonoBehaviourPun
{
    public static QuizManager Instance;
    public Alert alert;
    public int currentQuizIndex;

    public List<QuizObject> quizList = new List<QuizObject>();
    public Dictionary<string, List<Answer>> scoreBoardDict = new Dictionary<string, List<Answer>>();
    public List<Answer> personalQuizAnswerList = new List<Answer>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
        }

        for(int i = 0; i < quizList.Count; i++)
        {
            personalQuizAnswerList.Add(new Answer(false, -1));
        }
    }

    #region Lectuure's Quiz UI Interactions

    public void SendSelectedQuiz(int index)
    {
        //photonView.RPC(nameof(SendSelectedQuizRPC), RpcTarget.AllBufferedViaServer, index);

        alert.OnAlert();
        currentQuizIndex = index;
    }

    [PunRPC]
    public void SendSelectedQuizRPC(int index)
    {
        alert.OnAlert();
        currentQuizIndex = index;
    }

    #endregion

    #region Student's Quiz UI Interactions

    public void SubmitAnswer(int quizIndex, int answer)
    {
        bool isAnswer = quizList[quizIndex].IsAnswer(answer);
        personalQuizAnswerList[quizIndex].Set(isAnswer, answer);
    }

    [PunRPC]
    public void SubmitAnswerRPC()
    {

    }

    #endregion

    #region ScoreBoard ManageMent

    #endregion
}

[System.Serializable]
public class Answer
{
    public Answer(bool isCollect, int answer)
    {
        this.isCollect = isCollect;
        this.answer = answer;
    }

    private int answer;
    private bool isCollect = false;

    public void Set(bool isCollect, int answer)
    {
        this.isCollect = isCollect;
        this.answer = answer;
    }
}
