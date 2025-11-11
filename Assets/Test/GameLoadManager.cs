/*
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
[AddComponentMenu("Game/Save and Load/Game Load Manager")]
public class GameLoadManager : MonoBehaviour
{
    #region === Serialized Fields ===

    [Header("Player Settings")]
    [SerializeField, Tooltip("The player GameObject whose position and rotation will be saved and loaded.")]
    private GameObject player;

    [Header("Scene Objects")]
    [SerializeField, Tooltip("List of GameObjects to save and restore active states for.")]
    private List<GameObject> gameObjects = new();

    [Header("Events")]
    [Tooltip("Invoked when the game restarts or reloads after clearing saves.")]
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
            Debug.LogError("GameLoadManager disabled: Missing player reference.", this);
            enabled = false;
            return;
        }

        // Validate object list reference.
        if (gameObjects == null || gameObjects.Count == 0)
        {
            Debug.LogError("GameLoadManager disabled: GameObjects list is empty or unassigned.", this);
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
            Debug.Log("GameLoadManager: No saved data found or loading not allowed.", this);
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
            Debug.LogError("Save failed: Player reference is null. Script disabled.", this);
            enabled = false;
            return;
        }

        // Validate list.
        if (gameObjects == null || gameObjects.Count == 0)
        {
            Debug.LogError("Save failed: GameObjects list is empty. Script disabled.", this);
            enabled = false;
            return;
        }

        // Save object and player states.
        SetObjectsData(gameObjects);
        SetPlayerData(player);

        // Allow future loading.
        SetLoadingSave(true);

        Debug.Log("Game successfully saved.", this);
    }

    /// <summary>
    /// Loads saved data into the scene, restoring player and object states.
    /// </summary>
    private void LoadGame()
    {
        // Ensure valid references.
        if (player == null || gameObjects == null || gameObjects.Count == 0)
        {
            Debug.LogError("Load failed: Missing or invalid references.", this);
            return;
        }

        // Load data using GameSaveManager.
        GetObjectsData(gameObjects);
        GetPlayerData(player);

        // Restore time and audio systems.
        Time.timeScale = 1;
        AudioListener.pause = false;

        Debug.Log("Game successfully loaded.", this);
    }

    #endregion

    #region === Utilities ===

    /// <summary>
    /// Reloads the current scene after a specified delay.
    /// Useful for soft restarts or reinitialization of gameplay.
    /// </summary>
    /// <param name="delay">Time delay before reload, in seconds.</param>
    public void ReloadSceneTime(float delay)
    {
        Debug.LogWarning($"Scene reload scheduled in {delay} seconds.", this);
        Invoke(nameof(ReloadScene), delay);
    }

    /// <summary>
    /// Immediately reloads the current scene and triggers restart events.
    /// </summary>
    public void ReloadScene()
    {
        Debug.Log("Reloading current scene.", this);
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
        if (GUILayout.Button(new GUIContent("Clear Save Data", "Resets all save data to defaults.")))
        {
            ClearAllSaveData();
            Debug.Log("All save data cleared via inspector button.");
        }

        EditorGUILayout.Space(10f);

        // Default inspector rendering.
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

#endif