using Reflex.Attributes;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Inject] private readonly UIManager _uiManager;
    
    [SerializeField] private List<GameTask> _tasks;

    private int _currentTaskIndex;
    private Timer _clock;
    
    private bool _allTasksDone = false;

    private void Start()
    {
        _clock = new Timer(480, true);
        _currentTaskIndex = 0;
        _tasks[_currentTaskIndex].StartTask();
        _uiManager.Gameplay.SetTaskBox(_tasks[_currentTaskIndex]);
    }
    
    private void Update()
    {
        if (_allTasksDone)
            return;
        
        _clock.Tick(Time.deltaTime);
        CalculateClockTime();
        
        GameTask.TaskStatus status = _tasks[_currentTaskIndex].CheckTask(_clock.ElapsedTime);

        if (status == GameTask.TaskStatus.Completed)
        {
            _currentTaskIndex++;
            
            if (_currentTaskIndex >= _tasks.Count)
            {
                _allTasksDone = true;
                _uiManager.Gameplay.HideTaskContainer();
            }
            else
            {
                _tasks[_currentTaskIndex].StartTask();
                _uiManager.Gameplay.SetTaskBox(_tasks[_currentTaskIndex]);
            }
        }
        else if (status == GameTask.TaskStatus.Failed)
        {
            _uiManager.Gameplay.SetTaskBox(_tasks[_currentTaskIndex]);   
        }
    }

    private void CalculateClockTime()
    {
        int clockHours = 9 + (int)_clock.ElapsedTime / 60;
        int clockMinutes = (int)_clock.ElapsedTime % 60;
        
        _uiManager.Gameplay.SetClockText($"{clockHours.ToString("D2")}:{clockMinutes.ToString("D2")}");
    }
    
    
}
