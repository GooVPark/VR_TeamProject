using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
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
    [SerializeField] private Image oxSignImage;
    [SerializeField] private ButtonInteractor oxSubmitButton;
    [Space(5)]

    [Header("Feedback")]
    [SerializeField] private Sprite signO;
    [SerializeField] private Sprite signX;
    [SerializeField] private Image quizResult;
    [Space(5)]

    [Header("Parameters")]
    QuizJson quiz;
    public bool isSolved = false;
    private bool isHovered;
    [SerializeField] private bool isActivated;
    public int quizNumber = 0;
    [SerializeField] private int selectedNumber = 0;
    [SerializeField] private TMP_Text oxQuizCounts;
    [SerializeField] private TMP_Text selectionQuizCounts;
    private bool onQuizCounts = false;
    public Transform target;
    public bool interactable;
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
    [SerializeField] private XRSimpleInteractable[] selectionButtons;
    [SerializeField] private XRSimpleInteractable pannel;

    private void Start()
    {
        if (EventSyncronizerRoom.Instance)
        {
            eventMessage = null;
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

        if (quizObjects.Count > 0)
        {
            signImage.gameObject.SetActive(true);
        }

    }

    public void Initialize()
    {
        quizObjects.Clear();
        isSolved = false;
        quizNumber = 0;

        if (isSolved)
        {
            signImage.sprite = finishSprite;
        }
        else
        {
            signImage.sprite = readySprite;
        }

        if (quizObjects.Count > 0 && NetworkManager.User.userType == UserType.Student)
        {
            signImage.gameObject.SetActive(true);
        }

        
        
        selectionQuizUI.SetActive(false);
        oxQuizUI.SetActive(false);
        quizResult.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) < 5f)
            {
                foreach(var item in selectionButtons)
                {
                    item.enabled = true;
                }
                pannel.enabled = true;
            }
            else
            {
                foreach (var item in selectionButtons)
                {
                    item.enabled = false;
                }
                pannel.enabled = false;
            }
        }
    }

    public void SetCurrentQuiz()
    {
        if (quizNumber == quizObjects.Count)
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

        selectionQuestionUI.SetActive(false);
        selectionFeedbackUI.SetActive(false);
        oxQuestionUI.SetActive(false);
        oxFeedbackUI.SetActive(false);  

        QuizObject quiz = quizObjects[quizNumber];

        if (quizObjects.Count > 1)
        {
            onQuizCounts = true;
        }
        else
        {
            onQuizCounts = false;
        }


        switch (quiz.quizType)
        {
            case QuizType.Selection:

                oxQuizUI.SetActive(false);
                selectionQuizUI.SetActive(true);
                selectionQuestionUI.SetActive(true);

                selectionQIndex.text = $"Q{quiz.quizIndex + 1}";
                selectionQuizText.text = quiz.contents;

                for(int i = 0; i < quiz.question.Length; i++)
                {
                    selections[i].SetText(quiz.question[i]);
                }

                selectionQuizCounts.gameObject.SetActive(onQuizCounts);
                selectionQuizCounts.text = $"{quizNumber + 1}/{quizObjects.Count}";

                break;
            case QuizType.OX:

                selectionQuizUI.SetActive(false);
                oxQuizUI.SetActive(true);
                oxQuestionUI.SetActive(true);

                oxQIndex.text = $"Q{quiz.quizIndex + 1}";
                oxQuizText.text = quiz.contents;

                if(quiz.hasImage)
                {
                    quizImage.gameObject.SetActive(true);
                }
                else
                {
                    quizImage.gameObject.SetActive(false);
                }

                oxQuizCounts.gameObject.SetActive(onQuizCounts);
                oxQuizCounts.text = $"{quizNumber + 1}/{quizObjects.Count}";

                break;
        }

        for (int i = 0; i < selectionButtons.Length; i++)
        {
            selectionButtons[i].GetComponent<QuizSlot>().SetInteractable(false);
            selectionButtons[i].gameObject.GetComponent<QuizSlot>().Deselect();
        }
    }

    public void SelectQuizSlot(int slotNumber)
    {
        selectedNumber = slotNumber;
        QuizObject quiz = quizObjects[quizNumber];

        for(int i = 0; i < selectionButtons.Length; i++)
        {
            selectionButtons[i].GetComponent<QuizSlot>().SetInteractable(true);
        }

        bool isCollect = false;
        
        if(quiz.answer == slotNumber)
        {
            isCollect = true;
            if(resultSignPopUp != null)
            {
                StopCoroutine(resultSignPopUp);
            }
            resultSignPopUp = StartCoroutine(ResultSignPopUp(quiz.quizType, isCollect));

        }
        else
        {
            isCollect = false;
            if (resultSignPopUp != null)
            {
                StopCoroutine(resultSignPopUp);
            }
            resultSignPopUp = StartCoroutine(ResultSignPopUp(quiz.quizType, isCollect));
        }

    }

    private Coroutine resultSignPopUp;
    private IEnumerator ResultSignPopUp(QuizType quizType, bool isCollected)
    {
        quizResult.gameObject.SetActive(true);

        if (isCollected)
        {
            quizResult.sprite = signO;
        }
        else
        {
            quizResult.sprite = signX;
        }

        yield return new WaitForSeconds(2f);

        quizResult.gameObject.SetActive(false);

        QuizObject quiz = quizObjects[quizNumber];

        if (isCollected)
        {
            onSubmit?.Invoke(1);
            DataManager.Instance.SetQuizResult(NetworkManager.User.email, 1, quizObjects[quizNumber].quizIndex);
        }
        else
        {
            onSubmit?.Invoke(2);
            DataManager.Instance.SetQuizResult(NetworkManager.User.email, 2, quizObjects[quizNumber].quizIndex);
        }
        if (quiz.quizType == QuizType.Selection)
        {
            selections[quiz.answer].Select();
            if (selectionResultPopUp != null)
            {
                StopCoroutine(selectionResultPopUp);
            }
            selectionResultPopUp = StartCoroutine(SelectionResultPopUp(isCollected));
        }
        else
        {
            if (oxResultPopUp != null)
            {
                StopCoroutine(oxResultPopUp);
            }
            oxResultPopUp = StartCoroutine(OXResultPopUp(isCollected));
        }



        string message = $"{EventMessageType.QUIZ}";
        eventMessage?.Invoke(message);

    }

    private Coroutine selectionResultPopUp;
    private IEnumerator SelectionResultPopUp(bool isCollected)
    {
        QuizObject quiz = quizObjects[quizNumber];

        selectionQuestionUI.SetActive(false);
        selectionFeedbackUI.SetActive(true);

        selectionAIndex.text = $"A{quiz.quizIndex + 1}";
        //selectionFeedbackAnswerText.text = quiz.question[selectedNumber];

        if (isCollected)
        {
            selectionFeedbackResult.text = "정답입니다.";
            selectionFeedbackDescript.gameObject.SetActive(false);
        }
        else
        {
            selectionFeedbackResult.text = "오답입니다.";
            selectionFeedbackDescript.gameObject.SetActive(true);
            if(quiz.hasFixedAnswer)
            {
                selectionFeedbackDescript.text = $"[오답 풀이]\n{quiz.fixedAnswer[0]}";
            }
            else
            {
                selectionFeedbackDescript.text = $"[오답 풀이]\n{quiz.fixedAnswer[selectedNumber]}";
            }

            selections[selectedNumber].OnCheckSign();
        }

        yield return new WaitForSeconds(5f);

        selectionFeedbackUI.SetActive(false);
        selectionQuizUI.SetActive(false);

        quizNumber++;

        SetCurrentQuiz();
    }

    private Coroutine oxResultPopUp;
    private IEnumerator OXResultPopUp(bool isCollected)
    {
        QuizObject quiz = quizObjects[quizNumber];

        oxQuestionUI.SetActive(false);
        oxFeedbackUI.SetActive(true);

        //oxFeedbackAnswer[selectedNumber].SetActive(true);
        oxAIndex.text = $"A{quiz.quizIndex + 1}";

        if (isCollected)
        {
            oxFeedbackResult.text = "정답입니다.";
            oxFeedbackDescript.gameObject.SetActive(false);
        }
        else
        {
            quizImage.gameObject.SetActive(false);
            oxFeedbackResult.text = "오답입니다.";
            oxFeedbackDescript.gameObject.SetActive(true);
            oxFeedbackDescript.text = $"[오답 풀이]\n{quiz.fixedAnswer[0]}";

            selections[selectedNumber].OnCheckSign();
        }

        yield return new WaitForSeconds(5f);

        oxFeedbackUI.SetActive(false);
        oxQuizUI.SetActive(false);

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
