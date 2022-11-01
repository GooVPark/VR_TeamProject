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

    public List<QuizObject> GetQuizs()
    {
        List<QuizObject> quizList = new List<QuizObject>();

        List<int> randomNumberA = GetRandomNumbers(typeACount);
        List<int> randomNumberB = GetRandomNumbers(typeBCount);
        List<int> randomNumberC = GetRandomNumbers(typeCCount);

        for (int i = 0; i < typeACount; i++)
        {
            typeAList[randomNumberA[i]].quizIndex = quizIndex;
            quizList.Add(typeAList[randomNumberA[i]]);
            quizIndex++;
        }
        for(int i = 0; i < typeBCount; i++)
        {
            typeBList[randomNumberB[i]].quizIndex = quizIndex;
            quizList.Add(typeBList[randomNumberB[i]]);
            quizIndex++;
        }
        for(int i = 0;  i < typeCCount; i++)
        {
            typeCList[randomNumberC[i]].quizIndex = quizIndex;
            quizList.Add(typeCList[randomNumberC[i]]);
            quizIndex++;
        }

        return quizList;
    }

    public void SetQuiz()
    {
        photonView.RPC(nameof(SetQuizRPC), RpcTarget.All);
    }

    [PunRPC]
    public void SetQuizRPC()
    {
        List<QuizObject> quizList = GetQuizs();
        foreach (var quiz in quizList)
        {
            quizObjects[quiz.position].quizObjects.Add(quiz);
        }

        foreach (var quizObject in quizObjects)
        {
            quizObject.HasQuiz();
        }
    }
}