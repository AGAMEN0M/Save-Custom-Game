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
        public byte[] screenshot;

        [Tooltip("Defines the maximum screenshot resolution to avoid memory overhead.")]
        public int pixelLimit = 500;

        [Tooltip("Stores the formatted game time string.")]
        public string gameTime = "00:00:00";

        [Tooltip("Holds the list of currently loaded or relevant scenes.")]
        public List<string> sceneNames;

        #endregion

        #region === Auto Save Settings ===

        [Header("Auto Save Settings")]
        [Tooltip("Enables or disables the autosave feature.")]
        public bool autosaveEnabled;

        [Tooltip("Triggers save based on custom events.")]
        public bool saveGameByEvent;

        [Tooltip("Triggers save based on timed intervals.")]
        public bool saveGameByTime;

        [Tooltip("Defines how often autosave occurs when enabled.")]
        public float saveInterval = 60f;

        #endregion

        #region === Saving Mode Settings ===

        [Header("Saving Mode Settings")]
        [Tooltip("When enabled, the system saves in local game data storage.")]
        public bool gameData = true;

        [Tooltip("When enabled, the system saves in Application.LocalLow.")]
        public bool localLow;

        [Tooltip("When enabled, the system saves using PlayerPrefs.")]
        public bool playerPrefs;

        #endregion

        #region === Custom Data Lists ===

        [Header("Custom Items Settings")]
        [Tooltip("List of custom items containing multiple custom data types.")]
        public List<SaveCustomItem> saveCustomItems;

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
        public string itemTag;

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
    public class SaveCustomObjectCreator
    {
        /// <summary>
        /// Creates a new SaveCustomObject asset inside a "Save Custom Game/Resources" folder.
        /// Searches the project for a valid base folder and creates missing directories if needed.
        /// Prevents duplicate assets and allows replacement if one already exists.
        /// </summary>
        [MenuItem("Assets/Create/Tools/Save Custom Game/Save Custom Object Data")]
        public static void CreateCustomObjectData()
        {
            // Attempt to locate "Save Custom Game" folder anywhere in the project.
            string[] guids = AssetDatabase.FindAssets("Save Custom Game t:folder");

            string basePath = null;

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Ensure it's exactly the folder we want.
                if (path.EndsWith("Save Custom Game"))
                {
                    basePath = path;
                    break;
                }
            }

            // Fallback: if not found, create default structure.
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = "Assets/Save Custom Game";

                // Create main folder if missing.
                if (!AssetDatabase.IsValidFolder(basePath))
                {
                    AssetDatabase.CreateFolder("Assets", "Save Custom Game");
                }
            }

            // Ensure Resources folder exists inside Save Custom Game.
            string resourcesPath = $"{basePath}/Resources";

            if (!AssetDatabase.IsValidFolder(resourcesPath))
            {
                AssetDatabase.CreateFolder(basePath, "Resources");
            }

            // Define final asset path.
            string assetPath = $"{resourcesPath}/Save Custom Object Data.asset";

            // Check if asset already exists.
            var existing = AssetDatabase.LoadAssetAtPath<SaveCustomObject>(assetPath);

            if (existing != null)
            {
                // Ask user if they want to replace existing asset.
                if (!EditorUtility.DisplayDialog(
                    "Replace File",
                    "A 'Save Custom Object Data' already exists. Do you want to replace it?",
                    "Yes",
                    "No"))
                {
                    // Focus existing asset instead.
                    Selection.activeObject = existing;
                    EditorUtility.FocusProjectWindow();
                    return;
                }

                // Delete existing asset before creating a new one.
                AssetDatabase.DeleteAsset(assetPath);
            }

            // Create new ScriptableObject instance.
            var asset = ScriptableObject.CreateInstance<SaveCustomObject>();

            // Create asset in the resolved path.
            AssetDatabase.CreateAsset(asset, assetPath);

            // Mark as dirty and save changes.
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Focus and select the newly created asset.
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"SaveCustomObject created at: {assetPath}");
        }
    }

    #endregion

#endif
}