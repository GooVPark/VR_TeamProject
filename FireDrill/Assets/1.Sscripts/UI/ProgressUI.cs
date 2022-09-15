using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNumberText;
    [SerializeField] private TMP_Text companyNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private List<GameObject> progressMarks = new List<GameObject>();

    [SerializeField] private GameObject roomEnabled;
    [SerializeField] private GameObject roomDisabled;


    public void UpdateProgressUI(RoomData roomData)
    {
        if(roomData == null)
        {
            roomEnabled.SetActive(false);
            roomDisabled.SetActive(true);
            return;
        }

        roomEnabled.SetActive(true);
        roomDisabled.SetActive(false);

        roomNumberText.text = roomData.roomNumber.ToString();
        companyNameText.text = roomData.company;
        playerCountText.text = $"{NetworkManager.Instance.GetPlayerCount(roomData.roomNumber)}/{roomData.maxPlayerCount}";

        for(int i = 0; i < roomData.progress; i++)
        {
            progressMarks[i].SetActive(true);
        }

        for (int i = roomData.progress; i < 6; i++)
        {
            progressMarks[i].SetActive(false);
        }
    }
}