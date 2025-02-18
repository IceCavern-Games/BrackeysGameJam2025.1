using System;
using UnityEngine;

public abstract class GameTask : ScriptableObject
{
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;


    protected bool _isActive;
    protected bool _isComplete = true;

    public string TaskName => _name;
    public string TaskDescription => _description;

    public virtual bool CheckTask() { return false; }

    public virtual void StartTask()
    {
        _isActive = true;
    }

    public virtual void FinishTask()
    {
        _isActive = false;
        _isComplete = true;
    }
}
