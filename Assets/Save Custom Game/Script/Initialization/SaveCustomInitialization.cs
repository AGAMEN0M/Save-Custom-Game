/*
 * ---------------------------------------------------------------------------
 * Description: Initializes the custom save system when the game starts. 
 *              Loads SaveCustomObject from Resources and instantiates a persistent 
 *              GameObject in the scene with SaveCustomInScene and AutoSaveCustom 
 *              components. Ensures consistent and centralized save management 
 *              across scenes using runtime initialization.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

namespace SaveCustomGame
{
    public class SaveCustomInitialization
    {
        public static SaveCustomObject saveCustomObject; // Static reference to the SaveCustomObject.

        // Method to be executed on runtime initialization.
        [RuntimeInitializeOnLoadMethod]
        public static void RunGameInitialization()
        {
            Debug.Log("Save Custom Initialization"); // Log initialization message.
            LoadSettingsData(); // Load Save Custom Object Data from Resources.

            // Check if SaveCustomObject is loaded.
            if (saveCustomObject == null)
            {
                // Log an error if SaveCustomObject is not found.
                Debug.LogError("Could not find saveCustomObject with name 'Save Custom Object Data'");
                return;
            }

            GameObject saveCustomGameObject = new("[Save Custom Object]"); // Create a new GameObject named "[Save Custom Object]".

            // Add SaveCustomInScene and AutoSaveCustom components to the GameObject.
            var saveCustomInScene = saveCustomGameObject.AddComponent<SaveCustomInScene>();
            var autoSaveCustom = saveCustomGameObject.AddComponent<AutoSaveCustom>();

            // Assign references between components and objects.
            saveCustomInScene.saveCustomObject = saveCustomObject;
            autoSaveCustom.saveCustomInScene = saveCustomInScene;

            Object.DontDestroyOnLoad(saveCustomGameObject); // Ensure the GameObject persists across scene changes.
        }

        // Method to load Save Custom Object Data from Resources.
        private static void LoadSettingsData()
        {
            saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data");

            if (saveCustomObject == null)
            {
                Debug.LogError("Failed to load Save Custom Object Data from Resources.");
            }
        }
    }
}