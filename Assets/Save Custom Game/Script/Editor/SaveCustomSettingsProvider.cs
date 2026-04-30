/*
 * ---------------------------------------------------------------------------
 * Description: Provides a fully customized Project Settings panel for managing
 *              the SaveCustomObject asset. This system replaces the default
 *              Unity inspector with a structured and optimized interface,
 *              allowing intuitive editing of save settings, storage modes,
 *              autosave behavior, and dynamic custom data collections.
 *              Includes intelligent UI rendering, tab-based data organization,
 *              and persistent editor state handling.
 *
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SaveCustomGame.Editor
{
    #region === Settings Provider ===

    /// <summary>
    /// Custom Unity Project Settings provider responsible for rendering and managing
    /// the SaveCustomObject configuration interface.
    /// This class implements a fully custom inspector-like UI with dynamic lists,
    /// tab-based navigation, contextual controls, and optimized layout handling.
    /// It also ensures proper asset creation, validation, and persistence using
    /// Unity's SerializedObject workflow.
    /// </summary>
    public static class SaveCustomSettingsProvider
    {
        /// <summary>
        /// Cached reference to the loaded SaveCustomObject.
        /// </summary>
        private static SaveCustomObject data;

        /// <summary>
        /// Creates and configures the Unity Project Settings provider responsible for displaying
        /// and editing the SaveCustomObject asset.
        /// This includes UI rendering, asset validation, and full custom inspector logic.
        /// </summary>
        /// <returns>A configured <see cref="SettingsProvider"/> instance.</returns>
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Project/Save Custom Game", SettingsScope.Project)
            {
                label = "Save Custom Game",

                guiHandler = (searchContext) =>
                {
                    // Retrieve the ScriptableObject that stores all save system data.
                    if (data == null) data = SaveDataUtility.GetSaveCustomObject();

                    // Add spacing and show a general description of this settings panel.
                    EditorGUILayout.Space(5);
                    EditorGUILayout.HelpBox("Configure Save Custom Game settings including autosave, storage mode, and custom data.", MessageType.Info);
                    EditorGUILayout.Space(5);

                    #region === Missing Asset ===

                    // If the asset does not exist, show warning and provide creation button.
                    if (data == null)
                    {
                        EditorGUILayout.HelpBox("SaveCustomObject asset not found.", MessageType.Warning);

                        // Button to create or locate the required asset.
                        if (GUILayout.Button(new GUIContent("Create Settings Asset", "Creates or locates the SaveCustomObject asset."), GUILayout.Height(30)))
                        {
                            SaveCustomObjectCreator.CreateCustomObjectData(); // Create the asset using the utility method.
                            data = SaveDataUtility.GetSaveCustomObject(); // Try retrieving it again after creation.

                            if (data != null)
                            {
                                // Highlight and select the newly created asset.
                                EditorGUIUtility.PingObject(data);
                                Selection.activeObject = data;
                            }

                            GUIUtility.ExitGUI(); // Exit GUI to avoid layout errors after asset creation.
                        }

                        return;
                    }

                    #endregion

                    #region === Asset Reference ===

                    // Display reference to the ScriptableObject.
                    EditorGUILayout.LabelField("Data Asset", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();

                    // Show asset field as read-only.
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(data, typeof(SaveCustomObject), false);
                    EditorGUI.EndDisabledGroup();

                    // Ping button highlights the asset in Project window.
                    if (GUILayout.Button(new GUIContent("Ping", "Highlight asset in Project window."), GUILayout.MaxWidth(50)))
                    {
                        EditorGUIUtility.PingObject(data);
                        Selection.activeObject = data;
                    }

                    // Open button shows the asset in Inspector.
                    if (GUILayout.Button(new GUIContent("Open", "Open asset in Inspector."), GUILayout.MaxWidth(50)))
                    {
                        EditorUtility.OpenPropertyEditor(data);
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(10);

                    #endregion

                    #region === Inspector ===

                    // Create serialized wrapper to safely edit properties with undo support.
                    SerializedObject so = new(data);
                    so.Update();

                    // Cache frequently used properties.
                    var pixelLimit = so.FindProperty("pixelLimit");
                    var saveGameByEvent = so.FindProperty("saveGameByEvent");
                    var saveGameByTime = so.FindProperty("saveGameByTime");
                    var saveInterval = so.FindProperty("saveInterval");
                    var gameData = so.FindProperty("gameData");
                    var localLow = so.FindProperty("localLow");
                    var playerPrefs = so.FindProperty("playerPrefs");
                    var saveCustomItems = so.FindProperty("saveCustomItems");

                    #region === Screenshot Settings ===

                    // Control maximum resolution to prevent excessive memory usage.
                    EditorGUILayout.LabelField("Screenshot Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(pixelLimit, new GUIContent("Screenshot Pixel Limit"));
                    EditorGUILayout.Space(10);

                    #endregion

                    #region === Auto Save ===

                    EditorGUILayout.LabelField("Auto Save Settings", EditorStyles.boldLabel);

                    // Toggle autosave triggers.
                    EditorGUILayout.PropertyField(saveGameByEvent);
                    EditorGUILayout.PropertyField(saveGameByTime);

                    EditorGUI.indentLevel++;

                    // Only allow editing interval if time-based saving is enabled.
                    using (new EditorGUI.DisabledScope(!saveGameByTime.boolValue))
                    {
                        EditorGUILayout.PropertyField(saveInterval);
                    }

                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space(10);

                    #endregion

                    #region === Storage Mode ===

                    EditorGUILayout.LabelField("Storage Mode Settings", EditorStyles.boldLabel);

                    // Define toolbar options with tooltips.
                    GUIContent[] options =
                    {
                        new("Game Data", "Save files inside the game directory (useful for builds or portable setups)."),
                        new("LocalLow", "Save files in Application.persistentDataPath (recommended for most cases)."),
                        new("PlayerPrefs", "Save using Unity PlayerPrefs system (limited and not suited for large data).")
                    };

                    // Ensure at least one option is always selected.
                    if (!gameData.boolValue && !localLow.boolValue && !playerPrefs.boolValue) gameData.boolValue = true;

                    // Determine currently selected option.
                    int selectedIndex = gameData.boolValue ? 0 : localLow.boolValue ? 1 : playerPrefs.boolValue ? 2 : 0;

                    // Draw toolbar.
                    int newIndex = GUILayout.Toolbar(selectedIndex, options);

                    // Apply selection changes.
                    if (newIndex != selectedIndex)
                    {
                        gameData.boolValue = newIndex == 0;
                        localLow.boolValue = newIndex == 1;
                        playerPrefs.boolValue = newIndex == 2;
                    }

                    // Contextual description based on selected option.
                    string text = "";
                    switch (newIndex)
                    {
                        case 0:
                            text = "Data will be saved inside the game folder.";
                            break;

                        case 1:
                            text = "Data will be saved in Application.persistentDataPath.";
                            break;

                        case 2:
                            text = "Data will be stored using Unity PlayerPrefs.";
                            break;
                    }

                    EditorGUILayout.HelpBox(text, MessageType.Info);

                    EditorGUILayout.Space(10);

                    #endregion

                    #region === Custom Data ===

                    EditorGUILayout.LabelField("Custom Save Data Structure", EditorStyles.boldLabel);

                    EditorGUILayout.Space(5);

                    // Header with add/remove controls for main item list.
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Items", EditorStyles.miniBoldLabel);

                    if (GUILayout.Button(new GUIContent("+", "Add a new element."), GUILayout.Width(25)))
                    {
                        saveCustomItems.arraySize++;
                    }

                    if (GUILayout.Button(new GUIContent("-", "Remove the last element."), GUILayout.Width(25)) && saveCustomItems.arraySize > 0)
                    {
                        saveCustomItems.arraySize--;
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);

                    // Iterate through all custom items.
                    for (int i = 0; i < saveCustomItems.arraySize; i++)
                    {
                        var item = saveCustomItems.GetArrayElementAtIndex(i);

                        var itemTag = item.FindPropertyRelative("itemTag");
                        var itemVector = item.FindPropertyRelative("itemVector");
                        var itemFloat = item.FindPropertyRelative("itemFloat");
                        var itemInt = item.FindPropertyRelative("itemInt");
                        var itemString = item.FindPropertyRelative("itemString");
                        var itemBool = item.FindPropertyRelative("itemBool");

                        EditorGUILayout.BeginVertical("box");

                        #region === Header ===

                        // Header with tag and remove button.
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(itemTag, GUIContent.none);

                        if (GUILayout.Button(new GUIContent("X", "Remove this element."), GUILayout.Width(25)))
                        {
                            saveCustomItems.DeleteArrayElementAtIndex(i);
                            break;
                        }

                        EditorGUILayout.EndHorizontal();

                        #endregion

                        EditorGUILayout.Space(5);

                        #region === Tabs ===

                        // Detect which lists contain data.
                        bool hasVector = itemVector.arraySize > 0;
                        bool hasFloat = itemFloat.arraySize > 0;
                        bool hasInt = itemInt.arraySize > 0;
                        bool hasString = itemString.arraySize > 0;
                        bool hasBool = itemBool.arraySize > 0;

                        // Build tab UI dynamically.
                        GUIContent[] tabs =
                        {
                            new(hasVector ? $"Vector ({itemVector.arraySize})" : "Vector",
                            "Stores Vector4 values (X, Y, Z, W).\nUseful for positions, rotations, scales, or custom packed data."),
                            new(hasFloat ? $"Float ({itemFloat.arraySize})" : "Float",
                            "Stores floating point values.\nIdeal for health, speed, timers, percentages, etc."),
                            new(hasInt ? $"Int ({itemInt.arraySize})" : "Int",
                            "Stores integer values.\nBest for counters, levels, IDs, or discrete states."),
                            new(hasString ? $"String ({itemString.arraySize})" : "String",
                            "Stores text values.\nUseful for names, identifiers, JSON data, or serialized references."),
                            new(hasBool ? $"Bool ({itemBool.arraySize})" : "Bool",
                            "Stores boolean values (true/false).\nGood for flags, toggles, and state conditions.")
                        };

                        // Persist tab state per item.
                        int tabIndex = EditorPrefs.GetInt(item.propertyPath, 0);
                        int newTab = GUILayout.Toolbar(tabIndex, tabs);

                        if (newTab != tabIndex) EditorPrefs.SetInt(item.propertyPath, newTab);

                        EditorGUILayout.Space(5);

                        // Resolve active list.
                        SerializedProperty currentList = newTab switch
                        {
                            0 => itemVector,
                            1 => itemFloat,
                            2 => itemInt,
                            3 => itemString,
                            4 => itemBool,
                            _ => itemVector
                        };

                        #endregion

                        #region === List Drawer ===

                        // Draw list controls.
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Entries", EditorStyles.miniBoldLabel);

                        if (GUILayout.Button(new GUIContent("+", "Add a new element."), GUILayout.Width(25)))
                        {
                            currentList.arraySize++;
                        }

                        if (GUILayout.Button(new GUIContent("-", "Remove the last element."), GUILayout.Width(25)) && currentList.arraySize > 0)
                        {
                            currentList.arraySize--;
                        }

                        EditorGUILayout.EndHorizontal();

                        // Draw elements.
                        for (int j = 0; j < currentList.arraySize; j++)
                        {
                            var element = currentList.GetArrayElementAtIndex(j);

                            var tagProp = element.FindPropertyRelative(element.type.Contains("Vector") ? "vectorTag" :
                                element.type.Contains("Float") ? "floatTag" :
                                element.type.Contains("Int") ? "intTag" :
                                element.type.Contains("String") ? "stringTag" :
                                "boolTag");

                            var valueProp = element.FindPropertyRelative(element.type.Contains("Vector") ? "vectorValue" :
                                element.type.Contains("Float") ? "floatValue" :
                                element.type.Contains("Int") ? "intValue" :
                                element.type.Contains("String") ? "stringValue" :
                                "boolValue");

                            EditorGUILayout.BeginHorizontal();

                            // Tag field (fixed width for alignment).
                            EditorGUILayout.PropertyField(tagProp, GUIContent.none, GUILayout.MaxWidth(150));

                            // Custom handling for Vector4 layout.
                            if (valueProp.propertyType == SerializedPropertyType.Vector4)
                            {
                                var rect = GUILayoutUtility.GetRect(200, EditorGUIUtility.singleLineHeight);
                                valueProp.vector4Value = EditorGUI.Vector4Field(rect, GUIContent.none, valueProp.vector4Value);
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(valueProp, GUIContent.none);
                            }

                            // Remove button.
                            if (GUILayout.Button(new GUIContent("X", "Remove this element."), GUILayout.Width(20)))
                            {
                                currentList.DeleteArrayElementAtIndex(j);
                                break;
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        #endregion

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(5);
                    }

                    EditorGUILayout.Space(10);

                    #endregion

                    // Apply changes and register undo properly.
                    if (so.ApplyModifiedProperties())
                    {
                        Undo.RecordObject(data, "Modify Save Custom Object");
                        EditorUtility.SetDirty(data);
                        AssetDatabase.SaveAssets();
                    }

                    #endregion
                },

                keywords = new HashSet<string>
                {
                    "Save",
                    "Save System",
                    "Autosave",
                    "Persistence",
                    "Save Custom Game"
                }
            };
        }
    }

    #endregion
}