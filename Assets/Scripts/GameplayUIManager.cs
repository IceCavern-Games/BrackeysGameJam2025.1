using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameplayUIManager : MonoBehaviour
{
    public string CurrentPrompt => _interactPrompt.Text;

    [Inject] private readonly InputManager _inputManager;
    [Inject] private readonly TaskManager _taskManager;

    private UIDocument _document;

    private VisualElement _interactContainer;
    private ButtonPrompt _interactPrompt;

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
        _interactPrompt = _interactContainer.Q<ButtonPrompt>();

        _taskContainer = _document.rootVisualElement.Q<VisualElement>(name: "task-container");
        _taskList = _taskContainer.Q<ListView>();

        _clockContainer = _document.rootVisualElement.Q<VisualElement>(name: "clock-container");
        _clockLabel = _clockContainer.Q<Label>();

        if (_interactPrompt.Text == "Interact")
            HideInteractPrompt();
    }

    private void OnDisable()
    {
        _inputManager.DeviceTypeChanged -= OnDeviceChanged;
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
    public void HideInteractPrompt()
    {
        _interactContainer.style.display = DisplayStyle.None;
    }

    public void SetInteractPrompt(string text)
    {
        _interactPrompt.Text = text;

        _interactContainer.style.display = text != string.Empty ? DisplayStyle.Flex : DisplayStyle.None;
    }

    [Inject]
    private void OnInject()
    {
        SetPromptSprite();
        _inputManager.DeviceTypeChanged += OnDeviceChanged;
    }

    private void OnDeviceChanged(InputDevice device, string controlScheme)
    {
        SetPromptSprite();
    }

    private void SetPromptSprite()
    {
        _interactPrompt.Sprite = _inputManager.Prompts.GetBindingIconForAction(_inputManager.Input.actions["Interact"]);
    }
    #endregion
}
