using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizObjectManager : MonoBehaviour
{
    //1. 각 타입별로 3~4개씩 랜덤하게 번호 지정
    //2. 랜덤하게 뽑힌 번호로 QuizJson 불러와서 List<QuizJson>으로 저장
    //3. foreach 돌려서 quizObjects에 quizPosition으로 지정
    //4. position 중복 시 연속으로 풀 수 있게
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