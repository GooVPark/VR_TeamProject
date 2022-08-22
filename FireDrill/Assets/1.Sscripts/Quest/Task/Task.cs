using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskState
{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task")]
public class Task : ScriptableObject
{
    #region Events

    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public event StateChangedHandler onStateChanged;

    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    public event SuccessChangedHandler onSuccessChanged;

    #endregion
    [SerializeField] private Category category;
    public Category Category => category;

    [Header("Text")]
    [SerializeField] private string codeName;
    public string CodeName => codeName;

    [SerializeField] private string discription;
    public string Discription => discription;

    [Header("Action")]
    [SerializeField] private TaskAction action;

    [Header("Target")]
    [SerializeField] private TaskTarget[] targets;

    [Header("Setting")]
    [SerializeField] private InitialSuccessValue initialSuccessValue; 

    [SerializeField] private int neededSuccessToComplete;
    public int NeededSuccessToComplete => neededSuccessToComplete;

    [SerializeField] private bool canReceiveReportsDuringCompletion;

    private TaskState state;

    public TaskState State
    {
        get => state;
        set
        {
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
        }
    }

    private int currentSuccess;
    public int CurrentSuccess
    {
        get => currentSuccess;
        set
        {
            int prevSuccess = currentSuccess;
            currentSuccess = Mathf.Clamp(value, 0, neededSuccessToComplete);
            if(currentSuccess != prevSuccess)
            {
                State = currentSuccess == neededSuccessToComplete ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, currentSuccess, prevSuccess);
            }
        }
    }

    public bool IsComplete => State == TaskState.Complete;
    public Quest Owner { get; private set; }

    public void SetUp(Quest owner)
    {
        Owner = owner;  
    }

    public void Start()
    {
        State = TaskState.Running;

        if(initialSuccessValue)
        {
            CurrentSuccess = initialSuccessValue.GetValue(this);
        }
    }

    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;
    }


    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    public void Complete()
    {
        CurrentSuccess = neededSuccessToComplete;
    }

    public bool IsTarget(string category, object target) => Category == Category && targets.Any(x => x.IsEqual(target)) && (!IsComplete || (IsComplete && canReceiveReportsDuringCompletion));

    public bool ContainsTarget(object target) => targets.Any(x => x.IsEqual(target));
}
