﻿using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static InputManager inputManager;        // The input manager
    public bool isPaused = false;                   // Is paused
    
    // Resources
    public float resourceMoney = 0;
    public float resourceFood = 0;
    public float resourceWood = 0;
    public float resourceIron = 0;
    public float resourceCopper = 0;
    public float resourceUranium = 0;
    public float resourceCoal = 0;
    public float resourceAluminium = 0;
    public float resourceGold = 0;
    public float resourceOil = 0;
    public float resourceAnimals = 0;


    // Options
    [SerializeField] Canvas optionsMenu;            // The options menu
    [SerializeField] OptionsManager optionsManager; // The options manager
    
    // Start is called before the first frame update
    void Awake()
    {
        // DontDestroyOnLoad(gameObject);  // Stops object from being destroyed

        if (inputManager == null)
        {
            inputManager = new InputManager();
        }

        optionsMenu.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }
    
    // Updates canvas to being active or inactive
    public void UpdateCanvas(bool active)
    {
        optionsMenu.enabled = isPaused = active;

        // Makes mouse invisible when moving about but visible and starting centered when in a menu and when selection is activated
        Cursor.visible = active || inputManager.frozenAngle; // WIP
            if (Cursor.visible)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
    }

    // Sets system to expect an action's input to be changed
    public void UpdateInputKey(int selectedAction)
    {
        inputManager.updateSelectedAction(selectedAction);
        optionsManager.currentButton = EventSystem.current.currentSelectedGameObject;
    }
}
