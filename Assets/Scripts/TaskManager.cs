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
        _clock = new Timer(90, false);
        _currentTaskIndex = 0;
        _tasks[_currentTaskIndex].SetTaskActive(true);
        _uiManager.Gameplay.SetTaskBox(_tasks[_currentTaskIndex]);
    }
    
    private void Update()
    {
        if (_allTasksDone)
            return;
        
        _clock.Tick(Time.deltaTime);

        if (_tasks[_currentTaskIndex].CheckTask())
        {
            _tasks[_currentTaskIndex].SetTaskActive(false);
            _currentTaskIndex++;
            

            if (_currentTaskIndex >= _tasks.Count)
            {
                _allTasksDone = true;
                _uiManager.Gameplay.HideTaskContainer();
            }
            else
            {
                _tasks[_currentTaskIndex].SetTaskActive(true);
                _uiManager.Gameplay.SetTaskBox(_tasks[_currentTaskIndex]);
            }
        }
    }
    
    
}
