/*
 * ---------------------------------------------------------------------------
 * Description: Central utility class for managing data stored in SaveCustomObject.
 *              Provides unified access for reading and modifying float, int, string,
 *              bool, and vector values; capturing and restoring screenshots; converting
 *              textures to sprites; locating scene save components; controlling
 *              auto-save behavior; and triggering save events. Ensures safe access,
 *              automatic container creation when needed, and consistent logging for
 *              debugging across the save system.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Collections.Generic;
using UnityEngine;
using System;

using static SaveCustomGame.ExceptionUtility;
using Object = UnityEngine.Object;

namespace SaveCustomGame
{
    public static class SaveDataUtility
    {
        #region === Get Save Object ===

        private static SaveCustomObject saveCustomObject; // Internal static reference to the loaded SaveCustomObject instance.

        /// <summary>
        /// Retrieves the SaveCustomObject instance used to store all custom saved data.
        /// Automatically loads it from the Resources folder if it has not been loaded yet.
        /// Logs an error if the asset cannot be found, ensuring visibility of missing data issues.
        /// </summary>
        /// <returns>
        /// The loaded SaveCustomObject instance, or null if the resource cannot be located.
        /// </returns>
        public static SaveCustomObject GetSaveCustomObject()
        {
            if (saveCustomObject == null)
            {
                saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data");
                if (saveCustomObject == null)
                {
                    Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null.");
                    return null;
                }
            }

            return saveCustomObject;
        }

        #endregion

        #region === Get Value Methods ===

        /// <summary>
        /// Retrieves a stored Vector4 value associated with the specified itemTag and vectorTag.
        /// Returns Vector4.zero if the value is not found.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="vectorTag">Key identifying the specific Vector4 entry.</param>
        /// <returns>The stored Vector4 value, or a default value if none exists.</returns>
        public static Vector4 GetVector(string itemTag, string vectorTag)
        {
            return FindValue<SaveCustomVector, Vector4>(
                itemTag, vectorTag,
                item => item.itemVector, // List selector for Vector4 entries.
                entry => entry.vectorTag, // Tag selector for finding the specific entry.
                entry => entry.vectorValue // Value retrieval.
            );
        }

        /// <summary>
        /// Retrieves a stored float value associated with the specified itemTag and floatTag.
        /// Returns 0 if the value is not found.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="floatTag">Key identifying the specific float entry.</param>
        /// <returns>The stored float value, or a default value if none exists.</returns>
        public static float GetFloat(string itemTag, string floatTag)
        {
            return FindValue<SaveCustomFloat, float>(
                itemTag, floatTag,
                item => item.itemFloat, // List selector for float entries.
                entry => entry.floatTag, // Tag selector.
                entry => entry.floatValue // Value retrieval.
            );
        }

        /// <summary>
        /// Retrieves a stored int value associated with the specified itemTag and intTag.
        /// Returns 0 if the value is not found.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="intTag">Key identifying the specific int entry.</param>
        /// <returns>The stored int value, or a default value if none exists.</returns>
        public static int GetInt(string itemTag, string intTag)
        {
            return FindValue<SaveCustomInt, int>(
                itemTag, intTag,
                item => item.itemInt, // List selector for int entries.
                entry => entry.intTag, // Tag selector.
                entry => entry.intValue // Value retrieval.
            );
        }

        /// <summary>
        /// Retrieves a stored string value associated with the specified itemTag and stringTag.
        /// Returns an empty string if the value is not found.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="stringTag">Key identifying the specific string entry.</param>
        /// <returns>The stored string value, or a default value if none exists.</returns>
        public static string GetString(string itemTag, string stringTag)
        {
            return FindValue<SaveCustomString, string>(
                itemTag, stringTag,
                item => item.itemString, // List selector for string entries.
                entry => entry.stringTag, // Tag selector.
                entry => entry.stringValue // Value retrieval.
            );
        }

        /// <summary>
        /// Retrieves a stored bool value associated with the specified itemTag and boolTag.
        /// Returns false if the value is not found.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="boolTag">Key identifying the specific bool entry.</param>
        /// <returns>The stored bool value, or a default value if none exists.</returns>
        public static bool GetBool(string itemTag, string boolTag)
        {
            return FindValue<SaveCustomBool, bool>(
                itemTag, boolTag,
                item => item.itemBool, // List selector for bool entries.
                entry => entry.boolTag, // Tag selector.
                entry => entry.boolValue // Value retrieval.
            );
        }

        #endregion

        #region === Set Value Methods ===

        /// <summary>
        /// Sets a vector value (Vector2, Vector3, Vector4 or Quaternion) associated with the given tags.
        /// Automatically converts the input value to a Vector4 before saving.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="vectorTag">Key identifying the specific vector entry.</param>
        /// <param name="newValue">The new vector-compatible value to store.</param>
        public static void SetVector(string itemTag, string vectorTag, object newValue)
        {
            Vector4 value;
            bool supported = true;

            // Convert the provided object to a Vector4 based on supported input types.
            switch (newValue)
            {
                // Convert Vector2 to Vector4.
                case Vector2 v2:
                    value = new(v2.x, v2.y, 0f, 0f);
                    break;

                // Convert Vector3 to Vector4.
                case Vector3 v3:
                    value = new(v3.x, v3.y, v3.z, 0f);
                    break;

                // Already Vector4.
                case Vector4 v4:
                    value = v4;
                    break;

                // Convert Quaternion to Vector4.
                case Quaternion q:
                    value = new(q.x, q.y, q.z, q.w);
                    break;

                // Unsupported type.
                default:
                    supported = false;
                    value = Vector4.zero;
                    break;
            };

            // Log error only if type unsupported.
            if (!supported)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Unsupported value type for SetVector. Received: {newValue.GetType()}");
                return;
            }

            // Forward processed Vector4 to final saving method.
            SetVector4(itemTag, vectorTag, value);
        }

        /// <summary>
        /// Sets a Vector4 value associated with the specified itemTag and vectorTag.
        /// Creates a new entry if no matching entry exists.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="vectorTag">Key identifying the specific Vector4 entry.</param>
        /// <param name="newValue">The new Vector4 value to store.</param>
        public static void SetVector4(string itemTag, string vectorTag, Vector4 newValue)
        {
            SetValue<SaveCustomVector, Vector4>(
                itemTag, vectorTag, newValue,
                item => item.itemVector, // Get list of vector entries.
                (item, list) => item.itemVector = list, // Assign list if it was newly created.
                entry => entry.vectorTag, // Get entry key.
                (entry, val) => entry.vectorValue = val, // Assign the new value.
                (tag, val) => new SaveCustomVector { vectorTag = tag, vectorValue = val } // Create entry.
            );
        }

        /// <summary>
        /// Sets a float value associated with the specified itemTag and floatTag.
        /// Creates a new entry if no matching entry exists.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="floatTag">Key identifying the specific float entry.</param>
        /// <param name="newValue">The new float value to store.</param>
        public static void SetFloat(string itemTag, string floatTag, float newValue)
        {
            SetValue<SaveCustomFloat, float>(
                itemTag, floatTag, newValue,
                item => item.itemFloat,
                (item, list) => item.itemFloat = list,
                entry => entry.floatTag,
                (entry, val) => entry.floatValue = val,
                (tag, val) => new SaveCustomFloat { floatTag = tag, floatValue = val }
            );
        }

        /// <summary>
        /// Sets an int value associated with the specified itemTag and intTag.
        /// Creates a new entry if no matching entry exists.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="intTag">Key identifying the specific int entry.</param>
        /// <param name="newValue">The new int value to store.</param>
        public static void SetInt(string itemTag, string intTag, int newValue)
        {
            SetValue<SaveCustomInt, int>(
                itemTag, intTag, newValue,
                item => item.itemInt,
                (item, list) => item.itemInt = list,
                entry => entry.intTag,
                (entry, val) => entry.intValue = val,
                (tag, val) => new SaveCustomInt { intTag = tag, intValue = val }
            );
        }

        /// <summary>
        /// Sets a string value associated with the specified itemTag and stringTag.
        /// Creates a new entry if no matching entry exists.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="stringTag">Key identifying the specific string entry.</param>
        /// <param name="newValue">The new string value to store.</param>
        public static void SetString(string itemTag, string stringTag, string newValue)
        {
            SetValue<SaveCustomString, string>(
                itemTag, stringTag, newValue,
                item => item.itemString,
                (item, list) => item.itemString = list,
                entry => entry.stringTag,
                (entry, val) => entry.stringValue = val,
                (tag, val) => new SaveCustomString { stringTag = tag, stringValue = val }
            );
        }

        /// <summary>
        /// Sets a bool value associated with the specified itemTag and boolTag.
        /// Creates a new entry if no matching entry exists.
        /// </summary>
        /// <param name="itemTag">Identifier grouping related values.</param>
        /// <param name="boolTag">Key identifying the specific bool entry.</param>
        /// <param name="newValue">The new bool value to store.</param>
        public static void SetBool(string itemTag, string boolTag, bool newValue)
        {
            SetValue<SaveCustomBool, bool>(
                itemTag, boolTag, newValue,
                item => item.itemBool,
                (item, list) => item.itemBool = list,
                entry => entry.boolTag,
                (entry, val) => entry.boolValue = val,
                (tag, val) => new SaveCustomBool { boolTag = tag, boolValue = val }
            );
        }

        #endregion

        #region === Internal Helper Methods ===

        /// <summary>
        /// Finds and returns a SaveCustomItem that matches the provided itemTag.
        /// Throws an informative exception if the save object is missing or if
        /// the specified item does not exist. Ensures reliable data access.
        /// </summary>
        /// <param name="itemTag">Identifier for the stored group of values.</param>
        private static SaveCustomItem FindItem(string itemTag)
        {
            // Retrieve reference to SaveCustomObject.
            var saveObject = GetSaveCustomObject();
            if (saveObject == null) throw new KeyNotFoundException($"{GetCallingMethodInfo()} - SaveCustomObject is null.");

            // Iterate through all items to find a matching tag.
            foreach (var item in saveObject.saveCustomItems)
            {
                // Check if the tag matches.
                if (item.itemTag == itemTag) return item;
            }

            // If execution reaches here, no matching item was found.
            throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Item '{itemTag}' not found.");
        }

        /// <summary>
        /// Retrieves a stored value that matches the provided itemTag and dataTag.
        /// Used internally by the public GetValue methods to support multiple data types.
        /// Throws clear exceptions when the item or its data entry cannot be found.
        /// </summary>
        /// <typeparam name="TList">The container type used to store the data (e.g., SaveCustomFloat).</typeparam>
        /// <typeparam name="T">The actual value type being retrieved (e.g., float).</typeparam>
        /// <param name="itemTag">Unique tag representing the saved item group.</param>
        /// <param name="dataTag">Unique tag representing the specific value within the item.</param>
        /// <param name="selector">Delegate that selects the correct list from the item.</param>
        /// <param name="comparer">Delegate that retrieves the tag of each stored entry for comparison.</param>
        /// <param name="valueSelector">Delegate that retrieves the actual stored value.</param>
        private static T FindValue<TList, T>(
            string itemTag,
            string dataTag,
            Func<SaveCustomItem, List<TList>> selector,
            Func<TList, string> comparer,
            Func<TList, T> valueSelector)
        {
            // Find the correct item based on the provided tag.
            var item = FindItem(itemTag);

            // Attempt to get the correct list of stored entries.
            var list = selector(item) ?? throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Data list for '{itemTag}' is null.");

            // Search for the entry with the matching dataTag.
            foreach (var entry in list)
            {
                if (comparer(entry) == dataTag)
                {
                    // Return the found value.
                    return valueSelector(entry);
                }
            }

            // If no matching data key is found, throw an informative exception.
            throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Tag '{dataTag}' not found in item '{itemTag}'.");
        }

        /// <summary>
        /// Inserts or updates a stored value identified by itemTag and dataTag.
        /// Automatically creates items or entries if they do not yet exist.
        /// Ensures the SaveCustomObject structure remains valid and synchronized.
        /// </summary>
        /// <typeparam name="TList">The list entry type (e.g., SaveCustomInt).</typeparam>
        /// <typeparam name="TValue">The value type to store (e.g., int).</typeparam>
        /// <param name="itemTag">Unique tag identifying the group of stored values.</param>
        /// <param name="dataTag">Unique tag identifying the specific value within the group.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <param name="getList">Delegate to get the corresponding list from the item.</param>
        /// <param name="setList">Delegate to assign a new list back to the item when required.</param>
        /// <param name="getTag">Delegate to retrieve the tag of each entry for comparison.</param>
        /// <param name="setValue">Delegate to assign the stored value to an existing entry.</param>
        /// <param name="createEntry">Delegate that creates a new entry if one does not yet exist.</param>
        private static void SetValue<TList, TValue>(
            string itemTag,
            string dataTag,
            TValue newValue,
            Func<SaveCustomItem, List<TList>> getList,
            Action<SaveCustomItem, List<TList>> setList,
            Func<TList, string> getTag,
            Action<TList, TValue> setValue,
            Func<string, TValue, TList> createEntry)
        {
            // Retrieve reference to SaveCustomObject.
            var saveObject = GetSaveCustomObject();
            if (saveObject == null) return;

            // Ensure saveCustomItems list exists.
            saveObject.saveCustomItems ??= new List<SaveCustomItem>();

            // Search for an existing item that matches itemTag.
            foreach (var item in saveObject.saveCustomItems)
            {
                if (item.itemTag == itemTag)
                {
                    // Attempt to retrieve the entry list for this item.
                    var list = getList(item);
                    if (list == null)
                    {
                        list = new List<TList>();
                        setList(item, list); // Assign the newly created list.
                    }

                    // Search for an existing entry that matches dataTag.
                    foreach (var entry in list)
                    {
                        if (getTag(entry) == dataTag)
                        {
                            // Update existing value and exit.
                            setValue(entry, newValue);
                            return;
                        }
                    }

                    // If the tag was not found, append a new entry.
                    list.Add(createEntry(dataTag, newValue));
                    return;
                }
            }

            // If the item did not exist, create it and assign a new populated list.
            var newItem = new SaveCustomItem { itemTag = itemTag };
            var newList = new List<TList> { createEntry(dataTag, newValue) };
            setList(newItem, newList);
            saveObject.saveCustomItems.Add(newItem);
        }

        #endregion

        #region === Screenshot & Texture Methods ===

        /// <summary>
        /// Captures a screenshot from the specified camera and stores it inside the SaveCustomObject.
        /// The resolution is automatically adjusted according to the configured pixel limit to prevent oversized textures.
        /// </summary>
        /// <param name="targetCamera">The camera used to render and capture the screenshot.</param>
        public static void CaptureScreenshot(Camera targetCamera)
        {
            // Retrieves the current SaveCustomObject instance. Returns if not found.
            var saveObject = GetSaveCustomObject();
            if (saveObject == null) return;

            // Validates that the camera used to capture the screenshot is not null.
            if (targetCamera == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Camera is null.");
                return;
            }

            // Gets the current screen resolution.
            int width = Screen.width;
            int height = Screen.height;

            // Calculates aspect ratio for maintaining proportional scaling.
            float aspect = (float)width / height;

            // Adjusts resolution to ensure it does not exceed the allowed pixel limit.
            if (width > height)
            {
                width = Mathf.Min(width, saveObject.pixelLimit);
                height = Mathf.RoundToInt(width / aspect);
            }
            else
            {
                height = Mathf.Min(height, saveObject.pixelLimit);
                width = Mathf.RoundToInt(height * aspect);
            }

            // Creates a temporary RenderTexture to capture the camera output.
            RenderTexture rt = new(width, height, 24);
            targetCamera.targetTexture = rt;
            targetCamera.Render(); // Renders the camera view into the RenderTexture.

            // Creates a Texture2D to store the final screenshot image.
            Texture2D screenshot = new(rt.width, rt.height, TextureFormat.RGB24, false);
            RenderTexture.active = rt; // Sets the active texture to read from.

            // Reads the rendered pixels into the Texture2D.
            screenshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            screenshot.Apply(); // Finalizes texture data in memory.

            // Resets texture states.
            RenderTexture.active = null;
            targetCamera.targetTexture = null;

            // Converts the screenshot to a PNG byte array for saving.
            saveObject.screenshot = screenshot.EncodeToPNG();

            // Cleans up temporary objects to avoid memory leaks.
            Object.Destroy(rt);
            Object.Destroy(screenshot);
        }

        /// <summary>
        /// Converts a stored PNG screenshot byte array back into a Texture2D.
        /// </summary>
        /// <param name="screenshot">The byte array representing the saved screenshot.</param>
        /// <returns>Returns a Texture2D reconstructed from the stored screenshot data, or null if invalid.</returns>
        public static Texture2D RenderScreenshot(byte[] screenshot)
        {
            // Validates screenshot data.
            if (screenshot == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Screenshot data is null.");
                return null;
            }

            // Creates a minimal texture and loads the screenshot data into it.
            Texture2D texture = new(1, 1);
            texture.LoadImage(screenshot); // Automatically resizes texture to match the data.
            return texture;
        }

        /// <summary>
        /// Converts a Texture2D into a Unity Sprite.
        /// Useful for displaying saved screenshots in UI components.
        /// </summary>
        /// <param name="texture">The texture that will be converted into a sprite.</param>
        /// <returns>Returns a Sprite created from the provided texture, or null if invalid.</returns>
        public static Sprite TextureToSprite(Texture2D texture)
        {
            // Validates the input texture.
            if (texture == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Texture is null.");
                return null;
            }

            // Creates the sprite using the full size of the texture.
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height), // Defines the visible region of the sprite.
                new Vector2(0.5f, 0.5f) // Defines pivot at the center.
            );
        }

        #endregion

        #region === Scene Component & Auto-Save ===

        /// <summary>
        /// Attempts to locate the GameObject named "[Save Custom Object]" in the scene and retrieve its
        /// SaveCustomInScene component. This component is responsible for managing in-scene data related to saving.
        /// </summary>
        /// <returns>
        /// Returns the SaveCustomInScene component if found. If the object or component is missing, logs an error and returns null.
        /// </returns>
        public static SaveCustomInScene GetComponentSaveCustomInScene()
        {
            // Attempts to find the main Save Custom Object in the scene.
            var obj = GameObject.Find("[Save Custom Object]");

            // Validates that the object exists in the scene.
            if (obj == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - [Save Custom Object] not found.");
                return null;
            }

            // Attempts to retrieve the SaveCustomInScene component from the found object.
            if (obj.TryGetComponent(out SaveCustomInScene component))
            {
                return component;
            }

            // Logs an error if the expected component is missing.
            Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomInScene component missing.");
            return null;
        }

        /// <summary>
        /// Enables or disables automatic saving functionality based on the provided state.
        /// </summary>
        /// <param name="state">Determines whether auto-save should be enabled (true) or disabled (false).</param>
        public static void SetAutoSave(bool state)
        {
            // Retrieves the SaveCustomObject instance. Returns if not found.
            var saveObject = GetSaveCustomObject();
            if (saveObject == null) return;

            // Sets autosave state.
            saveObject.autosaveEnabled = state;
        }

        /// <summary>
        /// Triggers the auto-save process if the AutoSaveCustom component is found on the Save Custom Object.
        /// </summary>
        public static void SaveEvent()
        {
            // Attempts to find the Save Custom Object in the scene.
            var obj = GameObject.Find("[Save Custom Object]");

            // Validates object presence before attempting to save.
            if (obj == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Cannot run SaveEvent. Object not found.");
                return;
            }

            // Attempts to retrieve and execute the auto-save system.
            if (obj.TryGetComponent(out AutoSaveCustom autoSave))
            {
                autoSave.SaveAutoGame();
            }
        }

        #endregion
    }
}