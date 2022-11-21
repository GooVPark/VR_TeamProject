using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using ExitGames.Client.Photon;

public class TextChatManager : MonoBehaviour
{
    public delegate void EventMessage(string message);
    public EventMessage eventMessage;

    public delegate void SendMessageEvent(string message);
    public SendMessageEvent sendChatMessage;

    public TMP_Text[] chatList;

    private int colorNumber = 0;
    [SerializeField] private List<Color> colorList = new List<Color>();
    private void Start()
    {
        colorList.Add(Color.red);
        colorList.Add(Color.green);
        colorList.Add(Color.blue);
        //colorList.Add(Color.white);
        colorList.Add(Color.black);
        colorList.Add(Color.yellow);
        colorList.Add(Color.cyan);
        colorList.Add(Color.magenta);
        colorList.Add(Color.gray);
        colorList.Add(Color.grey);

        colorNumber = NetworkManager.User.characterNumber;
    }

    public void SendChatMessage(TMP_InputField inputField)
    {
        if(string.IsNullOrEmpty(inputField.text))
        {
            return;
        }

        string message = $"{EventMessageType.TEXTCHAT}_{NetworkManager.User.email}_{NetworkManager.User.name}_{inputField.text}={colorNumber}_{NetworkManager.RoomNumber}";
        if(eventMessage == null)
        {
            EventSyncronizer eventSyncronizer = FindObjectOfType<EventSyncronizer>();
            if(eventSyncronizer != null)
            {
                eventMessage += eventSyncronizer.OnSendMessage;
            }
            else
            {
                EventSyncronizerRoom eventSyncronizerRoom = FindObjectOfType<EventSyncronizerRoom>();
                eventMessage += eventSyncronizerRoom.OnSendMessage;
            }
        }
        eventMessage?.Invoke(message);
        sendChatMessage?.Invoke(inputField.text);
    }
    public static string ColorToStr(Color color)
    {
        string r = ((int)(color.r * 255)).ToString("X2");
        string g = ((int)(color.g * 255)).ToString("X2");
        string b = ((int)(color.b * 255)).ToString("X2");
        string a = ((int)(color.a * 255)).ToString("X2");


        string result = string.Format("{0}{1}{2}{3}", r, g, b, a);


        return result;
    }

    public void OnGetMessage(string senders, string message, int roomNumber)
    {
        if (NetworkManager.RoomNumber == roomNumber)
        {
            string[] command = message.Split('=');
            string chatMessage = $"<color=#{ColorToStr(colorList[int.Parse(command[1]) % colorList.Count])}><b>{senders}</b>\n{command[0]}</color>\n";
            chatList[0].text += chatMessage;
        }
    }

}
