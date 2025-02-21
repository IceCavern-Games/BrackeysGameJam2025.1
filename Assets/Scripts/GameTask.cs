using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameTask : ScriptableObject
{
    public enum TaskStatus
    {
        Inactive,
        InProgress,
        Completed,
        Failed
    }

    public event Action<GameTask> Completed;
    public event Action<GameTask> Failed;
    public event Action<GameTask> Started;

    public string Name;
    public string Description;
    public int Deadline;
    public int StartsAt;

    public string CompleteDialogueNode => _completeDialogueNode;
    public List<GameTask> FollowUpTasks => _followUpTasks;
    public string StartDialogueNode => _startDialogueNode;
    public TaskStatus Status { get; private set; } = TaskStatus.Inactive;

    [SerializeField] private string _startDialogueNode = string.Empty;
    [SerializeField] private string _completeDialogueNode = string.Empty;
    [FormerlySerializedAs("_followUpTask")] [SerializeField] private List<GameTask> _followUpTasks;

    public virtual void Check(float time)
    {
        if (Status == TaskStatus.Inactive && time >= StartsAt && StartsAt != -1)
            Start();

        if (Status == TaskStatus.InProgress && time >= Deadline)
            Fail();
    }

    public virtual void Complete()
    {
        Status = TaskStatus.Completed;
        Completed?.Invoke(this);
    }

    public virtual void Fail()
    {
        Status = TaskStatus.Failed;
        Failed?.Invoke(this);
    }

    public virtual void Start()
    {
        Status = TaskStatus.InProgress;
        Started?.Invoke(this);
    }
}
