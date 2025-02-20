using Reflex.Attributes;
using UnityEngine;

public class TaskTarget : MonoBehaviour
{
    [Inject] protected readonly TaskManager _taskManager;

    private Collider _collider;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        SetEnabled(false);
        BindToTask();
    }

    public void SetEnabled(bool isEnabled)
    {
        _meshRenderer.enabled = isEnabled;
    }

    public bool IsPositionInside(Vector3 position)
    {
        return _collider.bounds.Contains(position);
    }

    protected virtual void BindToTask() { }

    protected void OnTaskCompleted(GameTask task)
    {
        task.Completed -= OnTaskCompleted;
        task.Failed -= OnTaskFailed;

        SetEnabled(false);
    }

    protected void OnTaskFailed(GameTask task)
    {
        task.Completed -= OnTaskCompleted;
        task.Failed -= OnTaskFailed;

        SetEnabled(false);
    }

    protected void OnTaskStarted(GameTask task)
    {
        task.Started -= OnTaskStarted;

        SetEnabled(true);
    }
}
