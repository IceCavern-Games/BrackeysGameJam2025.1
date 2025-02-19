using System;
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

    public GameTask FollowUpTask => _followUpTask;
    public TaskStatus Status { get; private set; } = TaskStatus.Inactive;

    [SerializeField] private GameTask _followUpTask;

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
