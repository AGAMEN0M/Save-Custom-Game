/*
 * ---------------------------------------------------------------------------
 * Description: Component responsible for automated game saving based on a 
 *              time interval. Uses SaveCustomInScene to persist data at regular 
 *              intervals across a sequence of save slots. Respects SaveCustomObject 
 *              settings and avoids saving if auto-save is disabled, allowing flexible 
 *              and cyclic save management in the background.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

namespace SaveCustomGame
{
    [AddComponentMenu("Tools/Save Custom Game/In Background/Auto Save Custom")]
    public class AutoSaveCustom : MonoBehaviour
    {
        #region === Serialized Fields ===

        [Header("Auto Save Settings")]
        [Tooltip("Reference to the SaveCustomInScene component responsible for handling save operations.")]
        public SaveCustomInScene saveCustomInScene;

        #endregion

        #region === Private Fields ===

        private int currentAutoSaveSlot = 1; // Tracks the current autosave slot index for rotating saves.
        private float timeSinceLastSave = 0f; // Accumulates time since last save execution.
        private float saveInterval = 60f; // Time interval (in seconds) for triggering autosave.
        private readonly string saveKey = "AutoSaveCustom"; // Key used for storing autosave slot progress in PlayerPrefs.

        #endregion

        #region === Unity Methods ===

        /// <summary>
        /// Initializes autosave settings by configuring save interval, linking necessary components,
        /// and retrieving the last used autosave slot from saved preferences.
        /// </summary>
        private void Start()
        {
            // Check if SaveCustomInScene reference is missing and attempt to fetch it automatically.
            if (saveCustomInScene == null)
            {
                saveCustomInScene = SaveDataUtility.GetComponentSaveCustomInScene(); // Retrieve SaveCustomInScene.
            }

            // Disable autosave at startup to ensure controlled activation.
            saveCustomInScene.saveCustomObject.autosaveEnabled = false;

            // Load the save interval value defined in SaveCustomObject settings.
            saveInterval = saveCustomInScene.saveCustomObject.saveInterval;

            // If a previously used autosave slot is stored, retrieve it.
            if (PlayerPrefs.HasKey(saveKey)) currentAutoSaveSlot = PlayerPrefs.GetInt(saveKey);
        }

        /// <summary>
        /// Handles the autosave timer. Uses FixedUpdate to align with physics and consistent time flow.
        /// When enough time has elapsed, autosave will be triggered if autosave-by-event is disabled.
        /// </summary>
        private void FixedUpdate()
        {
            timeSinceLastSave += Time.fixedDeltaTime; // Accumulate elapsed time.

            // Check if accumulated time has reached or exceeded the autosave interval.
            if (timeSinceLastSave >= saveInterval)
            {
                timeSinceLastSave = 0f; // Reset timer.

                // Only trigger autosave if saving is not being controlled by events.
                if (saveCustomInScene.saveCustomObject.saveGameByEvent == false) SaveAutoGame();
            }
        }

        #endregion

        #region === Public Methods ===

        /// <summary>
        /// Performs an autosave if autosave is enabled in settings. Automatically rotates save slots,
        /// writes the updated slot index to PlayerPrefs, and invokes the SaveData method.
        /// </summary>
        public void SaveAutoGame()
        {
            if (!saveCustomInScene.saveCustomObject.autosaveEnabled) return; // Abort if autosave is disabled.
            saveCustomInScene.fileName = $"0 - {currentAutoSaveSlot}"; // Construct autosave filename using the current autosave slot index.
            saveCustomInScene.SaveData(); // Perform save using SaveCustomInScene's save handler.
            currentAutoSaveSlot = (currentAutoSaveSlot % 6) + 1; // Rotate autosave slot in a cyclic range (1 through 6).
            PlayerPrefs.SetInt(saveKey, currentAutoSaveSlot); // Store the updated autosave slot index in PlayerPrefs.
        }

        #endregion
    }
}