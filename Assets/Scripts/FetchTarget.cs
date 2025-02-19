using Reflex.Attributes;
using UnityEngine;

public class FetchTarget : MonoBehaviour
{
    [Inject] private readonly TaskManager _taskManager;

    [SerializeField] private FetchTask _gameTask;

    private Collider _collider;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        BindToTask();
        SetEnabled(false);
    }

    public void SetEnabled(bool isEnabled)
    {
        _meshRenderer.enabled = isEnabled;
    }

    public bool IsPositionInside(Vector3 position)
    {
        return _collider.bounds.Contains(position);
    }

    private void OnTaskCompleted(GameTask task)
    {
        Debug.Log($"{gameObject.name} ON TASK COMPLETED");

        task.Completed -= OnTaskCompleted;
        task.Failed -= OnTaskFailed;

        SetEnabled(false);
    }

    private void OnTaskFailed(GameTask task)
    {
        task.Completed -= OnTaskCompleted;
        task.Failed -= OnTaskFailed;

        SetEnabled(false);
    }

    private void OnTaskStarted(GameTask task)
    {
        task.Started -= OnTaskStarted;

        SetEnabled(true);
    }

    private void BindToTask()
    {
        var task = _taskManager.Tasks.Find((task) => task.Name == _gameTask.Name);

        Debug.Assert(task != null, "Task not defined in Task Manager prefab.");

        task.Completed += OnTaskCompleted;
        task.Failed += OnTaskFailed;
        task.Started += OnTaskStarted;

        (task as FetchTask).FetchTarget = this;
    }
}
