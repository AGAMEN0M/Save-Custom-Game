/*
 * ---------------------------------------------------------------------------
 * Description: Handles runtime auto-save control and toggling of GameObjects 
 *              for testing and debugging purposes. Provides feedback in the 
 *              console when actions occur. Allows testing of save functionality, 
 *              auto-save toggle, and dynamic activation of objects in-game. 
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

using static SaveCustomGame.SaveDataUtility;

[AddComponentMenu("UI/Save Custom Game/In Background/Test Save Custom")]
public class TestSaveCustom : MonoBehaviour
{
    #region === Inspector Fields ===

    [Header("Key Settings")]
    [SerializeField, Tooltip("Key to activate auto-save and trigger a save event.")]
    private KeyCode activateAndSave = KeyCode.Space;

    [SerializeField, Tooltip("Key to disable auto-save.")]
    private KeyCode disable = KeyCode.Escape;

    [Header("Test Objects")]
    [SerializeField, Tooltip("Array of GameObjects to toggle active state for testing.")]
    private GameObject[] gameObjects;

    [Header("Optional Save Slot")]
    [SerializeField, Tooltip("Optional: Specific save slot to test saving.")]
    private int testSaveSlot = 1;

    #endregion

    #region === Unity Events ===

    private void Update() => HandleInput();

    #endregion

    #region === Input Handling ===

    /// <summary>
    /// Checks for key presses to activate, save, or disable auto-save.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown(activateAndSave))
        {
            // Enable auto-save and trigger save event.
            SetAutoSave(true);
            SaveTestSlot();
        }

        if (Input.GetKeyDown(disable))
        {
            // Disable auto-save.
            SetAutoSave(false);
            Debug.Log("[TestSaveCustom] Auto-save disabled.", this);
        }
    }

    #endregion

    #region === Save Testing ===

    /// <summary>
    /// Performs a save event and provides console feedback.
    /// Saves in the optional test slot if assigned.
    /// </summary>
    private void SaveTestSlot()
    {
        if (testSaveSlot > 0)
        {
            SaveEvent(); // Trigger save event, could be modified to specify slot in future.
            Debug.Log($"[TestSaveCustom] Save triggered in slot {testSaveSlot}.", this);
        }
        else
        {
            SaveEvent();
            Debug.Log("[TestSaveCustom] Save triggered (no specific slot).", this);
        }
    }

    #endregion

    #region === GameObject Toggle ===

    /// <summary>
    /// Toggles the active state of each GameObject in the assigned array.
    /// If a GameObject is active, it becomes inactive, and vice versa.
    /// Useful for testing dynamic object visibility or activation during gameplay.
    /// </summary>
    public void SwitchGameObject()
    {
        foreach (var obj in gameObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }

        Debug.Log("[TestSaveCustom] Toggled GameObjects state.", this);
    }

    #endregion
}