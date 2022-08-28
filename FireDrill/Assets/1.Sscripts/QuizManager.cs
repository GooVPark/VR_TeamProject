using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;
using Photon.Pun;

public enum QuizState { Correct, Incorrect, NotYet }

public class QuizManager : MonoBehaviourPun
{
    public static QuizManager Instance;
    public Alert alert;
    public int currentQuizIndex = -1;

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


    }

    #region Lectuure's Quiz UI Interactions

    public void SendSelectedQuiz(int index)
    {
        Debug.Log("Send selected Quiz");
        photonView.RPC(nameof(SendSelectedQuizRPC), RpcTarget.AllBufferedViaServer, index);
    }

    [PunRPC]
    public void SendSelectedQuizRPC(int index)
    {
        Debug.Log("Send selected Quiz RPC");
        alert.OnAlert();
        currentQuizIndex = index;
    }

    #endregion

    #region Student's Quiz UI Interactions

    public void SubmitAnswer(int id, int quizIndex, int answer)
    {
        photonView.RPC(nameof(SubmitAnswerRPC), RpcTarget.All, id, quizIndex, answer);
    }

    [PunRPC]
    public void SubmitAnswerRPC(int id, int quizIndex, int answer)
    {
        QuizState state = quizList[quizIndex].IsAnswer(answer);
        //DataManager.Instance.userDB.userDB[id].UpdateAnswer(quizIndex, answer, state);
    }

    #endregion

    #region ScoreBoard ManageMent

    #endregion
}

[System.Serializable]
public class Answer
{
    public Answer(QuizState state, int answer)
    {
        this.state = state;
        this.answer = answer;
    }

    [SerializeField] private int answer;
    [SerializeField] private QuizState state;

    public void Update(QuizState state, int answer)
    {
        this.state = state;
        this.answer = answer;
    }
}
