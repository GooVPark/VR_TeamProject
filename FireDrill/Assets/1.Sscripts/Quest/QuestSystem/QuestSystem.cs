using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    #region Events

    public delegate void QuestRegisteredHandler(Quest newQuest);
    public event QuestRegisteredHandler onQuestRegistered;
    public event QuestRegisteredHandler onAchievementRegistered;

    public delegate void QuestCompletedHandler(Quest quest);
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCompletedHandler onAchievementCompleted;

    public delegate void QuestCanceledHandler(Quest quest);
    public event QuestCanceledHandler onQuestCanceled;

    #endregion

    private static QuestSystem instance;
    private static bool isApplicationQuitting;

    public static QuestSystem Instance
    {
        get
        {
            if(!isApplicationQuitting && instance == null)
            {
                instance = FindObjectOfType<QuestSystem>();
                if(instance == null)
                {
                    instance = new GameObject("Quest System").AddComponent<QuestSystem>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    private List<Quest> activeQuest = new List<Quest>();
    public IReadOnlyList<Quest> ActiveQuest => activeQuest;
    private List<Quest> completedQuest = new List<Quest>();
    public IReadOnlyList<Quest> CompletedQuest => completedQuest;

    private List<Quest> activeAchievements = new List<Quest>();
    public IReadOnlyList<Quest> ActiveAchievemvent => activeAchievements;
    private List<Quest> completedAchievment = new List<Quest>();
    public IReadOnlyList<Quest> CompletedAchievement => completedAchievment;

    private QuestDatabase questDatabase;
    private QuestDatabase achievementDatabase;

    private void Awake()
    {
        questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
        achievementDatabase = Resources.Load<QuestDatabase>("AchievementDatabase");
    }

    public void Register(Quest quest)
    {
        var newQuest = quest.Clone();
    }


    #region Callback

    private void OnQuestCompleted(Quest quest)
    {
        activeQuest.Remove(quest);
        completedQuest.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        activeQuest.Remove(quest);
        onQuestCanceled?.Invoke(quest);

        Destroy(quest, Time.deltaTime);
    }

    private void OnAchivementCompleted(Quest achievement)
    {
        activeAchievements.Remove(achievement);
        completedAchievment.Add(achievement);

        onAchievementCompleted?.Invoke(achievement);
    }

    #endregion
}
