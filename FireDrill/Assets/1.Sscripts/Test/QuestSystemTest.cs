using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystemTest : MonoBehaviour
{
    [SerializeField] private Quest quest;
    [SerializeField] private Category category;
    [SerializeField] private TaskTarget target;

    // Start is called before the first frame update
    void Start()
    {
        var questSystem = QuestSystem.Instance;

        questSystem.onQuestRegistered += (quest) =>
        {
            print($"New Quest: {quest.CodeName} Registered");
            print($"Active Quests Count: {questSystem.ActiveQuest.Count}");
        };

        questSystem.onQuestCompleted += (quest) =>
        {
            print($"Quest: {quest.CodeName} Completed");
            print($"Completed Quests Count: {questSystem.CompletedQuest}");
        };

        var newQuest = questSystem.Register(quest);
        newQuest.onTaskSuccessChanged += (quest, task, currentSuccess, prevSuccess) =>
        {
            print($"Quest: {quest.CodeName}, Task: {task.CodeName}, CurrentSuccesses: {currentSuccess}");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
