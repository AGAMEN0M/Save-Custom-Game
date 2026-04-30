/*
 * ---------------------------------------------------------------------------
 * Description: Provides menu options to automatically create base C# scripts for the Save Custom Game system.
 * 
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEditor;
using UnityEngine;
using System.IO;

namespace SaveCustomGame.Editor
{
    /// <summary>
    /// Provides methods to create base C# script files for Save Custom Game functionality.
    /// </summary>
    public class CreateBaseScript
    {
        #region === Script Creation Utility ===

        /// <summary>
        /// Creates a new C# script file with the specified name and source code.
        /// </summary>
        /// <param name="baseName">The name of the script file (without extension).</param>
        /// <param name="baseCode">The source code content to be written inside the new script file.</param>
        private static void CreateScript(string baseName, string baseCode)
        {
            // Determine the selected folder in the Project view.
            string selectedPath = "Assets";
            var selectedObject = Selection.activeObject;

            // If a folder or asset is selected, get its path.
            if (selectedObject != null)
            {
                selectedPath = AssetDatabase.GetAssetPath(selectedObject);

                // If the selected path is a file, get its directory instead.
                if (File.Exists(selectedPath))
                {
                    selectedPath = Path.GetDirectoryName(selectedPath);
                }
            }

            // Define the final script path.
            string scriptPath = Path.Combine(selectedPath, $"{baseName}.cs");

            // Prevent overwriting an existing file.
            if (File.Exists(scriptPath))
            {
                Debug.LogWarning($"A script named '{baseName}.cs' already exists at: {selectedPath}");
                return;
            }

            // Write the provided source code into the new script file.
            File.WriteAllText(scriptPath, baseCode);

            // Refresh the AssetDatabase to make Unity detect the new script.
            AssetDatabase.Refresh();

            // Load the newly created asset.
            var asset = AssetDatabase.LoadAssetAtPath<Object>(scriptPath);

            // Select the new script in the Project view.
            Selection.activeObject = asset;

            // Log confirmation in the Console.
            Debug.Log($"Script created successfully at: {scriptPath}");
        }

        #endregion

        #region === Menu Items ===

        /// <summary>
        /// Creates a new base script for the Game Save Manager.
        /// </summary>
        [MenuItem("Assets/Create/Tools/Save Custom Game/Game Save Manager (Script)")]
        public static void CreateSaveManager()
        {
            #region === Code ===

            string code =
@"/*
 * ---------------------------------------------------------------------------
 * Description: The base script is responsible for saving and loading player data,
 *              scene data, and object states into the Custom Save Game system.
 * 
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Collections.Generic;
using SaveCustomGame;
using UnityEngine;

using static SaveCustomGame.SaveDataUtility;

/// <summary>
/// Manages the saving and loading of player, scene, and object data in the current scene.
/// </summary>
public static class GameSaveManager
{
    #region === Fields and Keys ===

    #region < Scene Save Data >

    /// <summary>Holds a reference to the SaveCustomInScene component used for scene-level saving.</summary>
    private static SaveCustomInScene sceneSaveData;

    #endregion

    #region < General Save Keys >

    /// <summary>Root key for general save data.</summary>
    private const string gameSaveKey = ""GameSave"";
    /// <summary>Key that stores whether the game is allowed to load saved data.</summary>
    private const string allowedKey = ""IsAllowed"";

    #endregion

    #region < Player Save Keys >

    /// <summary>Root key for player save data.</summary>
    private const string playerKey = ""Player"";
    /// <summary>Key used to store the player position.</summary>
    private const string playerPositionKey = ""Position"";
    /// <summary>Key used to store the player rotation.</summary>
    private const string playerRotationKey = ""Rotation"";

    #endregion

    #region < Object Save Keys >

    /// <summary>Expected number of objects to save or load.</summary>
    private const int listCount = 2;
    /// <summary>Root key for object save data.</summary>
    private const string objectKey = ""Object"";
    /// <summary>Key for the first object's state.</summary>
    private const string objectState1Key = ""State1"";
    /// <summary>Key for the second object's state.</summary>
    private const string objectState2Key = ""State2"";

    #endregion

    #endregion

    #region === Public Methods ===

    /// <summary>
    /// Clears all saved data, resets the scene save state, 
    /// and restores default player position, rotation, and object states.
    /// </summary>
    public static void ClearAllSaveData()
    {
        // Retrieve the SaveCustomInScene component responsible for managing scene save data.
        sceneSaveData = GetComponentSaveCustomInScene();

        // Reset scene data if available.
        if (sceneSaveData != null) sceneSaveData.ResetSave();

        // Disable loading flag to prevent accidental load attempts.
        SetLoadingSave(false);

        // Reset the player's position and rotation to defaults.
        SetVector(playerKey, playerPositionKey, Vector3.zero);
        SetVector(playerKey, playerRotationKey, Quaternion.identity);

        // Reset object states to default active/inactive configuration.
        SetBool(objectKey, objectState1Key, true);
        SetBool(objectKey, objectState2Key, false);
    }

    /// <summary>
    /// Sets whether the game is currently allowed to load save data.
    /// </summary>
    /// <param name=""state"">True if loading is allowed; otherwise false.</param>
    public static void SetLoadingSave(bool state) => SetBool(gameSaveKey, allowedKey, state);

    /// <summary>
    /// Returns whether the game is currently allowed to load save data.
    /// </summary>
    /// <returns>True if loading is allowed, otherwise false.</returns>
    public static bool GetLoadingSave() => GetBool(gameSaveKey, allowedKey);

    /// <summary>
    /// Saves the current position and rotation of the specified player instance.
    /// </summary>
    /// <param name=""playerInstance"">The player GameObject to save.</param>
    public static void SetPlayerData(GameObject playerInstance)
    {
        if (playerInstance == null)
        {
            Debug.LogError(""[GameSaveManager] Player instance is null. Cannot save player data."");
            return;
        }

        // Save the player's current transform state.
        SetVector(playerKey, playerPositionKey, playerInstance.transform.position);
        SetVector(playerKey, playerRotationKey, playerInstance.transform.rotation);
    }

    /// <summary>
    /// Loads and applies the saved position and rotation to the specified player instance.
    /// </summary>
    /// <param name=""playerInstance"">The player GameObject to update.</param>
    public static void GetPlayerData(GameObject playerInstance)
    {
        if (playerInstance == null)
        {
            Debug.LogError(""[GameSaveManager] Player instance is null. Cannot load player data."");
            return;
        }

        // Retrieve saved position from storage.
        Vector3 position = GetVector(playerKey, playerPositionKey);

        // Retrieve saved rotation (stored as Vector4) and reconstruct a Quaternion.
        Vector4 rot = GetVector(playerKey, playerRotationKey);
        Quaternion rotation = new(rot.x, rot.y, rot.z, rot.w);

        // Apply the loaded position and rotation to the player instance.
        playerInstance.transform.SetPositionAndRotation(position, rotation);
    }

    /// <summary>
    /// Saves the active state of objects within the scene.
    /// </summary>
    /// <param name=""gameObjects"">The list of GameObjects whose states should be saved.</param>
    public static void SetObjectsData(List<GameObject> gameObjects)
    {
        // Validate list before proceeding.
        if (gameObjects == null || gameObjects.Count != listCount)
        {
            Debug.LogError($""[GameSaveManager] Invalid object list. Expected {listCount} objects."");
            return;
        }

        // Save the active state for each object.
        SetBool(objectKey, objectState1Key, gameObjects[0].activeSelf);
        SetBool(objectKey, objectState2Key, gameObjects[1].activeSelf);
    }

    /// <summary>
    /// Loads and applies saved active states to the specified scene objects.
    /// </summary>
    /// <param name=""gameObjects"">The list of GameObjects to restore states for.</param>
    public static void GetObjectsData(List<GameObject> gameObjects)
    {
        // Validate list before proceeding.
        if (gameObjects == null || gameObjects.Count != listCount)
        {
            Debug.LogError($""[GameSaveManager] Invalid object list. Expected {listCount} objects."");
            return;
        }

        // Apply saved active states to the provided objects.
        gameObjects[0].SetActive(GetBool(objectKey, objectState1Key));
        gameObjects[1].SetActive(GetBool(objectKey, objectState2Key));
    }

    #endregion
}";

            #endregion

            CreateScript("GameSaveManager", code);
        }

        /// <summary>
        /// Creates a new base script for the Game Load Manager.
        /// </summary>
        [MenuItem("Assets/Create/Tools/Save Custom Game/Game Load Manager (Script)")]
        public static void CreateLoadManager()
        {
            #region === Code ===

            string code =
@"/*
 * ---------------------------------------------------------------------------
 * Description: This base script is responsible for loading and saving game data,
 *              including player and object states. It integrates with GameSaveManager
 *              to manage persistent scene data.
 * 
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using static GameSaveManager;

/// <summary>
/// Manages the process of saving and loading gameplay data such as player
/// position, rotation, and object states. Also provides editor utilities
/// for clearing save data.
/// </summary>
[AddComponentMenu(""Game/Save and Load/Game Load Manager"")]
public class GameLoadManager : MonoBehaviour
{
    #region === Serialized Fields ===

    [Header(""Player Settings"")]
    [SerializeField, Tooltip(""The player GameObject whose position and rotation will be saved and loaded."")]
    private GameObject player;

    [Header(""Scene Objects"")]
    [SerializeField, Tooltip(""List of GameObjects to save and restore active states for."")]
    private List<GameObject> gameObjects = new();

    [Header(""Events"")]
    [Tooltip(""Invoked when the game restarts or reloads after clearing saves."")]
    public UnityEvent OnGameRestart;

    #endregion

    #region === Properties ===

    /// <summary>
    /// Gets or sets the player GameObject.
    /// </summary>
    public GameObject Player
    {
        get => player;
        set => player = value;
    }

    /// <summary>
    /// Gets or sets the list of GameObjects managed by the save system.
    /// </summary>
    public List<GameObject> GameObjects
    {
        get => gameObjects;
        set => gameObjects = value;
    }

    #endregion

    #region === Unity Methods ===

    /// <summary>
    /// Unity Start method that checks if save data exists and loads it if allowed.
    /// </summary>
    private void Start()
    {
        // Validate player reference.
        if (!player)
        {
            Debug.LogError(""GameLoadManager disabled: Missing player reference."", this);
            enabled = false;
            return;
        }

        // Validate object list reference.
        if (gameObjects == null || gameObjects.Count == 0)
        {
            Debug.LogError(""GameLoadManager disabled: GameObjects list is empty or unassigned."", this);
            enabled = false;
            return;
        }

        // Load game data if allowed.
        if (GetLoadingSave())
        {
            LoadGame();
        }
        else
        {
            Debug.Log(""GameLoadManager: No saved data found or loading not allowed."", this);
        }
    }

    #endregion

    #region === Save/Load ===

    /// <summary>
    /// Saves the player's current state and the active states of scene objects.
    /// </summary>
    public void SaveGame()
    {
        // Validate player.
        if (player == null)
        {
            Debug.LogError(""Save failed: Player reference is null. Script disabled."", this);
            enabled = false;
            return;
        }

        // Validate list.
        if (gameObjects == null || gameObjects.Count == 0)
        {
            Debug.LogError(""Save failed: GameObjects list is empty. Script disabled."", this);
            enabled = false;
            return;
        }

        // Save object and player states.
        SetObjectsData(gameObjects);
        SetPlayerData(player);

        // Allow future loading.
        SetLoadingSave(true);

        Debug.Log(""Game successfully saved."", this);
    }

    /// <summary>
    /// Loads saved data into the scene, restoring player and object states.
    /// </summary>
    private void LoadGame()
    {
        // Ensure valid references.
        if (player == null || gameObjects == null || gameObjects.Count == 0)
        {
            Debug.LogError(""Load failed: Missing or invalid references."", this);
            return;
        }

        // Load data using GameSaveManager.
        GetObjectsData(gameObjects);
        GetPlayerData(player);

        // Restore time and audio systems.
        Time.timeScale = 1;
        AudioListener.pause = false;

        Debug.Log(""Game successfully loaded."", this);
    }

    #endregion

    #region === Utilities ===

    /// <summary>
    /// Reloads the current scene after a specified delay.
    /// Useful for soft restarts or reinitialization of gameplay.
    /// </summary>
    /// <param name=""delay"">Time delay before reload, in seconds.</param>
    public void ReloadSceneTime(float delay)
    {
        Debug.LogWarning($""Scene reload scheduled in {delay} seconds."", this);
        Invoke(nameof(ReloadScene), delay);
    }

    /// <summary>
    /// Immediately reloads the current scene and triggers restart events.
    /// </summary>
    public void ReloadScene()
    {
        Debug.Log(""Reloading current scene."", this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Invoke restart event.
        OnGameRestart?.Invoke();
    }

    #endregion
}

#if UNITY_EDITOR

#region === Custom Inspector ===

/// <summary>
/// Custom inspector for GameLoadManager, adding a Clear Save button.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(GameLoadManager))]
public class GameLoadManagerInspector : Editor
{
    /// <summary>
    /// Draws the custom inspector interface.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Custom button for clearing save data.
        if (GUILayout.Button(new GUIContent(""Clear Save Data"", ""Resets all save data to defaults."")))
        {
            ClearAllSaveData();
            Debug.Log(""All save data cleared via inspector button."");
        }

        EditorGUILayout.Space(10f);

        // Default inspector rendering.
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#endif";

            #endregion

            CreateScript("GameLoadManager", code);
        }

        #endregion
    }
}