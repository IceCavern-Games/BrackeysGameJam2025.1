using UnityEngine;

public class LocationTarget : TaskTarget
{
    public bool PlayerIsIn { get; private set; }

    [SerializeField] private LocationTask _gameTask;

    protected override void BindToTask()
    {
        var task = _taskManager.Tasks.Find((task) => task.Name == _gameTask.Name);

        Debug.Assert(task != null, "LocationTask not defined in Task Manager prefab.");

        task.Completed += OnTaskCompleted;
        task.Failed += OnTaskFailed;
        task.Started += OnTaskStarted;

        (task as LocationTask).LocationTarget = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayerIsIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            PlayerIsIn = false;
    }
}
