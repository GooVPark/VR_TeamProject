using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//퀘스트가 시작되면 QuestScriptableObject에서 QuestInstance를 생성
//QuestManager의 currentQuestList에 추가 및 QuestScriptableObject의 퀘스트 내용에 맞는 액션에 등록
//플레이가 행동을 할 때마다 등록된 퀘스트 내용들 검사

public class QuestManager : MonoBehaviour
{
    private List<string> questID = new List<string>();
    private List<QuestInstance> currentQuestList = new List<QuestInstance>();

    private Dictionary<string, QuestInstance> questDataTable = new Dictionary<string, QuestInstance>();

    public void StartQuest(QuestData questData, PlayerController player)
    {
        currentQuestList.Add(new QuestInstance(questData));
        player.PlayerQuestUpdate += currentQuestList[^1].OnQuestUpdate;
        //ui update
    }

    public void EndQuest(QuestData questData, PlayerController player)
    {
        if(questData.nextQuest != null)
        {
            StartQuest(questData.nextQuest, player);
        }
    }
}
