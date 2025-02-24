using System;
using System.Collections.Generic;
using UnityEngine;

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
    public TaskStatus Status { get; set; } = TaskStatus.Inactive;

    [SerializeField] private string _startDialogueNode = string.Empty;
    [SerializeField] private string _completeDialogueNode = string.Empty;
    [SerializeField] private List<GameTask> _followUpTasks;

    public virtual void Check(float time)
    {
        if (Status == TaskStatus.Inactive && time >= StartsAt && StartsAt != -1)
            Start();

        if (Status == TaskStatus.InProgress && time >= Deadline)
            Fail();
    }

    public virtual void Complete()
    {
        Completed?.Invoke(this);
    }

    public virtual void Fail()
    {
        Failed?.Invoke(this);
    }

    public virtual void Start()
    {
        Started?.Invoke(this);
    }
}
