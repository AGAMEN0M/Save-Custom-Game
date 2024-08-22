/*
 * ---------------------------------------------------------------------------
 * Description: This script manages the saving and loading of game data within a scene. 
 *              It includes functionality to track in-game time, capture screenshots, 
 *              and save or load data such as scene information, elapsed time, and custom items. 
 *              The script supports multiple save paths and can store data locally or in PlayerPrefs. 
 *              It also handles resetting save data and ensures the correct camera is used for 
 *              capturing screenshots.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/
using System.Collections.Generic;
using System.IO;
using System;
using SaveCustomGame;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SaveCustomInScene : MonoBehaviour
{
    public SaveCustomObject saveCustomObject; // Reference to the SaveCustomObject.
    [SerializeField] private string gameTime = "00:00:00"; // Current in-game time.
    public string fileName = "SavingData"; // Default file name for saving data.
    public string savePath; // The path where the save file will be stored.
    public string sceneName; // Name of the current scene.
    public Camera sceneCamera; // Reference to the camera capturing the scene.

    private float elapsedTime = 0f; // Elapsed time since the start of the game.

    // Resets the save data.
    public void ResetSave()
    {
        elapsedTime = 0f; // reset the elapsed time.

        // Reset various properties in saveCustomObject.
        saveCustomObject.screenshot = null;
        saveCustomObject.gameTime = "00:00:00";
        saveCustomObject.sceneName = "";
    }

    // FixedUpdate is called at fixed intervals, primarily used for physics calculations.
    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime; // Increment the elapsed time by the fixed time interval.
        UpdateGameTime(); // Update the in-game time based on the elapsed time.
        UpdateSaveCustomObject(); // Update the SaveCustomObject with the current game time and scene name.
    }

    // UpdateSaveCustomObject method updates the SaveCustomObject with relevant game data.
    private void UpdateSaveCustomObject()
    {
        saveCustomObject.gameTime = gameTime; // Update the SaveCustomObject's gameTime variable with the current in-game time.

        // Get the name of the active scene and assign it to the SaveCustomObject's sceneName variable.
        sceneName = SceneManager.GetActiveScene().name;
        saveCustomObject.sceneName = sceneName;
    }

    // UpdateGameTime method converts elapsed time into a formatted in-game time.
    private void UpdateGameTime()
    {
        // Calculate hours, minutes, and seconds from the elapsed time.
        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        // Format the calculated time into a string in the "hours:minutes:seconds" format with at least 2 digits for hours, minutes, and seconds.
        gameTime = string.Format($"{hours:D2}:{minutes:D2}:{seconds:D2}");
    }

    // GetCamera method finds and assigns the appropriate camera for capturing the scene.
    private void GetCamera()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Find the GameObject tagged as "Player" in the scene.

        // Check if the playerObject is not null.
        if (playerObject != null)
        {
            // Try to get the Camera component from the playerObject.
            if (!playerObject.TryGetComponent<Camera>(out var playerCamera))
            {
                Camera[] childCameras = playerObject.GetComponentsInChildren<Camera>(); // If the Camera component is not found directly, try to get child Cameras.
                if (childCameras.Length > 0) { playerCamera = childCameras[0]; } // If child cameras exist, assign the first one found.
            }

            if (playerCamera != null) { sceneCamera = playerCamera; } // If playerCamera is found, assign it to the sceneCamera variable.
        }

        if (sceneCamera == null) { sceneCamera = Camera.main; } // If sceneCamera is still null, try to find the main Camera in the scene.

        // If no Camera is found yet, find any Camera in the scene.
        if (sceneCamera == null)
        {
            Camera[] allCameras = FindObjectsOfType<Camera>();
            if (allCameras.Length > 0) { sceneCamera = allCameras[0]; } // If any Cameras are found, assign the first one.
        }

        if (sceneCamera == null) { Debug.LogError("No cameras found at the scene."); } // If no Camera is found after searching, log an error message.
    }

    // SaveData method captures a screenshot and saves game data to a file.
    public void SaveData()
    {
        // Check if the sceneCamera reference is null.
        if (sceneCamera == null)
        {
            GetCamera(); // Get the appropriate camera to capture the scene.
        }

        SaveDataUtility.CaptureScreenshot(sceneCamera); // Capture a screenshot and update SaveCustomObject's screenshot data.

        // Create a SaveCustomFile object with relevant game data.
        SaveCustomFile data = new()
        {
            screenshot = saveCustomObject.screenshot,
            gameTime = saveCustomObject.gameTime,
            sceneName = saveCustomObject.sceneName,
            saveCustomItems = saveCustomObject.saveCustomItems,
            elapsedTime = elapsedTime,
        };
        
        string jsonData = JsonUtility.ToJson(data); // Convert the data to JSON format.

        try
        {
            string savePath = "";
            string directoryPath = "";

            // Determine the save path based on specified settings.
            if (saveCustomObject.localLow)
            {
                savePath = Path.Combine(Application.persistentDataPath, $"saves/{fileName}.json");
            }
            else if (saveCustomObject.playerPrefs)
            {
                // Save data using PlayerPrefs if configured.
                PlayerPrefs.SetString(fileName, jsonData);
                return;
            }
            else
            {
            #if UNITY_EDITOR
                savePath = Path.Combine(Application.dataPath, $"Editor/saves/{fileName}.json");
            #else
                savePath = Path.Combine(Application.dataPath, $"saves/{fileName}.json");
            #endif
            }

            // Extract directory path and create directory if it doesn't exist.
            directoryPath = Path.GetDirectoryName(savePath);
            Debug.Log($"Directory Path: {directoryPath}");

            if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }

            // Write JSON data to the file.
            File.WriteAllText(savePath, jsonData);
            Debug.Log($"Save Path: {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while saving data: {e.Message}"); // Log an error message if there's an exception while saving.
        }

        sceneCamera = null; // Reset the sceneCamera reference.
    }

    // LoadData method loads game data from a file or PlayerPrefs
    public void LoadData()
    {
        // If the saveCustomObject is configured to use PlayerPrefs.
        if (saveCustomObject.playerPrefs)
        {
            string jsonData = PlayerPrefs.GetString(fileName); // Retrieve saved data from PlayerPrefs using the specified fileName.
            var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize JSON data into a SaveCustomFile object.
            LoadDataString(data); // Load the data from PlayerPrefs.
        }
        else
        {
            // Check if the file exists at the specified savePath.
            if (File.Exists(savePath))
            {
                string jsonData = File.ReadAllText(savePath); // Read the JSON data from the file.
                var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize JSON data into a SaveCustomFile object.
                LoadDataString(data); // Load the data from the file.
            }
        }
    }

    // This method loads data from a SaveCustomFile object into SaveCustomObject or scene.
    private void LoadDataString(SaveCustomFile data)
    {
        // Assign the loaded data to the appropriate variables/components in SaveCustomObject or the scene.
        saveCustomObject.screenshot = data.screenshot;
        saveCustomObject.gameTime = data.gameTime;
        saveCustomObject.sceneName = data.sceneName;
        elapsedTime = data.elapsedTime;

        // Clear and load the SaveCustomItems from the loaded data.
        saveCustomObject.saveCustomItems.Clear();
        saveCustomObject.saveCustomItems = data.saveCustomItems;

        SceneManager.LoadScene(data.sceneName); // Load the scene associated with the saved data.
    }
}

// SaveCustomFile class represents the serialized data structure for saving and loading game data.
[System.Serializable]
public class SaveCustomFile
{
    public byte[] screenshot; // Byte array to store screenshot data.
    public string gameTime = "00:00:00"; // String to store in-game time information.
    public string sceneName; // String to store the name of the scene.
    public List<SaveCustomItem> saveCustomItems; // List to store custom items for saving.
    public float elapsedTime; // Floating-point value to store elapsed time.
}