using Reflex.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public List<GameTask> ActiveTasks { get; private set; } = new();
    public List<GameTask> Tasks { get; private set; } = new();

    [Inject] private readonly GameManager _gameManager;

    [SerializeField] private List<GameTask> _taskData;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Reset();
    }

    private void Update()
    {
        if (!_gameManager.Clock.IsActive)
            return;

        foreach (var task in Tasks.ToArray())
            task.Check(_gameManager.Clock.ElapsedTime);
    }

    public void Reset()
    {
        ActiveTasks.Clear();
        Tasks.Clear();
        SubscribeToTaskEvents();
    }

    private void OnTaskFailed(GameTask task)
    {
        Debug.Log($"Task \"{task.Name}\" failed!");

        task.Failed -= OnTaskFailed;
        task.Completed -= OnTaskCompleted;

        ActiveTasks.Remove(task);
        _gameManager.Fail();
    }

    private void OnTaskCompleted(GameTask task)
    {
        Debug.Log($"Task \"{task.Name}\" completed!");

        task.Failed -= OnTaskFailed;
        task.Completed -= OnTaskCompleted;

        // Finished task out of order. End the game. 😈
        if (ActiveTasks[0] != task)
            _gameManager.Fail();

        if (task.FollowUpTask != null)
        {
            var newTask = Tasks.Find((t) => t.Name == task.FollowUpTask.Name);
            ActiveTasks.Insert(0, newTask);
            newTask.Start();
        }

        ActiveTasks.Remove(task);

        // Check if all tasks have been completed.
        if (Tasks.All(t => t.Status == GameTask.TaskStatus.Completed))
            _gameManager.Win();
    }

    private void OnTaskStarted(GameTask task)
    {
        Debug.Log($"Task \"{task.Name}\" started!");

        task.Started -= OnTaskStarted;
        task.Failed += OnTaskFailed;
        task.Completed += OnTaskCompleted;

        if (!ActiveTasks.Contains(task))
            ActiveTasks.Add(task);
    }

    private void SubscribeToTaskEvents()
    {
        foreach (var taskData in _taskData)
        {
            var task = taskData.Clone();
            Tasks.Add(task);
            task.Started += OnTaskStarted;
        }
    }
}
