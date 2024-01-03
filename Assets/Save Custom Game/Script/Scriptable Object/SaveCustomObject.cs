using System.Collections.Generic;
using UnityEngine;

// This section checks if the code is being executed in Unity Editor to prevent compilation errors in a build.
#if UNITY_EDITOR
using UnityEditor;

// This class creates a custom asset in the Unity Editor.
public class CustomObjectDataCreator
{
    // Menu item for creating the custom object data asset.
    [MenuItem("Assets/Create/Save Custom Game/Save Custom Object Data")]
    public static void CreateCustomObjectData()
    {
        string path = "Assets/Resources";
        string assetPath = path + "/Save Custom Object Data.asset";

        // Create a Resources folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(path)) { AssetDatabase.CreateFolder("Assets", "Resources"); }

        // Check if the asset already exists, and prompt the user to replace it.
        if (AssetDatabase.LoadAssetAtPath<SaveCustomObject>(assetPath) != null)
        {
            if (!EditorUtility.DisplayDialog("Replace File", "There is already a 'Save Custom Object Data'. Do you want to replace it?", "Yes", "No"))
            {
                return;
            }
        }

        // Create an instance of SaveCustomObject and save it as an asset.
        SaveCustomObject asset = ScriptableObject.CreateInstance<SaveCustomObject>();
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

// This class represents the custom object data and inherits from ScriptableObject.
public class SaveCustomObject : ScriptableObject
{
    [Header("Settings")]
    public byte[] screenshot; // Stores a screenshot as a byte array.
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

// Serializable class representing a custom item with different data types.
[System.Serializable]
public class SaveCustomItem
{
    [Header("Item Settings")]
    public string itemTag; // Identification tag for the custom item.
    [Space(10)]
    [Header("Definitions")]
    // Definitions for various data types.
    public List<SaveCustomFloat> itemFloat;
    public List<SaveCustomInt> itemInt;
    public List<SaveCustomString> itemString;
    public List<SaveCustomBool> itemBool;
}

// Serializable class for saving custom float data.
[System.Serializable]
public class SaveCustomFloat
{
    public string floatTag; // Identification tag for the float value.
    public float floatValue; // Actual float value to be saved.
}

// Serializable class for saving custom int data.
[System.Serializable]
public class SaveCustomInt
{
    public string intTag; // Identification tag for the int value.
    public float intValue; // Actual int value to be saved.
}

// Serializable class for saving custom string data.
[System.Serializable]
public class SaveCustomString
{
    public string stringTag; // Identification tag for the string value.
    public string stringValue; // Actual string value to be saved.
}

// Serializable class for saving custom bool data.
[System.Serializable]
public class SaveCustomBool
{
    public string boolTag; // Identification tag for the bool value.
    public bool boolValue; // Actual bool value to be saved.
}