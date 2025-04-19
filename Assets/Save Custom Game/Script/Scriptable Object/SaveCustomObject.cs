/*
 * ---------------------------------------------------------------------------
 * Description: Defines a ScriptableObject used for storing custom save data, 
 *              such as screenshots, scene information, game time, and various 
 *              user-defined values (vectors, floats, ints, strings, and bools). 
 *              Also includes Unity Editor support for asset creation and configuration. 
 *              Serves as the core data structure for the SaveCustomGame system.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Collections.Generic;
using UnityEngine;

// This section checks if the code is being executed in Unity Editor to prevent compilation errors in a build.
#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Provides a menu item to create and manage a SaveCustomObject asset within the Unity Editor.
/// </summary>
public class KeyboardControlListCreator
{
    /// <summary>
    /// Creates a new SaveCustomObject asset in the Resources folder. If the asset already exists,
    /// prompts the user to replace it.
    /// </summary>
    [MenuItem("Assets/Create/Save Custom Game/Save Custom Object Data")]
    public static void CreateCustomObjectData()
    {
        string path = "Assets/Resources";
        string assetPath = $"{path}/Save Custom Object Data.asset";

        // Create a Resources folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(path)) { AssetDatabase.CreateFolder("Assets", "Resources"); }

        // Check if the asset already exists, and prompt the user to replace it.
        if (AssetDatabase.LoadAssetAtPath<SaveCustomGame.SaveCustomObject>(assetPath) != null)
        {
            if (!EditorUtility.DisplayDialog("Replace File", "There is already a 'Save Custom Object Data'. Do you want to replace it?", "Yes", "No"))
            {
                return;
            }
        }

        // Create an instance of SaveCustomObject and save it as an asset.
        var asset = ScriptableObject.CreateInstance<SaveCustomGame.SaveCustomObject>();
        AssetDatabase.CreateAsset(asset, assetPath);
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select the created asset in the Project window for easy access.
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
#endif

namespace SaveCustomGame
{
    /// <summary>
    /// ScriptableObject for storing game settings, autosave options, and customizable items such as
    /// vectors, floats, ints, strings, and bools.
    /// </summary>
    public class SaveCustomObject : ScriptableObject
    {
        [Header("Settings")]
        public byte[] screenshot; // Stores a screenshot as a byte array.
        public int pixelLimit = 1000; // Pixel limit for screenshot.
        public string gameTime = "00:00:00"; // Tracks the game's time.
        public string sceneName; // Stores the name of the scene.
        [Space(10)]
        [Header("Auto Save Settings")]
        public bool autosaveEnabled; // Controls whether autosaving is enabled.
        [Space(5)]
        // Conditions for triggering saves.
        public bool saveGameByEvent;
        public bool saveGameByTime;
        [Space(5)]
        public float saveInterval = 60f; // Time interval for autosaving.
        [Space(10)]
        [Header("Saving Mode Settings")]
        // Different saving modes.
        public bool gameData;
        public bool localLow;
        public bool playerPrefs;
        [Space(10)]
        [Header("Custom Items Settings")]
        public List<SaveCustomItem> saveCustomItems; // List of custom items with various data types.
    }

    /// <summary>
    /// Serializable container for a set of customizable item types, each with a specific data representation.
    /// </summary>
    [System.Serializable]
    public class SaveCustomItem
    {
        [Header("Item Settings")]
        public string itemTag; // Identification tag for the custom item.
        [Space(10)]
        [Header("Definitions")]
        // Definitions for various data types.
        public List<SaveCustomVector> itemVector;
        public List<SaveCustomFloat> itemFloat;
        public List<SaveCustomInt> itemInt;
        public List<SaveCustomString> itemString;
        public List<SaveCustomBool> itemBool;
    }

    /// <summary>
    /// Serializable data structure for storing a vector value with a descriptive tag.
    /// </summary>
    [System.Serializable]
    public class SaveCustomVector
    {
        public string vectorTag;
        public Vector4 vectorValue;
    }

    /// <summary>
    /// Serializable data structure for storing a float value with a descriptive tag.
    /// </summary>
    [System.Serializable]
    public class SaveCustomFloat
    {
        public string floatTag; // Identification tag for the float value.
        public float floatValue; // Actual float value to be saved.
    }

    /// <summary>
    /// Serializable data structure for storing an integer value with a descriptive tag.
    /// </summary>
    [System.Serializable]
    public class SaveCustomInt
    {
        public string intTag; // Identification tag for the int value.
        public int intValue; // Actual int value to be saved.
    }

    /// <summary>
    /// Serializable data structure for storing a string value with a descriptive tag.
    /// </summary>
    [System.Serializable]
    public class SaveCustomString
    {
        public string stringTag; // Identification tag for the string value.
        public string stringValue; // Actual string value to be saved.
    }

    /// <summary>
    /// Serializable data structure for storing a boolean value with a descriptive tag.
    /// </summary>
    [System.Serializable]
    public class SaveCustomBool
    {
        public string boolTag; // Identification tag for the bool value.
        public bool boolValue; // Actual bool value to be saved.
    }
}