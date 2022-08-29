using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HUDController : MonoBehaviour
{
    enum CameraUI { MegaPhone, ScoreBoard, VoiceChat, TextChat }
    [Header("Chat")]
    public GameObject virtualKeyboard;
    public GameObject chatUI;
    private bool onChatView;

    public InputField chatInputField;
    public TMP_Text[] chatList;

    public ButtonState[] cameraUIIcons;

    private void Awake()
    {
        NetworkManager.ChatCallback += UpdateChat;

        for (int i = 0; i < chatList.Length; i++)
        {
            chatList[i].text = "";
        }
        chatInputField.text = "";
    }

    #region Camera UI

    public void SetCameraUI()
    {
        User user = NetworkManager.User;

        if(user.userType == UserType.Lecture)
        {
            cameraUIIcons[0].gameObject.SetActive(true);
            cameraUIIcons[1].gameObject.SetActive(true);
        }

        cameraUIIcons[2].gameObject.SetActive(true);
        cameraUIIcons[3].gameObject.SetActive(true);
    }

    #endregion

    #region Chat

    public delegate void ShowSpeechBubbleEvent(string text);
    public ShowSpeechBubbleEvent showSpeechBubble;

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

        showSpeechBubble?.Invoke(msg);
    }

    #endregion
}
