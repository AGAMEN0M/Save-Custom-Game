/*
 * ---------------------------------------------------------------------------
 * Description: Handles runtime auto-save control and toggling of GameObjects 
 *              for testing and debugging purposes. Enables auto-save and 
 *              triggers a save event when a specified key is pressed, and disables 
 *              auto-save using another key. Also includes a method to toggle the 
 *              active state of an array of GameObjects. Useful for quick testing 
 *              of save functionality and dynamic object activation during gameplay.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

using static SaveCustomGame.SaveDataUtility;

[AddComponentMenu("UI/Save Custom Game/In Background/Test Save Custom")]
public class TestSaveCustom : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode activateAndSave = KeyCode.Space; // Key to activate and trigger auto-save.
    [SerializeField] private KeyCode disable = KeyCode.Escape; // Key to disable auto-save.
    [Space(10)]
    [SerializeField] private GameObject[] gameObjects; // Array of GameObjects to toggle.

    private void LateUpdate()
    {
        // Check if the key for activation and save is pressed.
        if (Input.GetKeyDown(activateAndSave))
        {
            // Enable auto-save and trigger save event.
            EnableAutoSave();
            SaveEvent();
        }

        // Check if the key for disabling auto-save is pressed.
        if (Input.GetKeyDown(disable))
        {
            DisableAutoSave(); // Disable auto-save.
        }
    }

    /// <summary>
    /// Toggles the active state of each GameObject in the assigned array.
    /// If a GameObject is active, it becomes inactive, and vice versa.
    /// Useful for testing dynamic object visibility or activation during gameplay.
    /// </summary>
    public void SwitchGameObject()
    {
        // Loop through each GameObject in the gameObjects array.
        foreach (GameObject obj in gameObjects)
        {            
            obj.SetActive(!obj.activeSelf); // Invert the current activation state of the GameObject.
        }
    }
}