using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableQuizObject : MonoBehaviour
{
    /*
     * ��� Ǫ�°� ��ü�� ��Ʈ��ũ�� �ݿ����� �ʴ´�.
     * DB���� ������ ��ȣ, ���� ��Ȳ���� ������.
     * ���� ��Ʈ��ũ ����ȭ�� ���� �ʴ´�.
     * ���� QuizManager���� ��Ƽ� ����ȭ�Ѵ�.
     */
    [Header("Quiz UI")]
    [SerializeField] private GameObject quizUI;
    [Space(5)]

    [Header("Parameters")]
    [SerializeField] private Transform userTransform; // ���� �÷��̾��� transform�� ������
    public void OnSelected()
    {
        quizUI.SetActive(true);
        DistanceCheck();
    }

    public void OnHovered()
    {
        //��Ʈ�ѷ��� Ray�� ������ ���̶���Ʈ ȿ�� ȣ��
    }


    /// <summary>
    /// QuizManager�� ��������, OX������ ���� ����
    /// </summary>
    /// <param name="answer"></param>
    public void Submit(int answer)
    {

    }

    /// <summary>
    /// QuizManager�� ���� �迭 ������ ���� ����
    /// </summary>
    /// <param name="answers"></param>
    public void Submit(int[] answers)
    {

    }

    /// <summary>
    /// ���� ������Ʈ�κ��� �ʹ� �־����� �ڵ����� ����â�� ����������.
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
