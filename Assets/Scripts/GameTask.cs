using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Task", menuName = "Scriptable Objects/Task")]
public class GameTask : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [HideInInspector] public FetchObject FetchObject;
    [HideInInspector] public FetchTarget FetchTarget;

    private bool _isActive;
    private bool _isComplete;
    

    public bool CheckTask()
    {
        return FetchTarget.IsPositionInside(FetchObject.transform.position);
    }

    public void SetTaskActive(bool active)
    {
        _isActive = active;
        FetchTarget.SetEnabled(active);
    }
}
