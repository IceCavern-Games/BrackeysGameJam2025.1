using UnityEngine;

public class FetchTarget : TaskTarget
{
    [SerializeField] private FetchTask _gameTask;

    protected override void BindToTask()
    {
        var task = _taskManager.Tasks.Find((task) => task.Name == _gameTask.Name);

        Debug.Assert(task != null, "FetchTask not defined in Task Manager prefab.");

        task.Completed += OnTaskCompleted;
        task.Failed += OnTaskFailed;
        task.Started += OnTaskStarted;

        (task as FetchTask).FetchTarget = this;
    }
}
