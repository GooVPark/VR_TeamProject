using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XRVirtualKey : MonoBehaviour
{
    public static XRVirtualKeyboard keyBoard = null;
    public enum KeyType
    {
        CHARACTER,
        OTHER,
        RETURN,
        SPACE,
        BACKSPACE,
        SHIFT,
        CAPSLOCK,
        TYPE
    }

    [SerializeField] private Image textImage;
    
    [SerializeField] private Sprite shiftedTextSpriteKR;
    [SerializeField] private Sprite textSpriteKR;

    [SerializeField] private Sprite shiftedTextSpriteEN;
    [SerializeField] private Sprite textSpriteEN;

    [SerializeField] private Sprite textSpriteNUM;

    [SerializeField] private Sprite shiftOn;
    [SerializeField] private Sprite shiftOff;

    public char keyCharacter;
    public KeyType type = KeyType.CHARACTER;

    private bool isHover = false;

    private void Update()
    {
        switch (type)
        {
            case KeyType.CHARACTER:
                switch (keyBoard.keyBoardType)
                {
                    case XRVirtualKeyboard.KeyBoardType.KOREAN:

                        if (keyBoard.PressedShift)
                        {
                            if (textImage != null)
                            {
                                if (shiftedTextSpriteKR != null)
                                {
                                    textImage.sprite = shiftedTextSpriteKR;
                                }
                            }
                        }
                        else
                        {
                            if (textImage != null)
                            {
                                textImage.sprite = textSpriteKR;
                            }
                        }

                        break;

                    case XRVirtualKeyboard.KeyBoardType.ENGLISH:

                        if (keyBoard.CapsLocked || keyBoard.PressedShift)
                        {
                            textImage.sprite = shiftedTextSpriteEN;
                        }
                        else
                        {
                            textImage.sprite = textSpriteEN;
                        }

                        break;

                    case XRVirtualKeyboard.KeyBoardType.NUMBER:

                        textImage.sprite = textSpriteNUM;

                        break;
                }
                break;
            case KeyType.OTHER:
                break;
            case KeyType.RETURN:
                break;
            case KeyType.SPACE:
                break;
            case KeyType.BACKSPACE:
                break;
            case KeyType.SHIFT:

                if(keyBoard.PressedShift)
                {
                    if(textImage != null)
                    {
                        textImage.sprite = shiftOn;
                    }
                }
                else
                {
                    if (textImage != null)
                    {
                        textImage.sprite = shiftOff;
                    }
                }

                break;

            case KeyType.CAPSLOCK:

                if (keyBoard.CapsLocked)
                {
                    if (textImage != null)
                    {
                        textImage.sprite = shiftOn;
                    }
                }
                else
                {
                    if (textImage != null)
                    {
                        textImage.sprite = shiftOff;
                    }
                }

                break;
        }
    }

    public void OnHoverEntered()
    {
        isHover = true;
    }

    public void OnHoverExited()
    {
        isHover = false;
    }

    public void OnSelect()
    {
        if(!isHover)
        {
            return;
        }

        keyBoard.KeyDown(this);
    }
}
