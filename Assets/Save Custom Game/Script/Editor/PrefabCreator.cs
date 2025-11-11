/*
 * ---------------------------------------------------------------------------
 * Description: Utility class for creating and instantiating UI prefabs related to the 
 *              Save Custom Game system. Provides menu options for adding UI save/load 
 *              menus directly into the current scene.
 *
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

using Object = UnityEngine.Object;

namespace SaveCustomGame
{
    /// <summary>
    /// Provides methods and menu options for instantiating Save Custom UI prefabs into the scene.
    /// </summary>
    public static class PrefabCreator
    {
        #region === Canvas And Prefab Creation ===

        /// <summary>
        /// Creates a new UI Canvas with EventSystem if none exists in the scene.
        /// </summary>
        private static Canvas CreateUICanvas()
        {
            // Create the Canvas root object.
            GameObject canvasGO = new("Canvas");

            // Add UI components required for rendering UI elements.
            var canvas = canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Configure the Canvas for overlay rendering; ensures UI always appears on screen.
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            canvas.sortingOrder = 0;
            canvas.targetDisplay = 0;

            // Create EventSystem if one is not present in the scene to handle UI interactions.
            GameObject eventSystemGO = new("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();

            // Register objects for Undo support inside Unity Editor.
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
            Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create EventSystem");

            // Return the created Canvas object.
            return canvas;
        }

        /// <summary>
        /// Searches the project's AssetDatabase for a prefab by name inside the Save Custom Game folder.
        /// </summary>
        private static GameObject FindPrefabByName(string prefabName)
        {
            // Retrieve all asset GUIDs that match the prefab type and name.
            var guids = AssetDatabase.FindAssets($"{prefabName} t:Prefab");

            foreach (var guid in guids)
            {
                // Convert GUID to actual path in the project.
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // Check if prefab matches our criteria and is located in the expected folder.
                if (path.Contains("Save Custom Game/Prefab") && Path.GetFileNameWithoutExtension(path).Equals(prefabName, StringComparison.OrdinalIgnoreCase))
                {
                    // Load and return the prefab located at the path.
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }

            // Log an error if no prefab with the given name could be found.
            Debug.LogError($"Prefab with the name '{prefabName}' not found.");
            return null;
        }

        /// <summary>
        /// Instantiates a prefab and attaches it to either the selected GameObject or an existing/created Canvas.
        /// </summary>
        /// <param name="fileName">The name of the prefab to instantiate.</param>
        /// <param name="selectedGameObject">Optional parent GameObject. If null, attaches to Canvas.</param>
        private static void CreateAndConfigurePrefab(string fileName, GameObject selectedGameObject)
        {
            // Attempt to locate an existing Canvas; if not found, create a new one.
            #pragma warning disable IDE0079
            #pragma warning disable UNT0007
            var canvas = Object.FindAnyObjectByType<Canvas>() ?? CreateUICanvas();
            #pragma warning restore UNT0007
            #pragma warning restore IDE0079

            // Attempt to load the specified prefab.
            var prefab = FindPrefabByName(fileName);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found: {fileName}.prefab. Ensure it exists in the project.");
                return;
            }

            // Determine the parent Transform where the new instance will be placed.
            var parent = selectedGameObject != null ? selectedGameObject.transform : canvas.transform;

            // Instantiate the prefab as a child of the chosen parent.
            var instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;

            // Perform final setup steps on the newly created prefab instance.
            FinalizePrefabSetup(fileName, instance);
        }

        /// <summary>
        /// Finalizes prefab creation by enabling Undo, unpacking into editable GameObject, selecting, and auto-renaming.
        /// </summary>
        /// <param name="fileName">Prefab base name.</param>
        /// <param name="newGameObject">Instantiated prefab instance.</param>
        private static void FinalizePrefabSetup(string fileName, GameObject newGameObject)
        {
            if (newGameObject == null) return;

            // Register the object for Undo handling.
            Undo.RegisterCreatedObjectUndo(newGameObject, $"Create {fileName}");

            // Convert prefab instance into a fully editable GameObject hierarchy.
            PrefabUtility.UnpackPrefabInstance(newGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            // Select the created object in the hierarchy.
            Selection.activeGameObject = newGameObject;

            // Trigger rename mode automatically to allow immediate renaming by user.
            EditorApplication.delayCall += () =>
            {
                if (Selection.activeGameObject == newGameObject)
                {
                    EditorWindow.focusedWindow.SendEvent(new()
                    {
                        keyCode = KeyCode.F2,
                        type = EventType.KeyDown
                    });
                }
            };
        }

        #endregion

        #region === Legacy UI Prefabs ===

        /// <summary>Creates the Legacy Load Menu UI prefab.</summary>
        [MenuItem("GameObject/UI/Save Custom Game/Legacy/Load Menu", false, 1)]
        public static void CreateLoadMenuPrefab() => CreateAndConfigurePrefab("Load Menu (Legacy)", Selection.activeGameObject);

        /// <summary>Creates the Legacy Save Menu UI prefab.</summary>
        [MenuItem("GameObject/UI/Save Custom Game/Legacy/Save Menu", false, 2)]
        public static void CreateSaveMenuPrefab() => CreateAndConfigurePrefab("Save Menu (Legacy)", Selection.activeGameObject);

        #endregion

        #region === TMP UI Prefabs ===

        /// <summary>Creates the TMP Load Menu UI prefab.</summary>
        [MenuItem("GameObject/UI/Save Custom Game/Load Menu (TMP)", false, 1)]
        public static void CreateLoadMenuPrefabTMP() => CreateAndConfigurePrefab("Load Menu (TMP)", Selection.activeGameObject);

        /// <summary>Creates the TMP Save Menu UI prefab.</summary>
        [MenuItem("GameObject/UI/Save Custom Game/Save Menu (TMP)", false, 2)]
        public static void CreateSaveMenuPrefabTMP() => CreateAndConfigurePrefab("Save Menu (TMP)", Selection.activeGameObject);

        #endregion
    }
}