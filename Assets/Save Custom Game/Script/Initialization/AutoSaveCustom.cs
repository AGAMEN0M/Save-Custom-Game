/*
 * ---------------------------------------------------------------------------
 * Description: This script manages an auto-save system for a game, saving the
 *              game state at regular intervals. It references a `SaveCustomInScene` 
 *              component to handle the actual save process. The script allows
 *              customization of the auto-save interval and cycles through a set
 *              of save slots to maintain multiple auto-save files. Autosaving can 
 *              be disabled via settings, and the script ensures that saves do not 
 *              occur if auto-save is turned off.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/
using UnityEngine;

public class AutoSaveCustom : MonoBehaviour
{
    [Header("Auto Save Settings")]
    public SaveCustomInScene saveCustomInScene; // Reference to SaveCustomInScene component.

    private int currentAutoSaveSlot = 1; // Current slot for autosaving.
    private float timeSinceLastSave = 0f; // Time elapsed since the last save.
    private float saveInterval = 60f; // Time interval between autosaves.

    private void Start()
    {
        // Check if the SaveCustomInScene reference is not assigned.
        if (saveCustomInScene == null)
        {
            saveCustomInScene = SaveDataUtility.GetComponentSaveCustomInScene(); // Get the SaveCustomInScene component if it's not assigned.
        }

        saveCustomInScene.saveCustomObject.autosaveEnabled = false; // Disable autosave at the start.
        saveInterval = saveCustomInScene.saveCustomObject.saveInterval; // Set the save interval from SaveCustomObject settings.

        // Retrieve the current autosave slot from PlayerPrefs if it exists.
        if (PlayerPrefs.HasKey("AutoSaveCustom")) { currentAutoSaveSlot = PlayerPrefs.GetInt("AutoSaveCustom"); }
    }

    private void FixedUpdate()
    {
        timeSinceLastSave += Time.fixedDeltaTime; // Track the time elapsed since the last save.

        // Check if the time since the last save exceeds the set save interval.
        if (timeSinceLastSave >= saveInterval)
        {
            timeSinceLastSave = 0f;

            // Check if autosave by event is disabled in settings before triggering autosave.
            if (saveCustomInScene.saveCustomObject.saveGameByEvent == false) { SaveAutoGame(); }
        }
    }

    // Method to trigger the autosave functionality.
    public void SaveAutoGame()
    {
        if (!saveCustomInScene.saveCustomObject.autosaveEnabled) { return; } // Check if autosave is disabled.
        saveCustomInScene.fileName = $"0 - {currentAutoSaveSlot}"; // Set the filename for the autosave based on the current autosave slot.
        saveCustomInScene.SaveData(); // Call the SaveData method from SaveCustomInScene to perform autosave.
        currentAutoSaveSlot = (currentAutoSaveSlot % 6) + 1; // Update the autosave slot for the next save.
        PlayerPrefs.SetInt("AutoSaveCustom", currentAutoSaveSlot); // Save the updated autosave slot in PlayerPrefs.
    }
}