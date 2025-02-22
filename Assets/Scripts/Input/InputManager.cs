using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ButtonPromptsManager))]
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public event Action<InputDevice, string> DeviceTypeChanged;

    public InputDevice CurrentDevice { get; private set; }
    public string CurrentControlScheme { get; private set; }
    public PlayerInput Input { get; private set; }
    public ButtonPromptsManager Prompts { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Prompts = GetComponent<ButtonPromptsManager>();
        Input = GetComponent<PlayerInput>();

        CurrentDevice = Input.devices[0];
    }

    private void OnEnable()
    {
        Input.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        Input.onActionTriggered -= OnActionTriggered;
    }

    public void SetCursorLock(bool isLocked = true)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnActionTriggered(InputAction.CallbackContext ctx)
    {
        UpdateLastUsedDevice(ctx.control.device);
    }

    /// <summary>
    /// Store the last used device and update subscribers.
    /// </summary>
    private void UpdateLastUsedDevice(InputDevice device)
    {
        if (device != null && device != CurrentDevice)
        {
            CurrentDevice = device;

            // Don't refresh for keyboard to/from mouse changes.
            if (CurrentControlScheme == "Keyboard&Mouse" && (CurrentDevice.name == "Keyboard" || CurrentDevice.name == "Mouse"))
                return;

            if (Input.currentControlScheme != CurrentControlScheme)
            {
                CurrentControlScheme = Input.currentControlScheme;
                DeviceTypeChanged?.Invoke(CurrentDevice, CurrentControlScheme);

                // Debug.Log($"Control scheme changed to {CurrentControlScheme}");
            }
            else
            {
                DeviceTypeChanged?.Invoke(CurrentDevice, null);
            }

            Debug.Log($"Input Device changed to <color=#{ColorUtility.ToHtmlStringRGB(Prompts.DeviceDisplayConfig.GetDeviceColor(device))}>{Prompts.DeviceDisplayConfig.GetDeviceName(device)}</color>.");
        }
    }
}
