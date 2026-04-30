/*
 * ---------------------------------------------------------------------------
 * Description: Initializes the custom save system at game startup.
 *              Loads SaveCustomObject from Resources and ensures a single
 *              persistent GameObject exists in the scene with required components.
 *              Prevents duplicate initialization and guarantees persistence
 *              across scene loads.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

using static SaveCustomGame.ExceptionUtility;

namespace SaveCustomGame
{
    public class SaveCustomInitialization
    {
        #region === Initialization Method ===

        /// <summary>
        /// Entry point invoked when the game starts.
        /// Ensures the save system is initialized only once and persists across scenes.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void RunGameInitialization()
        {
            Debug.Log($"{GetCallingMethodInfo()} - Save system initialization started.");

            // Prevent duplicate initialization by checking if a SaveCustomInScene already exists.
            var existing = Object.FindAnyObjectByType<SaveCustomInScene>();
            if (existing != null)
            {
                Debug.Log($"{GetCallingMethodInfo()} - Save system already initialized.");
                return;
            }

            // Load data using centralized utility (includes caching).
            var saveCustomObject = SaveDataUtility.GetSaveCustomObject();

            // Validate load result.
            if (saveCustomObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Failed to load SaveCustomObject.");
                return;
            }

            // Create runtime GameObject responsible for holding the save system components.
            GameObject saveCustomGameObject = new("[Save Custom Object]");

            // Add required components.
            var saveCustomInScene = saveCustomGameObject.AddComponent<SaveCustomInScene>();
            var autoSaveCustom = saveCustomGameObject.AddComponent<AutoSaveCustom>();

            // Assign references.
            saveCustomInScene.saveCustomObject = saveCustomObject;
            autoSaveCustom.saveCustomInScene = saveCustomInScene;

            // Persist across scenes.
            Object.DontDestroyOnLoad(saveCustomGameObject);

            Debug.Log($"{GetCallingMethodInfo()} - Save system initialized successfully.");
        }

        #endregion
    }
}