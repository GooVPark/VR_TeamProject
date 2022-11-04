using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuizObjectManager : MonoBehaviourPun
{
    //1. �� Ÿ�Ժ��� 3~4���� �����ϰ� ��ȣ ����
    //2. �����ϰ� ���� ��ȣ�� QuizJson �ҷ��ͼ� List<QuizJson>���� ����
    //3. foreach ������ quizObjects�� quizPosition���� ����
    //4. position �ߺ� �� �������� Ǯ �� �ְ�
    public InteractableQuizObject[] quizObjects;

    [SerializeField] private int typeACount;
    [SerializeField] private int typeBCount;
    [SerializeField] private int typeCCount;

    public List<QuizObject> typeAList;
    public List<QuizObject> typeBList;
    public List<QuizObject> typeCList;

    private int quizIndex = 0;

    public Dictionary<int, int[]> quizs = new Dictionary<int, int[]>();

    private void Start()
    {

    }

    public List<int> GetRandomNumbers(int maxCount)
    {
        List<int> numbers = new List<int>();
        List<int> randomNumber = new List<int>();
        for(int i = 0; i < 10; i++)
        {
            numbers.Add(i);
        }

        System.Random random = new System.Random();

        for(int i = 0; i < maxCount; i++)
        {
            int index = random.Next(numbers.Count);
            randomNumber.Add(numbers[index]);
            numbers.RemoveAt(index);
        }

        return randomNumber;
    }

    public Dictionary<int, int[]> GetQuizs()
    {
        Dictionary<int, int[]> quizListByTpye = new Dictionary<int, int[]>();

        List<int> randomNumberA = GetRandomNumbers(typeACount);
        List<int> randomNumberB = GetRandomNumbers(typeBCount);
        List<int> randomNumberC = GetRandomNumbers(typeCCount);

        quizListByTpye.Add(0, randomNumberA.ToArray());
        quizListByTpye.Add(1, randomNumberB.ToArray());
        quizListByTpye.Add(2, randomNumberC.ToArray());

        return quizListByTpye;
    }

    public void SetQuiz()
    {
        Dictionary<int, int[]> quizList = GetQuizs();
        photonView.RPC(nameof(SetQuizRPC), RpcTarget.All, quizList);
    }

    [PunRPC]
    public void SetQuizRPC(Dictionary<int, int[]> quizs)
    {
        PlayerDetector player = FindObjectOfType<PlayerDetector>();

        int index = 0;
        for(int i = 0; i < quizs.Count; i++)
        {
            for(int j = 0; j < quizs[i].Length; j++)
            {
                QuizObject quiz = null;

                switch(i)
                {
                    case 0:
                        quiz = typeAList[quizs[i][j]];
                        break;
                    case 1:
                        quiz = typeBList[quizs[i][j]];
                        break;
                    case 2:
                        quiz = typeCList[quizs[i][j]];
                        break;
                }
                quizObjects[quiz.position].quizObjects.Add(quiz);
                quiz.quizIndex = index;
                index++;
            }
        }

        for(int i = 0; i < quizObjects.Length; i++)
        {
            if(quizObjects[i].quizObjects.Count == 0)
            {
                quizObjects[i].gameObject.SetActive(false);
            }
            else
            {
                quizObjects[i].target = player.transform;
            }
        }
    }
}