using System;
using UnityEngine;

public abstract class GameTask : ScriptableObject
{
    public enum TaskStatus
    {
        Inactive,
        InProgress,
        Completed,
        Failed
    }
    
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;
    [SerializeField] protected int _deadline;
    
    protected TaskStatus _status = TaskStatus.Inactive;

    public string TaskName => _name;
    public string TaskDescription => _description;
    public int Deadline => _deadline;
    public TaskStatus Status => _status;

    public virtual TaskStatus CheckTask(float time) { return TaskStatus.Inactive; }

    public virtual void StartTask()
    {
        _status = TaskStatus.InProgress;
    }

    public virtual void FinishTask(bool isSuccess)
    {
        _status = isSuccess ? TaskStatus.Completed : TaskStatus.Failed;
    }
}
