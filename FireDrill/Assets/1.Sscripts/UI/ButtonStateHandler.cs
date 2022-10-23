using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStateHandler : MonoBehaviour
{
    public enum UIButtonType { Megaphone, Score, VoiceChat, TextChat }
    public UIButtonType type;
    public Sprite disable;
    public Sprite activate;
    public Sprite deactivate;

    private SpriteRenderer spriteRenderer;
    public Image image;

    [SerializeField]private  ButtonState buttonState;

    public ButtonInteractor button;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        button = gameObject.GetComponent<ButtonInteractor>();
        //button.OnClick.AddListener(() => OnSelect());

        UpdateState(buttonState);

    }

    private void Update()
    {
        switch (type)
        {
            case UIButtonType.Megaphone:
                UpdateState(NetworkManager.Instance.megaphoneDisabled, NetworkManager.Instance.onMegaphone);
                break;
            case UIButtonType.Score:
                UpdateState(NetworkManager.Instance.scoreBoardDisabled, NetworkManager.Instance.onScoreBoard);
                break;
            case UIButtonType.VoiceChat:
                UpdateState(NetworkManager.Instance.voiceChatDisabled, NetworkManager.Instance.onVoiceChat);
                break;
            case UIButtonType.TextChat:
                UpdateState(NetworkManager.Instance.textChatDisabled, NetworkManager.Instance.onTextChat);
                break;
        }
    }

    private void UpdateState(bool isDisabled, bool state)
    {
        if (isDisabled)
        {
            image.sprite = disable;
            return;
        }
        if (state)
        {
            image.sprite = activate;
        }
        else
        {
            image.sprite = deactivate;
        }
    }

    public void UpdateState(ButtonState buttonState)
    {
        Sprite sprite = disable;
        switch (buttonState)
        {
            case ButtonState.Disable:
                sprite = disable;
                break;
            case ButtonState.Activate:
                sprite = activate;
                break;
            case ButtonState.Deactivate:
                sprite = deactivate;
                break;
        }

        image.sprite = sprite;
    }

    public void OnSelect()
    {
        switch (buttonState)
        {
            case ButtonState.Disable:
                return;
            case ButtonState.Activate:

                buttonState = ButtonState.Deactivate;
                image.sprite = deactivate;


                break;
            case ButtonState.Deactivate:

                buttonState = ButtonState.Activate;
                image.sprite = activate;


                break;
        }
    }

}