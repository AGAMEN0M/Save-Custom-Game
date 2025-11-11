/*
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
    private const string gameSaveKey = "GameSave";
    /// <summary>Key that stores whether the game is allowed to load saved data.</summary>
    private const string allowedKey = "IsAllowed";

    #endregion

    #region < Player Save Keys >

    /// <summary>Root key for player save data.</summary>
    private const string playerKey = "Player";
    /// <summary>Key used to store the player position.</summary>
    private const string playerPositionKey = "Position";
    /// <summary>Key used to store the player rotation.</summary>
    private const string playerRotationKey = "Rotation";

    #endregion

    #region < Object Save Keys >

    /// <summary>Expected number of objects to save or load.</summary>
    private const int listCount = 2;
    /// <summary>Root key for object save data.</summary>
    private const string objectKey = "Object";
    /// <summary>Key for the first object's state.</summary>
    private const string objectState1Key = "State1";
    /// <summary>Key for the second object's state.</summary>
    private const string objectState2Key = "State2";

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
    /// <param name="state">True if loading is allowed; otherwise false.</param>
    public static void SetLoadingSave(bool state) => SetBool(gameSaveKey, allowedKey, state);

    /// <summary>
    /// Returns whether the game is currently allowed to load save data.
    /// </summary>
    /// <returns>True if loading is allowed, otherwise false.</returns>
    public static bool GetLoadingSave() => GetBool(gameSaveKey, allowedKey);

    /// <summary>
    /// Saves the current position and rotation of the specified player instance.
    /// </summary>
    /// <param name="playerInstance">The player GameObject to save.</param>
    public static void SetPlayerData(GameObject playerInstance)
    {
        if (playerInstance == null)
        {
            Debug.LogError("[GameSaveManager] Player instance is null. Cannot save player data.");
            return;
        }

        // Save the player's current transform state.
        SetVector(playerKey, playerPositionKey, playerInstance.transform.position);
        SetVector(playerKey, playerRotationKey, playerInstance.transform.rotation);
    }

    /// <summary>
    /// Loads and applies the saved position and rotation to the specified player instance.
    /// </summary>
    /// <param name="playerInstance">The player GameObject to update.</param>
    public static void GetPlayerData(GameObject playerInstance)
    {
        if (playerInstance == null)
        {
            Debug.LogError("[GameSaveManager] Player instance is null. Cannot load player data.");
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
    /// <param name="gameObjects">The list of GameObjects whose states should be saved.</param>
    public static void SetObjectsData(List<GameObject> gameObjects)
    {
        // Validate list before proceeding.
        if (gameObjects == null || gameObjects.Count != listCount)
        {
            Debug.LogError($"[GameSaveManager] Invalid object list. Expected {listCount} objects.");
            return;
        }

        // Save the active state for each object.
        SetBool(objectKey, objectState1Key, gameObjects[0].activeSelf);
        SetBool(objectKey, objectState2Key, gameObjects[1].activeSelf);
    }

    /// <summary>
    /// Loads and applies saved active states to the specified scene objects.
    /// </summary>
    /// <param name="gameObjects">The list of GameObjects to restore states for.</param>
    public static void GetObjectsData(List<GameObject> gameObjects)
    {
        // Validate list before proceeding.
        if (gameObjects == null || gameObjects.Count != listCount)
        {
            Debug.LogError($"[GameSaveManager] Invalid object list. Expected {listCount} objects.");
            return;
        }

        // Apply saved active states to the provided objects.
        gameObjects[0].SetActive(GetBool(objectKey, objectState1Key));
        gameObjects[1].SetActive(GetBool(objectKey, objectState2Key));
    }

    #endregion
}