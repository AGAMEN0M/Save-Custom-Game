/*
 * ---------------------------------------------------------------------------
 * Description: Handles scene-specific save and load operations for the game. 
 *              Tracks elapsed game time, manages screenshot capturing, updates 
 *              SaveCustomObject with current scene data, and serializes data to 
 *              disk or PlayerPrefs. Supports automatic camera detection and 
 *              provides reset functionality for save slots. Designed to work 
 *              seamlessly with the auto-save system and multiple storage options.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace SaveCustomGame
{
    [AddComponentMenu("UI/Save Custom Game/In Background/Save Custom In Scene")]
    public class SaveCustomInScene : MonoBehaviour
    {
        #region === Serialized Fields ===

        [Tooltip("Reference to the SaveCustomObject instance that holds save configuration and data.")]
        public SaveCustomObject saveCustomObject; // Reference to SaveCustomObject that stores game save data and settings.

        [SerializeField, Tooltip("Formatted in-game time string.")]
        private string gameTime = "00:00:00"; // Stores formatted elapsed gameplay time.

        [Tooltip("The filename used for saving data.")]
        public string fileName = "SavingData"; // Filename used when writing save data.

        [Tooltip("The full path where the save file will be stored.")]
        public string savePath; // Full resolved save path.

        [Tooltip("List of active scenes recorded during the save process.")]
        public List<string> sceneNames = new(); // Stores the names of currently loaded scenes.

        [Tooltip("Reference to the camera used for screenshot capturing.")]
        public Camera sceneCamera; // Reference to camera used to capture a screenshot for the save slot.

        #endregion

        #region === Private Fields ===

        private float elapsedTime = 0f; // Accumulates gameplay time.

        #endregion

        #region === Public Methods ===

        /// <summary>
        /// Resets in-game time and clears stored save data fields such as screenshot,
        /// formatted time value and scene list.
        /// </summary>
        public void ResetSave()
        {
            elapsedTime = 0f; // Reset total elapsed time to zero.
            saveCustomObject.screenshot = null; // Remove any previously saved screenshot.
            saveCustomObject.gameTime = "00:00:00"; // Reset stored time text.
            saveCustomObject.sceneNames.Clear(); // Remove stored scenes list.
        }

        /// <summary>
        /// Captures the current game state, takes a screenshot, stores scene info,
        /// converts the data to JSON and saves it either to PlayerPrefs or to disk
        /// depending on configured SaveCustomObject settings.
        /// </summary>
        public void SaveData()
        {
            // Ensure a valid camera exists for screenshot capture.
            if (sceneCamera == null) GetCamera(); // Attempt to locate and assign a camera.

            // Capture screenshot and assign byte data to saveCustomObject.screenshot.
            SaveDataUtility.CaptureScreenshot(sceneCamera); // Screenshot is processed internally.

            // Create a serializable data container with current save state values.
            SaveCustomFile data = new()
            {
                screenshot = saveCustomObject.screenshot,
                gameTime = saveCustomObject.gameTime,
                sceneNames = saveCustomObject.sceneNames,
                saveCustomItems = saveCustomObject.saveCustomItems,
                elapsedTime = elapsedTime,
            };

            // Convert data object into JSON format for storage.
            string jsonData = JsonUtility.ToJson(data); // Serialization to JSON.

            try
            {
                string savePath = ""; // Will hold the resolved save path.
                string directoryPath = ""; // Will store the directory path for validation.

                // Determine save location based on user configuration.
                if (saveCustomObject.localLow)
                {
                    // Save in Application.persistentDataPath (e.g., AppData LocalLow).
                    savePath = Path.Combine(Application.persistentDataPath, $"saves/{fileName}.json");
                }
                else if (saveCustomObject.playerPrefs)
                {
                    // Save JSON into PlayerPrefs rather than disk.
                    PlayerPrefs.SetString(fileName, jsonData); // Key-value storage.
                    return; // Skip file writing.
                }
                else
                {
                    // Save data into project folder or build directory.
                    string filePath = $"saves/{fileName}.json";

                #if UNITY_EDITOR
                    // When running inside the editor, save to Asset folder for accessibility.
                    savePath = Path.Combine(Application.dataPath, $"Editor/{filePath}");
                #else
                    // When running in a build, save alongside game files.
                    savePath = Path.Combine(Application.dataPath, filePath);
                #endif
                }

                // Extract directory component of the save path.
                directoryPath = Path.GetDirectoryName(savePath);

                // Ensure directory exists before attempting to write.
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                // Write JSON string to file.
                File.WriteAllText(savePath, jsonData); // File I/O operation.
            }
            catch (Exception e)
            {
                // Log save failure details.
                Debug.LogError($"Error while saving data: {e.Message}.", this);
            }

            // Clear the stored camera reference after saving to prevent stale camera usage.
            sceneCamera = null;
        }

        /// <summary>
        /// Loads stored save data from either disk or PlayerPrefs and restores saved
        /// scene states, elapsed time and stored screenshot snapshot.
        /// </summary>
        public void LoadData()
        {
            // Load from PlayerPrefs if selected.
            if (saveCustomObject.playerPrefs)
            {
                string jsonData = PlayerPrefs.GetString(fileName); // Retrieve stored JSON string.
                var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize into SaveCustomFile structure.
                LoadDataString(data); // Apply loaded data.
            }
            else if (File.Exists(savePath))
            {
                // Load from disk if file exists.
                string jsonData = File.ReadAllText(savePath); // Read raw JSON text from file.
                var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize data.
                LoadDataString(data); // Apply loaded values.
            }
        }

        #endregion

        #region === Private Methods ===

        private void FixedUpdate()
        {
            elapsedTime += Time.fixedDeltaTime; // Increment elapsed time each physics frame.
            UpdateGameTime(); // Convert elapsedTime into formatted text.
            UpdateSaveCustomObject(); // Update SaveCustomObject to reflect latest game state.
        }

        /// <summary>
        /// Loads JSON data into SaveCustomObject fields and restores scenes.
        /// </summary>
        private void LoadDataString(SaveCustomFile data)
        {
            saveCustomObject.screenshot = data.screenshot; // Load screenshot byte array.
            saveCustomObject.gameTime = data.gameTime; // Load formatted game time.
            saveCustomObject.sceneNames = data.sceneNames; // Copy saved scene list.
            elapsedTime = data.elapsedTime; // Restore gameplay time count.

            saveCustomObject.saveCustomItems.Clear(); // Clear existing stored items.
            saveCustomObject.saveCustomItems = data.saveCustomItems; // Restore saved items list.

            // Restore loaded scenes in the correct additive order.
            if (data.sceneNames != null && data.sceneNames.Count > 0)
            {
                SceneManager.LoadScene(data.sceneNames[0]); // Load primary scene.

                // Load additional scenes without unloading first scene.
                for (int i = 1; i < data.sceneNames.Count; i++)
                {
                    SceneManager.LoadScene(data.sceneNames[i], LoadSceneMode.Additive); // Load successive scenes.
                }
            }
        }

        /// <summary>
        /// Updates SaveCustomObject with current game time string and active scenes list.
        /// </summary>
        private void UpdateSaveCustomObject()
        {
            saveCustomObject.gameTime = gameTime; // Store updated formatted time.
            sceneNames.Clear(); // Clear list before repopulating.

            // Iterate through all currently loaded scenes and record their names.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i); // Retrieve scene index reference.
                sceneNames.Add(scene.name); // Store scene name.
            }

            saveCustomObject.sceneNames = sceneNames; // Assign updated scene list.
        }

        /// <summary>
        /// Converts elapsed time to formatted HH:MM:SS string.
        /// </summary>
        private void UpdateGameTime()
        {
            // Calculate hours, minutes and seconds.
            int hours = Mathf.FloorToInt(elapsedTime / 3600f);
            int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            // Format the numeric values into a readable string.
            gameTime = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        /// Attempts to find a valid camera in the scene to use for screenshot capture.
        /// </summary>
        private void GetCamera()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player"); // Attempt to locate player object.

            if (playerObject != null)
            {
                // Attempt to get camera directly from player object.
                if (!playerObject.TryGetComponent<Camera>(out var playerCamera))
                {
                    // If player does not have a Camera, search its children.
                    var childCameras = playerObject.GetComponentsInChildren<Camera>();
                    if (childCameras.Length > 0) playerCamera = childCameras[0]; // Use the first available camera.
                }

                if (playerCamera != null) sceneCamera = playerCamera; // Assign discovered camera.
            }

            if (sceneCamera == null) sceneCamera = Camera.main; // Fallback to main camera.

            if (sceneCamera == null)
            {
                // If no main camera exists, search all cameras in the scene.
                var allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
                if (allCameras.Length > 0) sceneCamera = allCameras[0]; // Assign first found.
            }

            // Final fallback error warning if still missing.
            if (sceneCamera == null) Debug.LogError("No cameras found at the scene.", this);
        }

        #endregion
    }

    #region === SaveCustomFile Structure ===

    /// <summary>
    /// Serializable data structure used to save and load game state.
    /// </summary>
    [Serializable]
    public class SaveCustomFile
    {
        [Tooltip("Stored screenshot byte data.")]
        public byte[] screenshot;

        [Tooltip("Recorded formatted game time.")]
        public string gameTime = "00:00:00";

        [Tooltip("Names of active scenes at the moment of saving.")]
        public List<string> sceneNames;

        [Tooltip("List of custom objects to save along with the game state.")]
        public List<SaveCustomItem> saveCustomItems;

        [Tooltip("Elapsed game time in seconds.")]
        public float elapsedTime;
    }

    #endregion
}