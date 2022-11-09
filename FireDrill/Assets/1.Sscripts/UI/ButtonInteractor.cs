using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public enum ButtonState { Enable, Disable, Activate, Deactivate, HoverEnter, HoverExit }

public class ButtonInteractor : MonoBehaviour
{
    public delegate void ButtonEvent();
    public ButtonEvent onClick;

    public UnityEvent OnClick;

    //public Image image;
    //public TMP_Text text;

    public bool interactable = false;
    public bool isHovered = false;

    //[SerializeField] private ButtonState defaultState;

    //public Sprite disabledSprite;
    //public Sprite enabledSprite;
    //public Sprite hoverEnterSprite;
    //public Sprite hoverExitSprite;
    //public Sprite activateSprite;
    //public Sprite deactivateSprite;

    //[SerializeField] private ButtonState buttonState;

    private void Start()
    {
        //collider.size = new Vector2(image.rectTransform.rect.width / 100, image.rectTransform.rect.height / 100);
        //SetButtonSprite(defaultState);
    }

    public void OnSelect()
    {
        if (isHovered)
        {
            Debug.Log("On Select");
            onClick?.Invoke();
            OnClick?.Invoke();
        }
    }

    public void OnHoverEntered()
    {
        isHovered = true;
        //SetButtonSprite(ButtonState.HoverEnter);
    }

    public void OnHoverExited()
    {
        isHovered = false;
        //SetButtonSprite(ButtonState.HoverExit);
        
    }

    //public void SetButtonSprite(ButtonState state)
    //{
    //    Sprite sprite = null;
    //    switch (state)
    //    {
    //        case ButtonState.Enable:
    //            sprite = enabledSprite;
    //            buttonState = state;
    //            break;
    //        case ButtonState.Disable:
    //            sprite = disabledSprite;
    //            buttonState = state;
    //            break;
    //        case ButtonState.Activate:
    //            sprite = activateSprite;
    //            buttonState = state;
    //            break;
    //        case ButtonState.Deactivate:
    //            sprite = deactivateSprite;
    //            buttonState = state;
    //            break;
    //        case ButtonState.HoverEnter:
    //            sprite = hoverEnterSprite;
    //            buttonState = state;
    //            break;
    //        case ButtonState.HoverExit:
    //            SetButtonSprite(buttonState);
    //            break;
    //    }

    //    if (sprite == null)
    //    {

    //    }
    //    else
    //    {
    //        image.sprite = sprite;
    //    }
    //}
}
