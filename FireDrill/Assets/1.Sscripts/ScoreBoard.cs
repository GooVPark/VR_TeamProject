using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;
using UnityEngine.UI;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    public RoomSceneManager roomManager;
    public bool isOrderd = false;
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;
    [SerializeField] private Image image;
    [SerializeField] private ScoreUI[] scoureRows;

    private float elapsedTime = 0f;
    private float interval = 0.5f;

    private void Start()
    {
   
    }

    private void Update()
    {
        if(elapsedTime > interval)
        {
            UpdateScoreBoard();
            elapsedTime = 0f;
        }
        elapsedTime += Time.deltaTime;
    }

    public void ListOrder()
    {
        if(isOrderd)
        {
            isOrderd = false;
            image.sprite = off;
        }
        else
        {
            isOrderd = true;
            image.sprite = on;
        }

        UpdateScoreBoard();
        elapsedTime = 0f;
    }

    private void UpdateScoreBoard()
    {
        List<User> users = roomManager.GetUsersInRoom(roomManager.roomNumber);
        
        if(isOrderd)
        {
            var list = from user in users orderby user.totalScore descending select users;
            users = (List<User>)list;
        }

        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].userType == UserType.Lecture) continue;

            scoureRows[i].gameObject.SetActive(true);
            scoureRows[i].UpdateScore(users[i]);
        }
        for (int i = users.Count; i < scoureRows.Length; i++)
        {
            scoureRows[i].gameObject.SetActive(false);
        }
    }
}
