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

    public ButtonStateManager[] cameraUIIcons;

    private void Awake()
    {
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

}
