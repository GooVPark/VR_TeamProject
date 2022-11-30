using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 UI 버튼의 상태 관리
/// </summary>
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

    //NetworkManager의 버튼 파라미터에서 값을 받아와서 상태를 바꿔줌
    private void UpdateState(bool isDisabled, bool state)
    {
        //Disalbe되면 버튼을 누를 수 없는 상태
        //이후 Activate/Diactivate 토글
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