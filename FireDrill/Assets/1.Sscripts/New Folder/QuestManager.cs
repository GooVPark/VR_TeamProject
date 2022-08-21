using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����Ʈ�� ���۵Ǹ� QuestScriptableObject���� QuestInstance�� ����
//QuestManager�� currentQuestList�� �߰� �� QuestScriptableObject�� ����Ʈ ���뿡 �´� �׼ǿ� ���
//�÷��̰� �ൿ�� �� ������ ��ϵ� ����Ʈ ����� �˻�

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
