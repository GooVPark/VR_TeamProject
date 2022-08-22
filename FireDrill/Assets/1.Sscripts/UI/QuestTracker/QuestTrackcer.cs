using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTrackcer : MonoBehaviour
{
    [SerializeField] private TMP_Text questTitleText;
    [SerializeField] private TaskDiscriptor taskDiscriptorPrefab;

    private Dictionary<Task, TaskDiscriptor> taskDiscriptorByTask = new Dictionary<Task, TaskDiscriptor>();

    private Quest targetQuest;

    private void OnDestroy()
    {
        if(targetQuest != null)
        {
            targetQuest.onNewTaskGroup -= UpdateTaskDiscriptors;
            targetQuest.onCompleted -= DestroySelf;
        }

        foreach(var tuple in taskDiscriptorByTask)
        {
            var task = tuple.Key;
            task.onSuccessChanged -= UpdateText;
        }
    }

    public void SetUp(Quest targetQuest, Color titleColor)
    {
        this.targetQuest = targetQuest;

        questTitleText.text = targetQuest.Category == null ? targetQuest.DisplayName : $"[{targetQuest.Category.DisplayName}] {targetQuest.DisplayName}";

        questTitleText.color = titleColor;

        targetQuest.onNewTaskGroup += UpdateTaskDiscriptors;
        targetQuest.onCompleted += DestroySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDiscriptors(targetQuest, taskGroups[0]);

        if (taskGroups[0] != targetQuest.CurrentTaskGroup)
        {
            for(int i = 1; i < taskGroups.Count; i++)
            {
                var taskGroup = taskGroups[i];
                UpdateTaskDiscriptors(targetQuest, taskGroup, taskGroups[i - 1]);

                if(taskGroup == targetQuest.CurrentTaskGroup)
                {
                    break;
                }
            }
        }
    }

    private void UpdateTaskDiscriptors(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach(var task in currentTaskGroup.Tasks)
        {
            var taskDiscriptor = Instantiate(taskDiscriptorPrefab, transform);
            taskDiscriptor.UpdateText(task);
            task.onSuccessChanged += UpdateText;

            taskDiscriptorByTask.Add(task, taskDiscriptor);
        }

        if(prevTaskGroup != null)
        {
            foreach (var task in prevTaskGroup.Tasks)
            {
                var taskDiscriptor = taskDiscriptorByTask[task];
                taskDiscriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }

    private void UpdateText(Task task, int currentSuccess, int prevSuccess)
    {
        taskDiscriptorByTask[task].UpdateText(task);
    }

    private void DestroySelf(Quest quest)
    {
        Destroy(gameObject);
    }
}
