using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XRVirtualKeyboard : MonoBehaviour
{
    public enum KeyBoardType
    {
        KOREAN,
        ENGLISH,
        NUMBER
    }

    public XRVirtualTextInputBox textInputBox;

    public UnityEngine.Events.UnityEvent OnClick;

    protected bool pressShift = false;
    public bool PressedShift => pressShift;
    protected bool capsLocked = false;
    public bool CapsLocked => capsLocked;

    public KeyBoardType defaultType = KeyBoardType.ENGLISH;
    public KeyBoardType keyBoardType = KeyBoardType.ENGLISH;

    public int maxTextCount;
    public bool disableOnReturn = false;

    private void Awake()
    {
        XRVirtualKey.keyBoard = this;
    }

    private void OnEnable()
    {
        keyBoardType = defaultType;
        textInputBox.Clear();
    }

    public void KeyDown(XRVirtualKey key)
    {
        if(textInputBox != null)
        {
            switch (key.type)
            {
                case XRVirtualKey.KeyType.CHARACTER:
                    if(textInputBox.TextField.Length <= maxTextCount)
                    {
                        char keyCharacter = key.keyCharacter;

                        if(pressShift)
                        {
                            keyCharacter = char.ToUpper(keyCharacter);
                            pressShift = false;
                        }

                        if(keyBoardType == KeyBoardType.KOREAN)
                        {
                            textInputBox.KeyDownHangul(keyCharacter);
                        }
                        else if(keyBoardType == KeyBoardType.ENGLISH)
                        {
                            if(capsLocked)
                            {
                                keyCharacter = char.ToUpper(keyCharacter);
                            }

                            textInputBox.KeyDown(keyCharacter);
                        }
                        else if(keyBoardType == KeyBoardType.NUMBER)
                        {
                            textInputBox.KeyDownNumber(keyCharacter);
                        }
                    }
                    break;
                case XRVirtualKey.KeyType.OTHER:
                    if(textInputBox.TextField.Length <= maxTextCount)
                    {
                        char keyCharacter = key.keyCharacter;
                        textInputBox.KeyDownNumber(keyCharacter);
                    }
                    break;
                case XRVirtualKey.KeyType.RETURN:
                    OnClick?.Invoke();
                    textInputBox.Clear();
                    if(disableOnReturn)
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                case XRVirtualKey.KeyType.SPACE:
                    if (textInputBox.TextField.Length <= maxTextCount)
                    {
                        textInputBox.KeyDown(key);
                    }
                    break;
                case XRVirtualKey.KeyType.BACKSPACE:
                    textInputBox.KeyDown(key);
                    break;
                case XRVirtualKey.KeyType.SHIFT:
                    pressShift = !pressShift;
                    break;
                case XRVirtualKey.KeyType.CAPSLOCK:
                    break;
                case XRVirtualKey.KeyType.TYPE:

                    int current = (int)keyBoardType;
                    keyBoardType = (KeyBoardType)((current + 1) % 3);

                    break;
            }
        }
    }
}
