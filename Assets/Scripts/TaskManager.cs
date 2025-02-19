using Reflex.Attributes;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public List<GameTask> ActiveTasks { get; private set; } = new();

    [Inject] private readonly GameManager _gameManager;

    [SerializeField] private List<GameTask> _taskData;

    private readonly List<GameTask> _tasks = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SubscribeToTaskEvents();
    }

    private void Update()
    {
        if (_tasks != null)
        {
            foreach (var task in _tasks)
                task.Check(_gameManager.Clock.ElapsedTime);
        }
    }

    private void OnTaskFailed(GameTask task)
    {
        Debug.Log($"Task \"{task.Name}\" failed!");

        task.Failed -= OnTaskFailed;
        task.Finished -= OnTaskFinished;

        ActiveTasks.Remove(task);
        _gameManager.EndAttempt();
    }

    private void OnTaskFinished(GameTask task)
    {
        Debug.Log($"Task \"{task.Name}\" finished!");

        task.Failed -= OnTaskFailed;
        task.Finished -= OnTaskFinished;

        // Finished task out of order. End the game. 😈
        if (ActiveTasks[0] != task)
            _gameManager.EndAttempt();

        ActiveTasks.Remove(task);
    }

    private void OnTaskStarted(GameTask task)
    {
        Debug.Log($"Task \"{task.Name}\" started!");

        task.Started -= OnTaskStarted;
        task.Failed += OnTaskFailed;
        task.Finished += OnTaskFinished;

        ActiveTasks.Add(task);
    }

    private void SubscribeToTaskEvents()
    {
        foreach (var taskData in _taskData)
        {
            var task = taskData.Clone();
            _tasks.Add(task);
            task.Started += OnTaskStarted;
        }
    }
}
