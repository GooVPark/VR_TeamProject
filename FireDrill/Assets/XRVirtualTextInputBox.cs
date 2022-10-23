using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XRVirtualTextInputBox : MonoBehaviour
{
    AutomateKR automateKR = new AutomateKR();

    [SerializeField] protected TMP_InputField textField;

    public string TextField
    {
        get
        {
            if (textField != null)
            {
                return textField.text;
            }
            return "";
        }
        set
        {
            if (textField != null)
            {
                textField.text = value;
            }
        }
    }

    public void Clear()
    {
        automateKR.Clear();
        TextField = automateKR.completeText + automateKR.ingWord;
    }

    public void KeyDownHangul(char key)
    {
        automateKR.SetKeyCode(key);

        TextField = automateKR.completeText + automateKR.ingWord;
    }

    public void KeyDownNumber(char key)
    {
        automateKR.SetKeyStringNumber(key);

        TextField = automateKR.completeText + automateKR.ingWord;
    }

    public void KeyDown(char key)
    {
        automateKR.SetKeyString(key);

        TextField = automateKR.completeText + automateKR.ingWord;
    }

    public void KeyDown(XRVirtualKey _key)
    {
        switch (_key.type)
        {
            case XRVirtualKey.KeyType.BACKSPACE:
                {
                    automateKR.SetKeyCode(AutomateKR.KEY_CODE_BACKSPACE);

                }
                break;
            case XRVirtualKey.KeyType.SPACE:
                {
                    automateKR.SetKeyCode(AutomateKR.KEY_CODE_SPACE);
                }
                break;
        }

        TextField = automateKR.completeText + automateKR.ingWord;
    }
}
