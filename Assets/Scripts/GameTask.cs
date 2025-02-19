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

    public event Action<GameTask> Failed;
    public event Action<GameTask> Finished;
    public event Action<GameTask> Started;

    public string Name;
    public string Description;
    public int Deadline;
    public int StartsAt;

    public TaskStatus Status { get; private set; } = TaskStatus.Inactive;

    public virtual void Check(float time)
    {
        if (Status == TaskStatus.Inactive && time >= StartsAt)
            Start();

        if (Status == TaskStatus.InProgress && time >= Deadline)
            Fail();
    }

    public virtual void Start()
    {
        Started?.Invoke(this);
        Status = TaskStatus.InProgress;
    }

    public virtual void Fail()
    {
        Failed?.Invoke(this);
        Status = TaskStatus.Failed;
    }

    public virtual void Finish()
    {
        Finished?.Invoke(this);
        Status = TaskStatus.Completed;
    }
}
