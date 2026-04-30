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

namespace SaveCustomGame.Editor
{
    /// <summary>
    /// Provides methods and menu options for instantiating Save Custom UI prefabs into the scene.
    /// </summary>
    public static class PrefabCreator
    {
        #region === Canvas And Prefab Creation ===

        /// <summary>
        /// Creates a new UI Canvas configured for Screen Space Overlay.
        /// Also ensures that an EventSystem exists in the scene.
        /// </summary>
        /// <returns>Returns the created Canvas component.</returns>
        private static Canvas CreateUICanvas()
        {
            // Create the Canvas root object.
            GameObject canvasGO = new("Canvas");

            // Add required UI components.
            var canvas = canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Configure Canvas for overlay rendering.
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            canvas.sortingOrder = 0;
            canvas.targetDisplay = 0;

            // Ensure an EventSystem exists in the scene (avoid duplicates).
            var existingEventSystem = Object.FindAnyObjectByType<EventSystem>();

            if (existingEventSystem == null)
            {
                // Create EventSystem only if none exists.
                GameObject eventSystemGO = new("EventSystem");

                eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();

                // Register for Undo support.
                Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create EventSystem");
            }

            // Register Canvas for Undo support.
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");

            return canvas;
        }

        /// <summary>
        /// Searches the Unity AssetDatabase for a prefab with the specified name
        /// inside the "Save Custom Game/Prefab" folder.
        /// </summary>
        /// <param name="prefabName">The exact name of the prefab file (without extension).</param>
        /// <returns>Returns the matching prefab GameObject if found; otherwise null.</returns>
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
            var canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null) canvas = CreateUICanvas();

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

        /// <summary>
        /// Creates the Legacy Load Menu UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Save Custom Game/UI/Legacy/Load Menu")]
        public static void CreateLoadMenuPrefab() => CreateAndConfigurePrefab("Load Menu (Legacy)", Selection.activeGameObject);

        /// <summary>
        /// Creates the Legacy Save Menu UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Save Custom Game/UI/Legacy/Save Menu")]
        public static void CreateSaveMenuPrefab() => CreateAndConfigurePrefab("Save Menu (Legacy)", Selection.activeGameObject);

        #endregion

        #region === TMP UI Prefabs ===

        /// <summary>
        /// Creates the TMP Load Menu UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Save Custom Game/UI/Load Menu (TMP)")]
        public static void CreateLoadMenuPrefabTMP() => CreateAndConfigurePrefab("Load Menu (TMP)", Selection.activeGameObject);

        /// <summary>
        /// Creates the TMP Save Menu UI prefab.
        /// </summary>
        [MenuItem("GameObject/Tools/Save Custom Game/UI/Save Menu (TMP)")]
        public static void CreateSaveMenuPrefabTMP() => CreateAndConfigurePrefab("Save Menu (TMP)", Selection.activeGameObject);

        #endregion
    }
}