using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameplayUIManager : MonoBehaviour
{
    private UIDocument _document;

    private VisualElement _interactContainer;
    private Label _interactLabel;

    // Task container
    private VisualElement _taskContainer;
    private Label _taskNameLabel;
    private Label _taskDescriptionLabel;
    private Label _taskDeadlineLabel;
    private Label _taskFailedLabel;
    
    // Clock
    private VisualElement _clockContainer;
    private Label _clockLabel;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _interactContainer = _document.rootVisualElement.Q<VisualElement>(name: "interact-prompt");
        _interactLabel = _interactContainer.Q<Label>();

        _taskContainer = _document.rootVisualElement.Q<VisualElement>(name: "task-container");
        _taskNameLabel = _taskContainer.Q<Label>(name: "TaskBox__taskName");
        _taskDescriptionLabel = _taskContainer.Q<Label>(name: "TaskBox__taskDescription");
        _taskDeadlineLabel = _taskContainer.Q<Label>(name: "TaskBox__taskDeadline");
        _taskFailedLabel = _taskContainer.Q<Label>(name: "TaskBox__taskFailed");
        
        _clockContainer = _document.rootVisualElement.Q<VisualElement>(name: "clock-container");
        _clockLabel = _clockContainer.Q<Label>();
        
        if (_interactLabel.text == "Interact")
            HideInteractPrompt();

        HideTaskContainer();
    }

    /// <summary>
    ///
    /// </summary>
    public void HideInteractPrompt()
    {
        _interactContainer.style.display = DisplayStyle.None;
    }

    /// <summary>
    ///
    /// </summary>
    public void SetInteractPrompt(string text)
    {
        _interactLabel.text = text;
        _interactContainer.style.display = DisplayStyle.Flex;
    }

    public void HideTaskContainer()
    {
        _taskContainer.style.display = DisplayStyle.None;
    }

    public void SetTaskBox(GameTask task)
    {
        _taskNameLabel.text = task.TaskName;
        _taskDescriptionLabel.text = task.TaskDescription;
        _taskDeadlineLabel.text = $"Deadline: {(9 + task.Deadline / 60).ToString("D2")}:{(task.Deadline % 60).ToString("D2")}";
        _taskContainer.style.display = DisplayStyle.Flex;

        if (task.Status == GameTask.TaskStatus.Failed)
            _taskFailedLabel.style.display = DisplayStyle.Flex;
        else 
            _taskFailedLabel.style.display = DisplayStyle.None;
    }

    public void HideClockContainer()
    {
        _clockContainer.style.display = DisplayStyle.None;
    }

    public void SetClockText(string text)
    {
        _clockLabel.text = text;
        _clockContainer.style.display = DisplayStyle.Flex;
    }
}
