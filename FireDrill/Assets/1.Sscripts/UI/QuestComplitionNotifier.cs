using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class QuestComplitionNotifier : MonoBehaviour
{
    [SerializeField] private string titleDiscription;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private float showTime = 3f;

    private Queue<Quest> reservedQuests = new Queue<Quest>();
    private StringBuilder stringBuilder = new StringBuilder();

    private void Start()
    {
        var questSystem = QuestSystem.Instance;
        questSystem.onQuestCompleted += Notify;
        questSystem.onAchievementCompleted += Notify;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;
        if(questSystem != null)
        {
            questSystem.onQuestCompleted -= Notify;
            questSystem.onAchievementCompleted -= Notify;
        }
    }

    private void Notify(Quest quest)
    {
        reservedQuests.Enqueue(quest);

        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowNotice());
        }
    }

    private IEnumerator ShowNotice()
    {
        var waitSeconds = new WaitForSeconds(showTime);

        Quest quest;

        while(reservedQuests.TryDequeue(out quest))
        {
            titleText.text = titleDiscription.Replace("%{dn}", quest.DisplayName);

            foreach(var reward in quest.Rewards)
            {
                stringBuilder.Append(reward.Discription);
                stringBuilder.Append(" ");
                stringBuilder.Append(reward.Quantity);
                stringBuilder.Append(" ");
            }

            rewardText.text = stringBuilder.ToString();
            stringBuilder.Clear();

            yield return waitSeconds;
        }

        gameObject.SetActive(false);
    }
}
