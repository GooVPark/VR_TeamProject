using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonState { Disable, Activate, Deactivate }

public class ButtonStateHandler : MonoBehaviour
{
    public Sprite disable;
    public Sprite activate;
    public Sprite deactivate;

    private SpriteRenderer spriteRenderer;

    [SerializeField]private  ButtonState buttonState;

    public ButtonInteractor button;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        button = gameObject.GetComponent<ButtonInteractor>();
        button.onClick += OnSelect;

        UpdateState(buttonState);

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

        spriteRenderer.sprite = sprite;
    }

    public void OnSelect()
    {
        switch (buttonState)
        {
            case ButtonState.Disable:
                return;
            case ButtonState.Activate:

                buttonState = ButtonState.Deactivate;
                spriteRenderer.sprite = deactivate;

                break;
            case ButtonState.Deactivate:

                buttonState = ButtonState.Activate;
                spriteRenderer.sprite = activate;

                break;
        }
    }
    
}