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
    [SerializeField] private GameObject selectionQuestionUI;
    [SerializeField] private GameObject selectionFeedbackUI;
    [SerializeField] private TMP_Text selectionFeedbackResult;
    [SerializeField] private TMP_Text selectionFeedbackDescript;
    [SerializeField] private TMP_Text selectionFeedbackAnswerText;
    [SerializeField] private TMP_Text selectionQIndex;
    [SerializeField] private TMP_Text selectionAIndex;
    [SerializeField] private GameObject selectionFeedbackAnswer;
    [SerializeField] private QuizSlot[] selections;
    [SerializeField] private ButtonInteractor selectionSubmitButton;
    [Space(5)]

    [Header("OX Quiz")]
    [SerializeField] private TMP_Text oxQuizText;
    [SerializeField] private GameObject oxQuizUI;
    [SerializeField] private GameObject oxQuestionUI;
    [SerializeField] private GameObject oxFeedbackUI;
    [SerializeField] private GameObject[] oxFeedbackAnswer;
    [SerializeField] private TMP_Text oxFeedbackResult;
    [SerializeField] private TMP_Text oxFeedbackDescript;
    [SerializeField] private TMP_Text oxQIndex;
    [SerializeField] private TMP_Text oxAIndex;
    [SerializeField] private QuizSlot[] ox;
    [SerializeField] private Image quizImage;
    [SerializeField] private ButtonInteractor oxSubmitButton;
    [Space(5)]

    [Header("Feedback")]

    [Header("Parameters")]
    QuizJson quiz;
    private bool isSolved = false;
    private bool isHovered;
    [SerializeField] private bool isActivated;
    public int quizNumber = 0;
    [SerializeField] private int selectedNumber = 0;
    [Space(5)]

    private QuizSlot selectedSlot;
    public QuizSlot SelectedSlot
    {
        get
        {
            return selectedSlot;
        }
        set
        {
            if(selectedSlot != null)
            {
                selectedSlot.Deselect();
            }
            selectedSlot = value;
            selectedSlot.Select();
        }
    }

    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite finishSprite;

    [SerializeField] private Image signImage;

    public List<QuizObject> quizObjects = new List<QuizObject>();

    [SerializeField] private Transform userTransform; // 로컬 플레이어의 transform을 가져옴

    private void Start()
    {
        if (EventSyncronizerRoom.Instance)
        {
            eventMessage += EventSyncronizerRoom.Instance.OnSendMessage;
        }

        if (NetworkManager.Instance)
        {
            if (NetworkManager.User.userType == UserType.Lecture)
            {
                signImage.gameObject.SetActive(false);
            }
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

        signImage.gameObject.SetActive(true);
    }

    public void SetCurrentQuiz()
    {
        if(quizNumber == quizObjects.Count)
        {
            isSolved = true;
            selectionQuizUI.SetActive(false);
            oxQuizUI.SetActive(false);

            if (isSolved)
            {
                signImage.sprite = finishSprite;
            }
            else
            {
                signImage.sprite = readySprite;
            }

            signImage.gameObject.SetActive(true);
            return;
        }

        signImage.gameObject.SetActive(false);
        QuizObject quiz = quizObjects[quizNumber];

        switch (quiz.quizType)
        {
            case QuizType.Selection:

                oxQuizUI.SetActive(false);
                selectionQuizUI.SetActive(true);
                selectionQuestionUI.SetActive(true);

                selectionQIndex.text = $"Q{quiz.quizIndex}";
                selectionQuizText.text = quiz.contents;

                for(int i = 0; i < quiz.question.Length; i++)
                {
                    selections[i].SetText(quiz.question[i]);
                }

                break;
            case QuizType.OX:

                selectionQuizUI.SetActive(false);
                oxQuizUI.SetActive(true);
                oxQuestionUI.SetActive(true);

                oxQIndex.text = $"Q{quiz.quizIndex}";
                oxQuizText.text = quiz.contents;

                if(quiz.hasImage)
                {
                    quizImage.gameObject.SetActive(true);
                }
                else
                {
                    quizImage.gameObject.SetActive(false);
                }

                break;
        }
    }

    public void SelectQuizSlot(int slotNumber)
    {
        selectedNumber = slotNumber;
        QuizObject quiz = quizObjects[quizNumber];
        
        if(quiz.answer == slotNumber)
        {
            if (quiz.quizType == QuizType.Selection)
            {
                if (selectionResultPopUp != null)
                {
                    StopCoroutine(selectionResultPopUp);
                }
                selectionResultPopUp = StartCoroutine(SelectionResultPopUp(true));
            }
            else
            {
                if(oxResultPopUp != null)
                {
                    StopCoroutine(oxResultPopUp);
                }
                oxResultPopUp = StartCoroutine(OXResultPopUp(true));
            }
        }
        else
        {
            if (quiz.quizType == QuizType.Selection)
            {
                if (selectionResultPopUp != null)
                {
                    StopCoroutine(selectionResultPopUp);
                }
                selectionResultPopUp = StartCoroutine(SelectionResultPopUp(false));
            }
            else
            {
                if (oxResultPopUp != null)
                {
                    StopCoroutine(oxResultPopUp);
                }
                oxResultPopUp = StartCoroutine(OXResultPopUp(false));
            }
        }
    }

    private Coroutine selectionResultPopUp;
    private IEnumerator SelectionResultPopUp(bool isCollected)
    {
        QuizObject quiz = quizObjects[quizNumber];

        selectionQuestionUI.SetActive(false);
        selectionFeedbackUI.SetActive(true);

        selectionFeedbackAnswer.gameObject.SetActive(true);
        selectionAIndex.text = $"A{quiz.quizIndex}";
        selectionFeedbackAnswerText.text = quiz.question[quiz.answer];

        if (isCollected)
        {
            selectionFeedbackResult.text = "정답입니다.";
            selectionFeedbackDescript.gameObject.SetActive(false);
        }
        else
        {
            selectionFeedbackResult.text = "오답입니다.";
            selectionFeedbackDescript.gameObject.SetActive(true);
            selectionFeedbackDescript.text = $"[오답 풀이]\n";
        }

        yield return new WaitForSeconds(2f);
        selectionFeedbackUI.SetActive(false);

        quizNumber++;

        SetCurrentQuiz();
    }

    private Coroutine oxResultPopUp;
    private IEnumerator OXResultPopUp(bool isCollected)
    {
        QuizObject quiz = quizObjects[quizNumber];

        oxQuestionUI.SetActive(false);
        oxFeedbackUI.SetActive(true);

        oxFeedbackAnswer[quiz.answer].SetActive(true);
        oxAIndex.text = $"A{quiz.quizIndex}";

        if (isCollected)
        {
            oxFeedbackResult.text = "정답입니다.";
        }
        else
        {
            oxFeedbackResult.text = "오답입니다.";
            oxFeedbackDescript.gameObject.SetActive(true);
            oxFeedbackDescript.text = $"[오답 풀이]";
        }

        yield return new WaitForSeconds(2f);
        oxFeedbackAnswer[quiz.answer].SetActive(false);
        oxFeedbackUI.SetActive(false);

        quizNumber++;

        SetCurrentQuiz();
    }

    public void OnSelected()
    {
        if(quizNumber == quizObjects.Count)
        {
            return;
        }

        if(isHovered)
        {
            isActivated = false;
            SetCurrentQuiz();
        }
    }

    public void HasQuiz()
    {
        if(quizObjects.Count == 0)
        {
            gameObject.SetActive(false);
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


    public void CloseQuizWindow()
    {
        isSolved = true;

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
