using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextInputManager : MonoBehaviour
{
    public XRVirtualKeyboard virtualKeyboard;
    [SerializeField] private TMP_InputField input;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ShowVirtualKeyboard(TMP_InputField tmp)
    {
        virtualKeyboard.gameObject.SetActive(true);
        if (tmp != null)
        {
            input = tmp;
        }
    }

    public void Submit()
    {
        input.text = virtualKeyboard.textInputBox.TextField;    
        virtualKeyboard.gameObject.SetActive(false);
    }
}
