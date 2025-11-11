/*
 * ---------------------------------------------------------------------------
 * Description: Initializes the custom save system when the game starts. 
 *              Loads SaveCustomObject from Resources and instantiates a persistent 
 *              GameObject in the scene with SaveCustomInScene and AutoSaveCustom 
 *              components. Ensures consistent and centralized save management 
 *              across scenes using runtime initialization.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

namespace SaveCustomGame
{
    public class SaveCustomInitialization
    {
        /// <summary>
        /// Holds the loaded SaveCustomObject instance used across the game.
        /// </summary>
        public static SaveCustomObject saveCustomObject; // Static reference to the SaveCustomObject.

        #region === Initialization Method ===

        /// <summary>
        /// Entry point invoked when the game starts. It loads the SaveCustomObject data,
        /// creates a persistent GameObject in the scene, attaches relevant components,
        /// and ensures the save system persists across scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static void RunGameInitialization()
        {
            // Inform in the console that the save system initialization has started.
            Debug.Log("Save Custom Initialization.");

            // Load Save Custom Object Data stored in Resources.
            LoadSettingsData();

            // Verify if the SaveCustomObject was successfully loaded.
            if (saveCustomObject == null)
            {
                // If not found, notify via console and stop the initialization.
                Debug.LogError("Could not find SaveCustomObject with name 'Save Custom Object Data'.");
                return;
            }

            // Create a new GameObject to persist the save system during runtime.
            GameObject saveCustomGameObject = new("[Save Custom Object]");

            // Attach component responsible for holding the SaveCustomObject instance.
            var saveCustomInScene = saveCustomGameObject.AddComponent<SaveCustomInScene>();

            // Attach component responsible for handling autosave behavior.
            var autoSaveCustom = saveCustomGameObject.AddComponent<AutoSaveCustom>();

            // Pass the loaded SaveCustomObject reference to the SaveCustomInScene component.
            saveCustomInScene.saveCustomObject = saveCustomObject;

            // Link the AutoSaveCustom component to SaveCustomInScene to operate correctly.
            autoSaveCustom.saveCustomInScene = saveCustomInScene;

            // Ensure this GameObject will not be destroyed when switching scenes.
            Object.DontDestroyOnLoad(saveCustomGameObject);
        }

        #endregion

        #region === Load Save Settings Data ===

        /// <summary>
        /// Loads the SaveCustomObject asset from the Resources folder. Logs warnings if not found.
        /// </summary>
        private static void LoadSettingsData()
        {
            // Attempt to load the SaveCustomObject asset by name.
            saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data");

            // Log an error if the asset could not be located.
            if (saveCustomObject == null)
            {
                Debug.LogError("Failed to load Save Custom Object Data from Resources.");
            }
        }

        #endregion
    }
}