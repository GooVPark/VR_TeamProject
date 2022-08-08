using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuizManager : MonoBehaviourPun
{
    public static QuizManager Instance;
    public Alert alert;
    public int currentQuizIndex;

    public List<QuizObject> quizList = new List<QuizObject>();


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

    public void SendSelectedQuiz(int index)
    {
        photonView.RPC(nameof(SendSelectedQuizRPC), RpcTarget.AllBufferedViaServer, index);
    }

    [PunRPC]
    public void SendSelectedQuizRPC(int index)
    {
        alert.OnAlert();
        currentQuizIndex = index;
    }
    
}
