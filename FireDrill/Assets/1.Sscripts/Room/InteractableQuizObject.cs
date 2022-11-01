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

    public delegate void EventMessage(string message);
    public EventMessage eventMessage;

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
    [SerializeField] private ButtonInteractor selectionSubmitButton;
    [Space(5)]

    [Header("Sequence Quiz")]
    [SerializeField] private TMP_Text sequenceQuizText;
    [SerializeField] private GameObject sequenceQuizUI;
    [SerializeField] private QuizSlot[] slots;
    [SerializeField] private QuizSlot[] sequences;
    [SerializeField] private ButtonInteractor sequenceSubmitButton;

    [SerializeField] private int currentSlot = 0;
    [SerializeField] private int currentSequence = 0;
    [Space(5)]

    [Header("OX Quiz")]
    [SerializeField] private TMP_Text oxQuizText;
    [SerializeField] private GameObject oxQuizUI;
    [SerializeField] private QuizSlot[] ox;
    [SerializeField] private ButtonInteractor oxSubmitButton;
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
    private bool isHovered;
    private bool isActivated;
    public int quizNumber = 0;

    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite finishSprite;

    [SerializeField] private Image signImage;

    public List<QuizJson> quizs = new List<QuizJson>();

    [SerializeField] private Transform userTransform; // 로컬 플레이어의 transform을 가져옴

    private void Start()
    {
        eventMessage += EventSyncronizerRoom.Instance.OnSendMessage;

        if(NetworkManager.User.userType == UserType.Lecture)
        {
            signImage.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (isSolved)
        {
            signImage.sprite = finishSprite;
        }
        else
        {
            signImage.sprite = readySprite;
        }
    }

    public void OnSelected()
    { 
        if(!isHovered)
        {
            return;
        }

        quizUI.SetActive(true);
        signImage.gameObject.SetActive(false);
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
    }

    public void OnHoverEntered()
    {
        if (isActivated)
        {
            isHovered = true;
        }
    }

    public void OnHoverExited()
    {
        isHovered = false;
    }

    public void Activate()
    {
        isActivated = true;
    }

    public void Deactivate()
    {
        isActivated = false;
    }

    public void SelectAnswer(int number)
    {
        answers[currentSlot] = number;
    }

    public void SelectSlot(int number)
    {
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

        selectionSubmitButton.collider.enabled = false;
        sequenceSubmitButton.collider.enabled = false;
        oxSubmitButton.collider.enabled = false;

        isSolved = true;

        StartCoroutine(OXFeedback(2f, result));
        DataManager.Instance.SetQuizResult(NetworkManager.User.email, result, quizNumber);

        string message = $"{EventMessageType.QUIZ}";
        eventMessage?.Invoke(message);
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
            text = "정답입니다.\n10점을 획득 하셨습니다.";
        }
        else if(result == 1)
        {
            text = "오답입니다.\n";
        }

        feedbackTitle.text = text;
        feedbackBody.text = quiz.solutions[result];
    }

    public void CloseQuizWindow()
    {
        isSolved = true;
        //solvedUI.SetActive(true);
        //quizUI.SetActive(false);
        feedbackUI.SetActive(false);
        signImage.gameObject.SetActive(true);

        if(isSolved)
        {
            signImage.sprite = finishSprite;
        }
        else
        {
            signImage.sprite = readySprite;
        }
    }
}
