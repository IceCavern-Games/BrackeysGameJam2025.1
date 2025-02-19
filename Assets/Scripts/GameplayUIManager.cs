using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameplayUIManager : MonoBehaviour
{
    [Inject] private readonly TaskManager _taskManager;

    private UIDocument _document;

    private VisualElement _interactContainer;
    private Label _interactLabel;

    // Task container
    private VisualElement _taskContainer;
    private ListView _taskList;

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
        _taskList = _taskContainer.Q<ListView>();

        _clockContainer = _document.rootVisualElement.Q<VisualElement>(name: "clock-container");
        _clockLabel = _clockContainer.Q<Label>();

        if (_interactLabel.text == "Interact")
            HideInteractPrompt();
    }

    private void Start()
    {
        _taskList.itemsSource = _taskManager.ActiveTasks;
    }

    #region Clock UI
    public void HideClockContainer()
    {
        _clockContainer.style.display = DisplayStyle.None;
    }

    public void SetClockText(string text)
    {
        if (_clockLabel == null || _clockContainer == null)
            return;

        _clockLabel.text = text;
        _clockContainer.style.display = DisplayStyle.Flex;
    }
    #endregion
    #region Interact UI
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
    #endregion
}
