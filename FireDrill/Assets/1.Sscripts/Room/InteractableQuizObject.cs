using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableQuizObject : MonoBehaviour
{
    /*
     * 퀴즈를 푸는것 자체는 네트워크에 반영되지 않는다.
     * DB에는 문제의 번호, 정답 현황만을 보낸다.
     * 따라서 네트워크 동기화도 하지 않는다.
     * 답은 QuizManager에서 모아서 동기화한다.
     */
    [Header("Quiz UI")]
    [SerializeField] private GameObject quizUI;
    [Space(5)]

    [Header("Parameters")]
    [SerializeField] private Transform userTransform; // 로컬 플레이어의 transform을 가져옴
    public void OnSelected()
    {
        quizUI.SetActive(true);
        DistanceCheck();
    }

    public void OnHovered()
    {
        //컨트롤러의 Ray가 닿으면 하이라이트 효과 호출
    }


    /// <summary>
    /// QuizManager에 오지선다, OX퀴즈의 답을 제출
    /// </summary>
    /// <param name="answer"></param>
    public void Submit(int answer)
    {

    }

    /// <summary>
    /// QuizManager에 순서 배열 문제의 답을 제출
    /// </summary>
    /// <param name="answers"></param>
    public void Submit(int[] answers)
    {

    }

    /// <summary>
    /// 퀴즈 오브젝트로부터 너무 멀어지면 자동으로 퀴즈창이 꺼지도록함.
    /// </summary>
    private void DistanceCheck()
    {
        if (distanceCheck != null)
        {
            StopCoroutine(distanceCheck);
            distanceCheck = null;
        }
        distanceCheck = StartCoroutine(DistanceCheckIE());
    }
    private Coroutine distanceCheck;
    private IEnumerator DistanceCheckIE()
    {
        float distance = 0f;
        do
        {
            distance = Vector3.Distance(transform.position, userTransform.position);
            yield return null;
        }
        while (quizUI.activeSelf && distance < 5f);
    }
}
