using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

using Debug = UnityEngine.Debug;
public enum QuestState
{
    Inactive,
    Running,
    Complete,
    Cancel,
    WaitingForCompletion
}

[CreateAssetMenu(menuName = "Quest/Quest", fileName = "Quest")]
public class Quest : ScriptableObject
{
    #region Events

    public delegate void TaskSuccessChangedHandler(Quest quest, Task task, int currentSuccess, int prevSuccess);
    public TaskSuccessChangedHandler onTaskSuccessChanged;

    public delegate void CompletedHandler(Quest quest);
    public CompletedHandler onCompleted;

    public delegate void CanceledHandler(Quest quest);
    public CanceledHandler onCanceled;

    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup);
    public NewTaskGroupHandler onNewTaskGroup;

    #endregion
    [SerializeField] private Category category;
    public Category Category => category;
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    [Header("Task")]
    [SerializeField] private TaskGroup[] taskGroups;
    public IReadOnlyList<TaskGroup> TaskGroups => taskGroups;

    [Header("Reward")]
    [SerializeField] private Reward[] rewards;
    public IReadOnlyList<Reward> Rewards => rewards;

    private int currentTaskGroupIndex;
    public TaskGroup CurrentTaskGroup => taskGroups[currentTaskGroupIndex];
    [Header("Text")]
    [SerializeField] private string codeName;
    public string CodeName => codeName;

    [SerializeField] private string displayName;
    public string DisplayName => displayName;

    [SerializeField] private string discription;

    [Header("Option")]
    [SerializeField] private bool useAutoComplete;

    [SerializeField] private bool isCancelable;
    public virtual bool IsCancelable => isCancelable && cancelConditions.All(x => x.IsPass(this));

    [Header("Condition")]
    [SerializeField] private Condition[] acceptionConditions;
    public bool IsAcceptable => acceptionConditions.All(x => x.IsPass(this));

    [SerializeField] private Condition[] cancelConditions;

    public QuestState State { get; private set; }

    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsCompletable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCanceled => State == QuestState.Cancel;

    public void OnRegister()
    {
        Debug.Assert(!IsRegistered, "This quest has already been registered.");

        foreach(var taskGroup in taskGroups)
        {
            taskGroup.SetUp(this);
            foreach(var task in taskGroup.Tasks)
            {
                task.onSuccessChanged += OnSuccessChanged;
            }
        }

        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        Debug.Assert(IsRegistered, "This quest has already been registered.");
        Debug.Assert(!IsCanceled, "This quest has been canceled.");

        if(IsComplete)
        {
            return;
        }

        if(CurrentTaskGroup.IsAllTaskComplete)
        {
            if (currentTaskGroupIndex + 1 == taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;
                if(useAutoComplete)
                {
                    Complete();
                }
            }
            else
            {
                var prevTaskGroup = taskGroups[currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        else
        {
            State = QuestState.Running;
        }
    }

    public void Complete()
    {
        CheckIsRunning();

        foreach(var taskGroup in taskGroups)
        {
            taskGroup.Complete();
        }

        State = QuestState.Complete;

        foreach(var reward in rewards)
        {
            reward.Give(this);
        }

        onCompleted?.Invoke(this);

        onTaskSuccessChanged = null;
        onCompleted = null;
        onCanceled = null;
        onNewTaskGroup = null;
    }

    public virtual void Cancel()
    {
        CheckIsRunning();
        Debug.Assert(IsCancelable, "This quest can't be canceld");

        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }

    public Quest Clone()
    {
        var clone = Instantiate(this);
        clone.taskGroups = taskGroups.Select(x => new TaskGroup(x)).ToArray();
        
        return clone;
    }

    private void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess) => onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);

    [Conditional("UNITY_EDITOR")]
    private void CheckIsRunning()
    {
        Debug.Assert(IsRegistered, "This quest has already been registered.");
        Debug.Assert(!IsCanceled, "This quest has been canceled.");
        Debug.Assert(!IsCompletable, "This quest has already been completed.");
    }
}
