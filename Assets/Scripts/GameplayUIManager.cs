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
        _taskContainer.style.display = DisplayStyle.Flex;
    }
}
