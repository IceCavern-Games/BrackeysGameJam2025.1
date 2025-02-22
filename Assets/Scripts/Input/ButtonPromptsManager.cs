using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(UIDocument))]
public class ButtonPromptsManager : MonoBehaviour
{
    public DeviceDisplayConfigurator DeviceDisplayConfig => _deviceDisplayConfiguration;

    [SerializeField] private DeviceDisplayConfigurator _deviceDisplayConfiguration;

    private VisualElement _container;
    private InputManager _inputManager;
    private Dictionary<string, string> _promptMappings;
    private UIDocument _uiDocument;

    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _uiDocument = GetComponent<UIDocument>();

        _promptMappings = PromptMappings.Gameplay;
    }

    private void OnEnable()
    {
        _container = _uiDocument.rootVisualElement.Q<VisualElement>("button-prompts");

        RenderButtonPrompts();

        _inputManager.DeviceTypeChanged += OnDeviceChanged;
    }

    private void OnDisable()
    {
        _inputManager.DeviceTypeChanged -= OnDeviceChanged;
    }

    /// <summary>
    /// Get the Sprite/Icon for the given action's binding.
    /// </summary>
    public Sprite GetBindingIconForAction(InputAction action)
    {
        string bindingMask = _inputManager.Input.currentControlScheme;
        int controlBindingIndex = action.GetBindingIndex(bindingMask);
        string currentBindingInput = InputControlPath.ToHumanReadableString(
            action.bindings[controlBindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        // Handle keyboard composite for WASD
        if (bindingMask == "Keyboard&Mouse" && (action.name == "Move" || action.name == "Navigate"))
            currentBindingInput = "WASD";

        return _deviceDisplayConfiguration.GetDeviceBindingIcon(_inputManager.CurrentDevice, currentBindingInput);
    }

    public void ShowPrompts(Dictionary<string, string> mappings)
    {
        _promptMappings = mappings;
    }

    private void OnDeviceChanged(InputDevice device, string scheme)
    {
        RenderButtonPrompts();
    }

    private void RenderButtonPrompts()
    {
        if (_promptMappings == null)
            return;

        _container.Clear();

        foreach (var mapping in _promptMappings)
        {
            var prompt = new ButtonPrompt
            {
                Sprite = GetBindingIconForAction(_inputManager.Input.actions[mapping.Key]),
                Text = mapping.Value
            };
            _container.Add(prompt);
        }
    }
}
