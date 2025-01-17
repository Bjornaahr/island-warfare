﻿using UnityEngine;

public class InputManager
{
    bool currentlyReSelectingInput = false; // Is currently in the process of rebinding a key
    public bool frozenAngle = true;         // Is the camera's angle frozen?

    Actions currentReSelect;                // Key currently being rebounded
    GameObject currentButton;               // Currently selected button

    // Enum of actions
    public enum Actions
    {
        FORWARDS,
        BACKWARDS,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        ZOOM,
        PAUSE,
        CHANGE_CAMERA_MODE
    };

    public KeyCode[] bindings;
    
    // Initial keyInput values, run on startup
    public InputManager()
    {
        bindings = new KeyCode[System.Enum.GetValues(typeof(Actions)).Length];                      // Initializes bindings

        bindings[(int)Actions.FORWARDS] = GetKeyCodeFromPlayerPrefs(Actions.FORWARDS, KeyCode.W);
        bindings[(int)Actions.LEFT] = GetKeyCodeFromPlayerPrefs(Actions.LEFT, KeyCode.A);
        bindings[(int)Actions.BACKWARDS] = GetKeyCodeFromPlayerPrefs(Actions.BACKWARDS, KeyCode.S);
        bindings[(int)Actions.RIGHT] = GetKeyCodeFromPlayerPrefs(Actions.RIGHT, KeyCode.D);
        bindings[(int)Actions.UP] = GetKeyCodeFromPlayerPrefs(Actions.UP, KeyCode.Space);
        bindings[(int)Actions.DOWN] = GetKeyCodeFromPlayerPrefs(Actions.DOWN, KeyCode.LeftShift);
        bindings[(int)Actions.ZOOM] = GetKeyCodeFromPlayerPrefs(Actions.ZOOM, KeyCode.LeftControl);
        bindings[(int)Actions.PAUSE] = GetKeyCodeFromPlayerPrefs(Actions.PAUSE, KeyCode.Escape);
        bindings[(int)Actions.CHANGE_CAMERA_MODE] = GetKeyCodeFromPlayerPrefs(Actions.CHANGE_CAMERA_MODE, KeyCode.LeftAlt);
    }

    // Changes selected control to the specified key-code
    public void ChangeControl(Actions action, KeyCode keyCode)
    {
        bindings[(int)action] = keyCode;
        PlayerPrefs.SetString(action.ToString(), keyCode.ToString());
    }

    // Returns the keyCode version of the string
    public KeyCode StringToKey(string keyCode)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCode);
    }

    // Gets keycode for specified action
    private KeyCode GetKeyCodeFromPlayerPrefs(Actions action, KeyCode defaultValue)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(System.Enum.GetName(typeof(Actions), action), System.Enum.GetName(typeof(KeyCode), defaultValue)));
    }

    // Returns true if a keybinding is in the process of being changed
    public bool IsSelectingInput()
    {
        return currentlyReSelectingInput;
    }
    
    // Returns currently selected action
    public Actions ReturnCurrentlySelectedAction() 
    {
        return currentReSelect;
    }

    // Updates currently selected action and sets currentlyReSelectingInput to true
    public void UpdateSelectedAction(int selectedAction)
    {
        currentReSelect = (Actions)selectedAction;
        currentlyReSelectingInput = true;
    }

    // Updates key when finished
    public void FinishedUpdateKey()
    {
        currentlyReSelectingInput = false;
    }
}
