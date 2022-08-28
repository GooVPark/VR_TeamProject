using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HUDController : MonoBehaviour
{
    [Header("Chat")]
    public GameObject virtualKeyboard;
    public GameObject chatUI;
    private bool onChatView;

    public InputField chatInputField;
    public TMP_Text[] chatList;

    private void Awake()
    {
        InputManager.leftSecondaryButton += ShowTextChatWindow;
        NetworkManager.ChatCallback += UpdateChat;

        for (int i = 0; i < chatList.Length; i++)
        {
            chatList[i].text = "";
        }

    }

    public void ShowTextChatWindow()
    {
        if(onChatView)
        {
            virtualKeyboard.SetActive(false);
            chatUI.SetActive(false);
            
            onChatView = false;
        }
        else
        {
            virtualKeyboard.SetActive(true);
            chatUI.SetActive(true);

            onChatView = true;
        }
    }

    public void SendChatMessage()
    {
        NetworkManager.Instance.SendChat(chatInputField.text);
        chatInputField.text = "";
    }

    public void UpdateChat(string msg)
    {
        bool isInput = false;

        for (int i = 0; i < chatList.Length; i++)
        {
            if (chatList[i].text == "")
            {
                isInput = true;
                chatList[i].text = msg;
                break;
            }
        }

        if (!isInput)
        {
            for (int i = 1; i < chatList.Length; i++)
            {
                chatList[i - 1].text = chatList[i].text;
            }

            chatList[chatList.Length - 1].text = msg;
        }
    }
}
