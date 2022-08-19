using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour
{
    public delegate void PlayerQuestEvent(QuestObject target, QuestContentType type);
    public PlayerQuestEvent PlayerQuestUpdate;


    [Header("Quest Menu")]
    public QuestManager questManager;
    public QuestData questData;
    
    private void Start()
    {
        questManager.StartQuest(questData, this);
    }

    public void OnSelect(SelectEnterEventArgs arg)
    {
        QuestObject target = arg.interactableObject.transform.GetComponentInChildren<QuestObject>();
        PlayerQuestUpdate(target, QuestContentType.Get);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("QuestArea"))
        {
            QuestObject target = other.GetComponent<QuestObject>();
            PlayerQuestUpdate(target, QuestContentType.Move);
        }
    }
}
