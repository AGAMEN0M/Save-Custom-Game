/*
 * ---------------------------------------------------------------------------
 * Description: Provides a Unity Editor menu option for quickly opening the SaveCustomData
 *              ScriptableObject instance inside the Unity Inspector. This streamlines access
 *              to the custom save configuration used in the project.
 *
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEditor;
using UnityEngine;
using System.Linq;

namespace SaveCustomGame
{
    /// <summary>
    /// Provides a menu option to quickly open the SaveCustomData asset in the Unity Editor.
    /// </summary>
    public class OpenSaveCustomData
    {
        #region === Menu Method ===

        /// <summary>
        /// Opens the SaveCustomData ScriptableObject in the Unity property editor or focuses
        /// its existing window if it is already open.
        /// </summary>
        [MenuItem("Window/Save Custom Game/Save Custom Settings", false, 2032)]
        public static void OpenSettingsData()
        {
            // Retrieve the ScriptableObject instance responsible for storing save configuration.
            var settingsData = SaveDataUtility.GetSaveCustomObject();

            // If the asset could not be found, notify the developer and exit.
            if (settingsData == null)
            {
                Debug.LogError("Failed to load SaveCustomData. Ensure the ScriptableObject exists.");
                return;
            }

            // Search all opened editor windows to determine whether a window displaying
            // the same asset is already open. This avoids opening duplicate editors.
            var existingWindow = Resources.FindObjectsOfTypeAll<EditorWindow>().FirstOrDefault(window => window.titleContent.text == settingsData.name);

            // If an existing window is found, bring it to focus.
            if (existingWindow != null)
            {
                existingWindow.Focus();
            }
            else
            {
                // Otherwise, open the ScriptableObject in Unity's built-in Property Editor window.
                EditorUtility.OpenPropertyEditor(settingsData);
            }
        }

        #endregion
    }
}