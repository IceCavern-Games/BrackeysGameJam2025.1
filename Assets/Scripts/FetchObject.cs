using Reflex.Attributes;
using System.Collections.Generic;
using UnityEngine;

public class FetchObject : MonoBehaviour
{
    [Inject] private readonly TaskManager _taskManager;
    [SerializeField] private List<FetchTask> _gameTasks;

    private void OnEnable()
    {
        BindToTask();
    }

    private void BindToTask()
    {
        foreach (var gameTask in _gameTasks)
        {
            var task = _taskManager.Tasks.Find((task) => task.Name == gameTask.Name);

            Debug.Assert(task != null, "Task not defined in Task Manager prefab.");

            (task as FetchTask).FetchObject = this;
        }
    }
}
