using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizObjectManager : MonoBehaviour
{
    //1. �� Ÿ�Ժ��� 3~4���� �����ϰ� ��ȣ ����
    //2. �����ϰ� ���� ��ȣ�� QuizJson �ҷ��ͼ� List<QuizJson>���� ����
    //3. foreach ������ quizObjects�� quizPosition���� ����
    //4. position �ߺ� �� �������� Ǯ �� �ְ�
    public InteractableQuizObject[] quizObjects;

    [SerializeField] private int typeACount;
    [SerializeField] private int typeBCount;
    [SerializeField] private int typeCCount;

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

    public List<QuizJson> GetQuizs()
    {
        List<QuizJson> quizList = new List<QuizJson>();

        List<QuizJson> typeAList = DataManager.Instance.GetQuizListByType("TypeA");
        List<QuizJson> typeBList = DataManager.Instance.GetQuizListByType("TypeB");
        List<QuizJson> typeCList = DataManager.Instance.GetQuizListByType("TypeC");

        List<int> randomNumberA = GetRandomNumbers(typeACount);
        List<int> randomNumberB = GetRandomNumbers(typeBCount);
        List<int> randomNumberC = GetRandomNumbers(typeCCount);

        for (int i = 0; i < typeACount; i++)
        {
            quizList.Add(typeAList[randomNumberA[i]]);
        }
        for(int i = 0; i < typeBCount; i++)
        {
            quizList.Add(typeBList[randomNumberB[i]]);
        }
        for(int i = 0;  i < typeCCount; i++)
        {
            quizList.Add(typeCList[randomNumberC[i]]);
        }

        return quizList;
    }

    public void SetQuiz()
    {
        List<QuizJson> quizList = GetQuizs();
        foreach(var quiz in quizList)
        {
            quizObjects[quiz.quizIndex].quizs.Add(quiz);
        }
    }
}