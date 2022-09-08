using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InteractableQuizObject : MonoBehaviour
{
    #region Events

    public delegate void SubmitEvent(int value);
    public SubmitEvent onSubmit;

    #endregion

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
    [Space(5)]

    [Header("Feedback")]
    [SerializeField] private GameObject[] oxFeedbackUI;

    [SerializeField] private GameObject feedbackUI;
    [SerializeField] private TMP_Text feedbackTitle;
    [SerializeField] private TMP_Text feedbackBody;

    [SerializeField] private AudioClip correctAudio;
    [SerializeField] private AudioClip incorrectAudio;

    [SerializeField] private GameObject solvedUI;
    private GameObject currentQuizUI;

    [Header("Parameters")]
    QuizJson quiz;
    private bool isSolved = false;
    [SerializeField] private Transform userTransform; // ���� �÷��̾��� transform�� ������
    
    public void OnSelected()
    { 
        quizUI.SetActive(true);
        if (isSolved) return;


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
                currentQuizUI = selectionQuizUI;
                break;
            case QuizType.Sequence:
                sequenceQuizText.text = quiz.question;
                for (int i = 0; i < sequences.Length; i++)
                {
                    sequences[i].SetText(quiz.selections[i]);
                }
                sequenceQuizUI.SetActive(true);
                currentQuizUI = sequenceQuizUI;
                break;
            case QuizType.OX:
                oxQuizText.text = quiz.question;
                for(int i = 0; i < ox.Length; i++)
                {
                    ox[i].SetText(quiz.selections[i]);
                }
                oxQuizUI.SetActive(true);
                currentQuizUI = oxQuizUI;
                break;
        }

        DistanceCheck();
    }

    public void OnHovered()
    {
        //��Ʈ�ѷ��� Ray�� ������ ���̶���Ʈ ȿ�� ȣ��
    }

    public void SelectAnswer(int number)
    {
        answers[currentSlot] = number;
    }

    public void SelectSlot(int number)
    {
        if(currentSlot != 0) //������ �������� ���� ���
        {
            //���� ������ ������ ������ ���Կ� �Ѱ��ְ� �ʱ�ȭ
            string temp = slots[number - 1].GetText();
            slots[number-1].SetText(slots[currentSlot-1].GetText());
            slots[currentSlot - 1].SetText(temp);
            currentSlot = number;

            currentSlot = 0;
            currentSequence = 0;
        }
        //������ ���� ������ ���
        else if (currentSequence == 0)
        {
            currentSlot = number;
            return;
        }
        else //������ �������� �ִ� ���
        {
            currentSlot = number-1;
            SelectAnswer(currentSequence);

            slots[number-1].SetText(sequences[currentSequence-1].GetText());
            //sequences[currentSlot-1].SetInteractable(false);

            currentSlot = 0;
            currentSequence = 0;
        }
    }

    public void SelectSequence(int number)

    {
        //�������� ���� ������ ���
        if(currentSlot == 0)
        {
            currentSequence = number;
            //sequences[number-1].SetInteractable(false);
        }
        else if(currentSequence != 0) //�������� �������� ������ ���
        {
            //sequences[currentSequence-1].SetInteractable(true);
            currentSequence = number;
            //sequences[currentSequence-1].SetInteractable(false);
        }
        else //������ ������ ���� ���
        {
            currentSequence = number;
            SelectAnswer(currentSequence);
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

        onSubmit?.Invoke(result);
        
        StartCoroutine(OXFeedback(2f, result));
        DataManager.Instance.SetQuizResult(NetworkManager.User.email, result, code);
    }

    IEnumerator OXFeedback(float duration, int result)
    {
        oxFeedbackUI[result - 1].SetActive(true);

        yield return new WaitForSeconds(duration);

        oxFeedbackUI[result - 1].SetActive(false);
        currentQuizUI.SetActive(false);
        ShowSolution(result-1);
    }

    private void ShowSolution(int result)
    {
        feedbackUI.SetActive(true);
        string text = string.Empty;
        if (result == 0)
        {
            text = "�����Դϴ�.\n10���� ȹ�� �ϼ̽��ϴ�.";
        }
        else if(result == 1)
        {
            text = "�����Դϴ�.\n";
        }

        feedbackTitle.text = text;
        feedbackBody.text = quiz.solutions[result];
    }

    public void CloseQuizWindow()
    {
        isSolved = true;
        solvedUI.SetActive(true);
        quizUI.SetActive(false);
        feedbackUI.SetActive(false);
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
