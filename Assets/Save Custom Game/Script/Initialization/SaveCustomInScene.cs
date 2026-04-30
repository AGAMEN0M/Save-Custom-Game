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
    [AddComponentMenu("Tools/Save Custom Game/In Background/Save Custom In Scene")]
    public class SaveCustomInScene : MonoBehaviour
    {
        #region === Serialized Fields ===

        [Tooltip("Reference to the SaveCustomObject instance that holds save configuration and data.")]
        public SaveCustomObject saveCustomObject;

        [SerializeField, Tooltip("Formatted in-game time string.")]
        private string gameTime = "00:00:00";

        [Tooltip("The filename used for saving data.")]
        public string fileName = "SavingData";

        [Tooltip("The full path where the save file will be stored.")]
        public string savePath;

        [Tooltip("List of active scenes recorded during the save process.")]
        public List<string> sceneNames = new();

        [Tooltip("Reference to the camera used for screenshot capturing.")]
        public Camera sceneCamera;

        #endregion

        #region === Private Fields ===

        /// <summary>
        /// Accumulates total elapsed gameplay time in seconds.
        /// Used to generate the formatted game time string and persist progress.
        /// </summary>
        private float elapsedTime = 0f;

        #endregion

        #region === Public Methods ===

        /// <summary>
        /// Resets all runtime save-related data to initial state.
        /// Clears elapsed time, screenshot, formatted time string, and stored scene list.
        /// </summary>
        public void ResetSave()
        {
            elapsedTime = 0f; // Reset total elapsed gameplay time.
            saveCustomObject.screenshot = null; // Clear stored screenshot data.
            saveCustomObject.gameTime = "00:00:00"; // Reset formatted game time string.
            saveCustomObject.sceneNames.Clear(); // Clear stored scene names list.
        }

        /// <summary>
        /// Captures current game state and persists it to storage.
        /// Includes screenshot capture, scene tracking, and serialization to JSON.
        /// Supports saving to PlayerPrefs or file system depending on configuration.
        /// </summary>
        public void SaveData()
        {
            // Ensure a valid camera exists before capturing screenshot.
            if (sceneCamera == null) GetCamera();

            // Capture screenshot and store it inside SaveCustomObject.
            SaveDataUtility.CaptureScreenshot(sceneCamera);

            // Create serializable container with current game state.
            SaveCustomFile data = new()
            {
                screenshot = saveCustomObject.screenshot,
                gameTime = saveCustomObject.gameTime,
                sceneNames = saveCustomObject.sceneNames,
                saveCustomItems = saveCustomObject.saveCustomItems,
                elapsedTime = elapsedTime,
            };

            string jsonData = JsonUtility.ToJson(data); // Convert data to JSON format.

            try
            {
                string directoryPath = "";

                // Determine save location based on configuration.
                if (saveCustomObject.localLow)
                {
                    // Save to persistent data path (recommended for builds).
                    savePath = Path.Combine(Application.persistentDataPath, $"saves/{fileName}.json");
                }
                else if (saveCustomObject.playerPrefs)
                {
                    // Save using PlayerPrefs key-value system.
                    PlayerPrefs.SetString(fileName, jsonData);
                    PlayerPrefs.Save(); // Ensure data is written immediately.
                    return;
                }
                else
                {
                    string filePath = $"saves/{fileName}.json"; // Save to project/build directory.

                #if UNITY_EDITOR
                    savePath = Path.Combine(Application.dataPath, $"Editor/{filePath}"); // Editor-specific path.
                #else
                    savePath = Path.Combine(Application.dataPath, filePath); // Runtime build path.
                #endif
                }

                // Extract directory path from full save path.
                directoryPath = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(savePath, jsonData); // Write JSON data to file.
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while saving data: {e.Message}.", this);
            }

            sceneCamera = null; // Clear camera reference to avoid stale references.
        }

        /// <summary>
        /// Loads saved game data from PlayerPrefs or file system.
        /// Validates JSON content and restores game state if successful.
        /// </summary>
        public void LoadData()
        {
            string jsonData = null;

            // Load from PlayerPrefs if enabled.
            if (saveCustomObject.playerPrefs)
            {
                jsonData = PlayerPrefs.GetString(fileName);
            }
            else if (File.Exists(savePath)) // Otherwise attempt to load from file.
            {
                jsonData = File.ReadAllText(savePath);
            }

            // Validate loaded data.
            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogWarning("Save data is empty or missing.", this);
                return;
            }

            // Deserialize JSON into data structure.
            var data = JsonUtility.FromJson<SaveCustomFile>(jsonData);
            if (data == null)
            {
                Debug.LogError("Failed to deserialize save data.", this);
                return;
            }

            LoadDataString(data); // Apply loaded data to runtime state.
        }

        #endregion

        #region === Private Methods ===

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            UpdateGameTime();
            UpdateSaveCustomObject();
        }

        /// <summary>
        /// Applies deserialized save data to the current runtime state.
        /// Restores screenshot, game time, scene list, and custom saved items.
        /// </summary>
        /// <param name="data">Deserialized save data container.</param>
        private void LoadDataString(SaveCustomFile data)
        {
            // Restore basic save data.
            saveCustomObject.screenshot = data.screenshot;
            saveCustomObject.gameTime = data.gameTime;
            saveCustomObject.sceneNames = data.sceneNames;
            elapsedTime = data.elapsedTime; // Restore elapsed gameplay time.

            // Replace existing saved items with loaded ones.
            saveCustomObject.saveCustomItems.Clear();
            saveCustomObject.saveCustomItems = data.saveCustomItems;

            // Restore scenes if any were saved.
            if (data.sceneNames != null && data.sceneNames.Count > 0)
            {
                // Load primary scene.
                SceneManager.LoadScene(data.sceneNames[0]);

                // Load additional scenes additively.
                for (int i = 1; i < data.sceneNames.Count; i++)
                {
                    SceneManager.LoadScene(data.sceneNames[i], LoadSceneMode.Additive);
                }
            }
        }

        /// <summary>
        /// Updates SaveCustomObject with the latest runtime data.
        /// Synchronizes formatted time and currently loaded scenes.
        /// </summary>
        private void UpdateSaveCustomObject()
        {
            saveCustomObject.gameTime = gameTime; // Update formatted game time.
            sceneNames.Clear(); // Clear previous scene list.

            // Iterate through all loaded scenes.
            int count = SceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                sceneNames.Add(scene.name); // Store scene name.
            }

            // Assign updated scene list back to SaveCustomObject.
            saveCustomObject.sceneNames = sceneNames;
        }

        /// <summary>
        /// Converts elapsed time in seconds into a formatted HH:MM:SS string.
        /// </summary>
        private void UpdateGameTime()
        {
            // Calculate hours, minutes, and seconds.
            int hours = Mathf.FloorToInt(elapsedTime / 3600f);
            int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            // Format time string.
            gameTime = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        /// Attempts to locate a valid camera in the scene for screenshot capture.
        /// Priority: Player camera → Main camera → Any available camera.
        /// </summary>
        private void GetCamera()
        {
            // Try to find player object by tag.
            var playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                // Attempt to get camera directly from player.
                if (!playerObject.TryGetComponent<Camera>(out var playerCamera))
                {
                    // If not found, search in children.
                    var childCameras = playerObject.GetComponentsInChildren<Camera>();
                    if (childCameras.Length > 0) playerCamera = childCameras[0];
                }

                // Assign if found.
                if (playerCamera != null)
                {
                    sceneCamera = playerCamera;
                    return;
                }
            }

            sceneCamera = Camera.main; // Fallback to main camera.

            if (sceneCamera == null)
            {
                // Final fallback: find any camera in scene.
                var allCameras = FindObjectsByType<Camera>();
                if (allCameras.Length > 0) sceneCamera = allCameras[0];
            }

            // Log error if no camera found.
            if (sceneCamera == null) Debug.LogError("No cameras found in the scene.", this);
        }

        #endregion
    }

    #region === SaveCustomFile Structure ===

    /// <summary>
    /// Serializable container used to persist and restore game state.
    /// Includes screenshot, time data, scene list, and custom saved values.
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