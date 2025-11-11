/*
 * ---------------------------------------------------------------------------
 * Description: Defines a ScriptableObject used for storing custom save data, 
 *              such as screenshots, scene information, game time, and various 
 *              user-defined values (vectors, floats, ints, strings, and bools). 
 *              Also includes Unity Editor support for asset creation and configuration. 
 *              Serves as the core data structure for the SaveCustomGame system.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SaveCustomGame
{
    #region === Save Custom Object ===

    /// <summary>
    /// ScriptableObject for storing game settings, autosave options, screenshot data, 
    /// scene lists, and customizable item lists with support for multiple data types.
    /// </summary>
    public class SaveCustomObject : ScriptableObject
    {
        #region === Screenshot and Scene Settings ===

        [Header("Settings")]
        [Tooltip("Stores the screenshot data as a byte array for preview or restoration.")]
        public byte[] screenshot; // Stores a screenshot as a byte array.

        [Tooltip("Defines the maximum screenshot resolution to avoid memory overhead.")]
        public int pixelLimit = 1000; // Pixel limit for screenshot.

        [Tooltip("Stores the formatted game time string.")]
        public string gameTime = "00:00:00"; // Tracks the game's time.

        [Tooltip("Holds the list of currently loaded or relevant scenes.")]
        public List<string> sceneNames; // Stores the name of the scene.

        #endregion

        #region === Auto Save Settings ===

        [Header("Auto Save Settings")]
        [Tooltip("Enables or disables the autosave feature.")]
        public bool autosaveEnabled; // Controls whether autosaving is enabled.

        [Tooltip("Triggers save based on custom events.")]
        public bool saveGameByEvent; // Controls event-triggered saving.

        [Tooltip("Triggers save based on timed intervals.")]
        public bool saveGameByTime; // Controls time-based saving.

        [Tooltip("Defines how often autosave occurs when enabled.")]
        public float saveInterval = 60f; // Time interval for autosaving.

        #endregion

        #region === Saving Mode Settings ===

        [Header("Saving Mode Settings")]
        [Tooltip("When enabled, the system saves in local game data storage.")]
        public bool gameData;

        [Tooltip("When enabled, the system saves in Application.LocalLow.")]
        public bool localLow;

        [Tooltip("When enabled, the system saves using PlayerPrefs.")]
        public bool playerPrefs;

        #endregion

        #region === Custom Data Lists ===

        [Header("Custom Items Settings")]
        [Tooltip("List of custom items containing multiple custom data types.")]
        public List<SaveCustomItem> saveCustomItems; // List of custom items with various data types.

        #endregion
    }

    #endregion

    #region === Save Custom Item ===

    /// <summary>
    /// Container for multiple structured data types grouped under a tag.
    /// </summary>
    [Serializable]
    public class SaveCustomItem
    {
        [Header("Item Settings")]
        [Tooltip("Identification reference used to access this item.")]
        public string itemTag; // Identification tag for the custom item.

        [Header("Definitions")]
        [Tooltip("List of vector values associated with this tag.")]
        public List<SaveCustomVector> itemVector;

        [Tooltip("List of float values associated with this tag.")]
        public List<SaveCustomFloat> itemFloat;

        [Tooltip("List of int values associated with this tag.")]
        public List<SaveCustomInt> itemInt;

        [Tooltip("List of string values associated with this tag.")]
        public List<SaveCustomString> itemString;

        [Tooltip("List of boolean values associated with this tag.")]
        public List<SaveCustomBool> itemBool;
    }

    #endregion

    #region === Value Containers ===

    /// <summary>
    /// Stores a Vector4 value with an identifying tag.
    /// </summary>
    [Serializable]
    public class SaveCustomVector
    {
        [Tooltip("Identifier name for the vector value.")]
        public string vectorTag;

        [Tooltip("Actual Vector4 value to store.")]
        public Vector4 vectorValue;
    }

    /// <summary>
    /// Stores a float value with an identifying tag.
    /// </summary>
    [Serializable]
    public class SaveCustomFloat
    {
        [Tooltip("Identifier name for the float value.")]
        public string floatTag;

        [Tooltip("Actual float value to store.")]
        public float floatValue;
    }

    /// <summary>
    /// Stores an integer value with an identifying tag.
    /// </summary>
    [Serializable]
    public class SaveCustomInt
    {
        [Tooltip("Identifier name for the int value.")]
        public string intTag;

        [Tooltip("Actual int value to store.")]
        public int intValue;
    }

    /// <summary>
    /// Stores a string value with an identifying tag.
    /// </summary>
    [Serializable]
    public class SaveCustomString
    {
        [Tooltip("Identifier name for the string value.")]
        public string stringTag;

        [Tooltip("Actual string value to store.")]
        public string stringValue;
    }

    /// <summary>
    /// Stores a boolean value with an identifying tag.
    /// </summary>
    [Serializable]
    public class SaveCustomBool
    {
        [Tooltip("Identifier name for the boolean value.")]
        public string boolTag;

        [Tooltip("Actual bool value to store.")]
        public bool boolValue;
    }

    #endregion

#if UNITY_EDITOR

    #region === Editor Utility ===

    /// <summary>
    /// Provides Unity Editor menu functionality for creating the SaveCustomObject asset.
    /// </summary>
    public class KeyboardControlListCreator
    {
        /// <summary>
        /// Creates a new SaveCustomObject asset in the Resources folder.
        /// If the asset already exists, prompts the user whether to replace it.
        /// </summary>
        [MenuItem("Assets/Create/Save Custom Game/Save Custom Object Data", false, 1)]
        public static void CreateCustomObjectData()
        {
            string path = "Assets/Resources"; // Defines the folder where the asset will be stored.
            string assetPath = $"{path}/Save Custom Object Data.asset"; // Full path to the asset.

            // Ensure the Resources folder exists. If not, create it.
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets", "Resources"); // Creates a Resources folder inside Assets.
            }

            // Check if an asset already exists at the specified path.
            if (AssetDatabase.LoadAssetAtPath<SaveCustomObject>(assetPath) != null)
            {
                // Ask the user whether to overwrite the existing asset.
                if (!EditorUtility.DisplayDialog("Replace File", "There is already a 'Save Custom Object Data'. Do you want to replace it?", "Yes", "No"))
                {
                    return; // Abort operation if the user selects 'No'.
                }
            }

            // Create a new instance of SaveCustomObject.
            var asset = ScriptableObject.CreateInstance<SaveCustomObject>();

            // Save the newly created asset to the project.
            AssetDatabase.CreateAsset(asset, assetPath);

            // Mark the asset as modified and save the project.
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Automatically highlight and select the created asset in the Project window.
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }

    #endregion

#endif
}