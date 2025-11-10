using UnityEngine;
using UnityEngine.InputSystem;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace PXE.Core.Utilities.Input
{
    public static class InputHelper
    {
        public static InputDevice LastUsedDevice { get; private set; }

        static InputHelper()
        {
            // Set the initial device
            if (Gamepad.current != null)
            {
                LastUsedDevice = Gamepad.current;
            }
            else if (Keyboard.current != null)
            {
                LastUsedDevice = Keyboard.current;
            }
            
            // Listen for device changes
            InputSystem.onDeviceChange += (device, change) =>
            {
                if (change == InputDeviceChange.UsageChanged || change == InputDeviceChange.Added)
                {
                    LastUsedDevice = device;
                }
            };
        }

/// <summary>
/// Executes the GetButtonNameForAction method.
/// Handles the GetButtonNameForAction functionality.
/// </summary>
        public static string GetButtonNameForAction(InputActionReference actionReference)
        {
            if (actionReference == null)
            {
                Debug.LogError("Action reference is not assigned.");
                return string.Empty;
            }

            var action = actionReference.action;
            if (action == null)
            {
                Debug.LogError("The action reference does not contain a valid action.");
                return string.Empty;
            }

            var controls = action.controls;
            foreach (var control in controls)
            {
                if (control.device == LastUsedDevice)
                {
                    return control.displayName;
                }
            }

            return string.Empty;
        }
    }
}