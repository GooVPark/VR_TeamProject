using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    #region SavePass

    private const string KSaveRootPath = "questSystem";
    private const string KActiveQuestsSavePath = "activeQuests";
    private const string KCompletedQuestSavePath = "completedQuests";
    private const string KActiveAchievementSavePath = "activeAchievements";
    private const string KCompletedAchivementSavePath = "completedAchievements";

    #endregion
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
            if (!isApplicationQuitting && instance == null)
            {
                instance = FindObjectOfType<QuestSystem>();
                if (instance == null)
                {
                    instance = new GameObject("Quest System").AddComponent<QuestSystem>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    private List<Quest> activeQuests = new List<Quest>();
    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    private List<Quest> completedQuest = new List<Quest>();
    public IReadOnlyList<Quest> CompletedQuest => completedQuest;

    private List<Quest> activeAchievements = new List<Quest>();
    public IReadOnlyList<Quest> ActiveAchievemvent => activeAchievements;
    private List<Quest> completedAchievments = new List<Quest>();
    public IReadOnlyList<Quest> CompletedAchievement => completedAchievments;

    private QuestDatabase questDatabase;
    private QuestDatabase achievementDatabase;

    private void Awake()
    {
        questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
        achievementDatabase = Resources.Load<QuestDatabase>("AchievementDatabase");

        if (!Load())
        {
            foreach (var achievement in achievementDatabase.Quests)
            {
                Register(achievement);
            }
        }
    }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    public Quest Register(Quest quest)
    {
        var newQuest = quest.Clone();

        if (newQuest is Achievement)
        {
            newQuest.onCompleted += OnAchivementCompleted;
            activeAchievements.Add(newQuest);

            newQuest.OnRegister();
            onAchievementRegistered?.Invoke(newQuest);

        }
        else
        {
            newQuest.onCompleted += OnQuestCompleted;
            newQuest.onCanceled += OnQuestCanceled;

            activeQuests.Add(newQuest);

            newQuest.OnRegister();
            onQuestRegistered?.Invoke(newQuest);
        }

        return newQuest;
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(activeQuests, category, target, successCount);
        ReceiveReport(activeAchievements, category, target, successCount);
    }

    public void ReceiveReport(Category category, TaskTarget target, int successCount) => ReceiveReport(category.CodeName, target.Value, successCount);

    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        foreach (var quest in quests.ToArray()) //ToArray로 돌리면 반복문 도는 중에 내용이 바뀌어도 애러가 뜨지 않음
        {
            quest.ReceiveReport(category, target, successCount);
        }
    }

    public void CompleteWaitingQuest()
    {
        foreach (var quest in activeQuests.ToList())
        {
            if (quest.IsCompletable)
            {
                quest.Complete();
            }
        }
    }

    public bool ContainsInActiveQuest(Quest quest) => activeQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompletedQuest(Quest quest) => completedQuest.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInActiveAchievement(Quest quest) => activeAchievements.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompletedAchievement(Quest quest) => completedAchievments.Any(x => x.CodeName == quest.CodeName);

    public void Save()
    {
        var root = new JObject();
        root.Add(KActiveQuestsSavePath, CreateSaveDatas(activeQuests));
        root.Add(KCompletedQuestSavePath, CreateSaveDatas(completedQuest));
        root.Add(KActiveAchievementSavePath, CreateSaveDatas(activeAchievements));
        root.Add(KCompletedAchivementSavePath, CreateSaveDatas(completedAchievments));

        PlayerPrefs.SetString("questSystem", root.ToString());
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        if (PlayerPrefs.HasKey(KSaveRootPath))
        {
            var root = JObject.Parse(PlayerPrefs.GetString(KSaveRootPath));

            LoadSaveDatas(root[KActiveQuestsSavePath], questDatabase, LoadActiveQuest);
            LoadSaveDatas(root[KCompletedAchivementSavePath], questDatabase, LoadCompletedQuest);

            LoadSaveDatas(root[KActiveAchievementSavePath], achievementDatabase, LoadActiveQuest);
            LoadSaveDatas(root[KCompletedAchivementSavePath], achievementDatabase, LoadCompletedQuest);

            return true;
        }

        return false;
    }

    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();
        foreach (var quest in quests)
        {
            if (quest.IsSavable)
            {
                saveDatas.Add(JObject.FromObject(quest.ToSaveData()));
            }
        }

        return saveDatas;
    }

    private void LoadSaveDatas(JToken datasToken, QuestDatabase database, System.Action<QuestSaveData, Quest> onSuccess)
    {
        var datas = datasToken as JArray;
        foreach (var data in datas)
        {
            var saveData = data.ToObject<QuestSaveData>();
            var quest = database.FindQuestBy(saveData.codeName);
            onSuccess.Invoke(saveData, quest);
        }
    }

    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        if (newQuest is Achievement)
        {
            completedAchievments.Add(newQuest);
        }
        else
        {
            completedQuest.Add(newQuest);
        }
    }


    #region Callback

    private void OnQuestCompleted(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuest.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        activeQuests.Remove(quest);
        onQuestCanceled?.Invoke(quest);

        Destroy(quest, Time.deltaTime);
    }

    private void OnAchivementCompleted(Quest achievement)
    {
        activeAchievements.Remove(achievement);
        completedAchievments.Add(achievement);

        onAchievementCompleted?.Invoke(achievement);
    }

    #endregion
}
