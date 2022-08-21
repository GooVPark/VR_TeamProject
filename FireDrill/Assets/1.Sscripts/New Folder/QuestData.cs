using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestContentType
{
    Get,
    Move,
    Bring
}

[CreateAssetMenu(menuName = "Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    public int requiredContentCount;

    public List<QuestContent> contents;

    public QuestData prevQuest;
    public QuestData nextQuest;
}

[System.Serializable]
public class QuestContent
{
    public QuestContentType type;
    public string contentName;
    public int maxCount;
    public int currentCount;
    public string targetID;
    public bool isCompleted;

    public QuestContent(QuestContent content)
    {
        this.contentName = content.contentName;
        this.type = content.type;
        this.maxCount = content.maxCount;
        this.targetID = content.targetID;
        this.isCompleted = content.isCompleted;
    }   
}

public class QuestInstance
{
    private string questName;
    private int currentContentCount;
    private int requiredContentCount;
    private List<QuestContent> contents;
    private bool isKeepObject;

    private QuestData prevQuest;
    private QuestData nextQuest;

    public QuestInstance(QuestData questData)
    {
        prevQuest = questData.prevQuest;
        nextQuest = questData.nextQuest;

        questName = questData.questName;
        requiredContentCount = questData.requiredContentCount;

        contents = new List<QuestContent>();
        for(int i = 0; i < questData.contents.Count; i++)
        {
            contents.Add(new QuestContent(questData.contents[i]));
        }

        Debug.Log($"{questName} 시작!");
    }

    public void OnQuestUpdate(QuestObject target, QuestContentType type)
    {
        foreach(QuestContent content in contents)
        {
            if(content.isCompleted == false && content.targetID == target.id && content.type == type)
            {
                Debug.Log($"{questName} 진행 중! - {content.contentName} ({content.currentCount} / {content.maxCount})");
                if(content.currentCount >= content.maxCount)
                {
                    Debug.Log($"{questName} - {content.contentName} 달성!");
                    content.isCompleted = true;
                    currentContentCount++;
                }
            }

            if (currentContentCount >= requiredContentCount)
            {
                Debug.Log($"{questName} 완료!");
            }
        }
    }

    public void IsKeepObject(bool value)
    {
        isKeepObject = value;
    }
}

