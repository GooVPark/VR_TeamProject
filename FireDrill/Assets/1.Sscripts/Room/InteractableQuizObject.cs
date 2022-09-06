using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InteractableQuizObject : MonoBehaviour
{
    /*
     * 퀴즈를 푸는것 자체는 네트워크에 반영되지 않는다.
     * DB에는 문제의 번호, 정답 현황만을 보낸다.
     * 따라서 네트워크 동기화도 하지 않는다.
     * 답은 QuizManager에서 모아서 동기화한다.
     */
    [Header("Quiz")]
    [SerializeField] private GameObject quizUI;
    [SerializeField] private TMP_Text question;
    [SerializeField] private int answerCount = 0;
    [SerializeField] private int[] answers;
    [SerializeField] private int code;
    [Space(5)]

    [Header("Selection Quiz")]
    [SerializeField] private TMP_Text selectionQuizText;
    [SerializeField] private GameObject selectionQuizUI;
    [SerializeField] private QuizSlot[] selections;
    [Space(5)]

    [Header("Sequence Quiz")]
    [SerializeField] private TMP_Text sequenceQuizText;
    [SerializeField] private GameObject sequenceQuizUI;
    [SerializeField] private QuizSlot[] slots;
    [SerializeField] private QuizSlot[] sequences;

    [SerializeField] private int currentSlot = 0;
    [SerializeField] private int currentSequence = 0;
    [Space(5)]

    [Header("OX Quiz")]
    [SerializeField] private TMP_Text oxQuizText;
    [SerializeField] private GameObject oxQuizUI;
    [SerializeField] private QuizSlot[] ox;

    [Header("Parameters")]
    QuizJson quiz;
    [SerializeField] private Transform userTransform; // 로컬 플레이어의 transform을 가져옴
    
    public void OnSelected()
    {
        quizUI.SetActive(true);

        quiz = DataManager.Instance.quizsByCode[code];
        question.text = quiz.question;
        answers = new int[quiz.answer.Length];
        answerCount = answers.Length;

        switch (quiz.type)
        {
            case QuizType.Selection:
                selectionQuizText.text = quiz.question;
                for (int i = 0; i < selections.Length; i++)
                {
                    selections[i].SetText(quiz.selections[i]);
                }
                selectionQuizUI.SetActive(true);
                break;
            case QuizType.Sequence:
                sequenceQuizText.text = quiz.question;
                for (int i = 0; i < sequences.Length; i++)
                {
                    sequences[i].SetText(quiz.selections[i]);
                }
                sequenceQuizUI.SetActive(true);
                break;
            case QuizType.OX:
                oxQuizText.text = quiz.question;
                for(int i = 0; i < ox.Length; i++)
                {
                    ox[i].SetText(quiz.selections[i]);
                }
                oxQuizUI.SetActive(true);
                break;
        }

        DistanceCheck();
    }

    public void OnHovered()
    {
        //컨트롤러의 Ray가 닿으면 하이라이트 효과 호출
    }

    public void SelectAnswer(int number)
    {
        answers[currentSlot] = number;
    }

    public void SelectSlot(int number)
    {
        Debug.Log("Select Slot");
        if(currentSlot != 0) //슬롯을 연속으로 누른 경우
        {
            //이전 슬롯의 정보를 선택한 슬롯에 넘겨주고 초기화
            string temp = slots[number - 1].GetText();
            slots[number-1].SetText(slots[currentSlot-1].GetText());
            slots[currentSlot - 1].SetText(temp);
            currentSlot = number;

            currentSlot = 0;
            currentSequence = 0;
        }
        //슬롯을 먼저 선택한 경우
        else if (currentSequence == 0)
        {
            currentSlot = number;
            return;
        }
        else //선택한 선택지가 있는 경우
        {
            currentSlot = number;
            SelectAnswer(currentSequence-1);

            slots[number-1].SetText(sequences[currentSequence-1].GetText());
            //sequences[currentSlot-1].SetInteractable(false);

            currentSlot = 0;
            currentSequence = 0;
        }
    }

    public void SelectSequence(int number)
    {
        //선택지를 먼저 선택한 경우
        if(currentSlot == 0)
        {
            currentSequence = number;
            //sequences[number-1].SetInteractable(false);
        }
        else if(currentSequence != 0) //선택지를 연속으로 선택한 경우
        {
            //sequences[currentSequence-1].SetInteractable(true);
            currentSequence = number;
            //sequences[currentSequence-1].SetInteractable(false);
        }
        else //선택한 슬롯이 있을 경우
        {
            currentSequence = number;
            SelectAnswer(currentSequence-1);
            slots[currentSlot-1].SetText(sequences[currentSequence-1].GetText());
            //sequences[currentSequence-1].SetInteractable(false);

            currentSlot = 0;
            currentSequence = 0;
        }
    }

    public void Submit()
    {
        int result = 1;

        for(int i = 0; i < answers.Length; i++)
        {
            if (quiz.answer[i] != answers[i])
            {
                result = 2;
                break;
            }
        }
        
        DataManager.Instance.SetQuizResult(NetworkManager.User.email, result, code);
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
