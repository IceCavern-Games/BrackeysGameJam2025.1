using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Device Display Configurator", menuName = "Scriptable Objects/Device Display Configurator")]
public class DeviceDisplayConfigurator : ScriptableObject
{
    [System.Serializable]
    public struct DeviceSet
    {
        public string deviceRawPath;
        public DeviceDisplaySettings deviceDisplaySettings;
    }

    public List<DeviceSet> listDeviceSets = new List<DeviceSet>();

    private Color _fallbackDisplayColor = Color.white;

    public string GetDeviceName(InputDevice device)
    {
        string currentDeviceRawPath = device.ToString();
        string newDisplayName = null;

        for (int i = 0; i < listDeviceSets.Count; i++)
        {
            if (currentDeviceRawPath.Contains(listDeviceSets[i].deviceRawPath))
            {
                newDisplayName = listDeviceSets[i].deviceDisplaySettings.deviceDisplayName;
                break;
            }
        }

        newDisplayName ??= currentDeviceRawPath;

        return newDisplayName;
    }

    public Color GetDeviceColor(InputDevice device)
    {
        string currentDeviceRawPath = device.ToString();
        Color newDisplayColor = _fallbackDisplayColor;

        for (int i = 0; i < listDeviceSets.Count; i++)
        {
            if (currentDeviceRawPath.Contains(listDeviceSets[i].deviceRawPath))
            {
                newDisplayColor = listDeviceSets[i].deviceDisplaySettings.deviceDisplayColor;
                break;
            }
        }

        return newDisplayColor;
    }

    public Sprite GetDeviceBindingIcon(InputDevice device, string deviceInputBinding)
    {
        if (device == null)
            return null;

        string currentDeviceRawPath = device.ToString();
        Sprite displaySpriteIcon = null;

        for (int i = 0; i < listDeviceSets.Count; i++)
        {
            if (currentDeviceRawPath.Contains(listDeviceSets[i].deviceRawPath))
            {
                if (listDeviceSets[i].deviceDisplaySettings.deviceHasContextIcons)
                {
                    displaySpriteIcon = FilterForDeviceInputBinding(listDeviceSets[i], deviceInputBinding);
                    break;
                }
            }
        }

        return displaySpriteIcon;
    }

    Sprite FilterForDeviceInputBinding(DeviceSet targetDeviceSet, string inputBinding)
    {
        Sprite spriteIcon = null;

        switch (inputBinding)
        {
            case "Button North":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.buttonNorthIcon;
                break;

            case "Button South":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.buttonSouthIcon;
                break;

            case "Button West":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.buttonWestIcon;
                break;

            case "Button East":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.buttonEastIcon;
                break;

            case "Right Shoulder":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.triggerRightFrontIcon;
                break;

            case "Right Trigger":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.triggerRightBackIcon;
                break;

            case "rightTriggerButton":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.triggerRightBackIcon;
                break;

            case "Left Shoulder":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.triggerLeftFrontIcon;
                break;

            case "Left Trigger":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.triggerLeftBackIcon;
                break;

            case "leftTriggerButton":
                spriteIcon = targetDeviceSet.deviceDisplaySettings.triggerLeftBackIcon;
                break;

            default:
                for (int i = 0; i < targetDeviceSet.deviceDisplaySettings.customContextIcons.Count; i++)
                {
                    if (targetDeviceSet.deviceDisplaySettings.customContextIcons[i].customInputContextString == inputBinding)
                    {
                        if (targetDeviceSet.deviceDisplaySettings.customContextIcons[i].customInputContextIcon != null)
                        {
                            spriteIcon = targetDeviceSet.deviceDisplaySettings.customContextIcons[i].customInputContextIcon;
                        }
                    }
                }

                break;
        }

        return spriteIcon;
    }
}
