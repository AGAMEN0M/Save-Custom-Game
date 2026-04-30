/*
 * ---------------------------------------------------------------------------
 * Description: Centralized utility for managing SaveCustomObject data.
 *              Provides cached access for reading and writing primitive types
 *              (float, int, string, bool) and vector-based data (Vector4).
 *              Includes safe retrieval with fallback defaults and debug logging,
 *              automatic data structure creation, scene component caching,
 *              save control utilities, screenshot capture/restore pipeline,
 *              and conversion helpers (e.g., Vector4 to Quaternion).
 *              
 *              Designed for performance, safety, and debuggability across
 *              the entire save system.
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
        #region === Cached References ===

        /// <summary>
        /// Cached reference to the loaded SaveCustomObject.
        /// </summary>
        private static SaveCustomObject saveCustomObject;

        /// <summary>
        /// Cached reference to the SaveCustomInScene component.
        /// </summary>
        private static SaveCustomInScene cachedSceneComponent;

        /// <summary>
        /// Cache for fast item lookup using itemTag as key.
        /// </summary>
        private static readonly Dictionary<string, SaveCustomItem> itemCache = new();

        #endregion

        #region === Debug Control ===

        /// <summary>
        /// Stores already logged missing keys to prevent log spam.
        /// </summary>
        private static readonly HashSet<string> loggedMissingKeys = new();

        /// <summary>
        /// Logs a warning when a default value is used.
        /// Ensures each key logs only once.
        /// </summary>
        private static void LogDefaultUsage(string itemTag, string tag, Type type)
        {
            string key = $"{itemTag}.{tag}.{type.Name}";

            // Prevent duplicate logs.
            if (loggedMissingKeys.Contains(key)) return;

            loggedMissingKeys.Add(key);

            Debug.LogWarning($"{GetCallingMethodInfo()} - Using default value for missing key: '{itemTag}.{tag}' (Type: {type.Name})");
        }

        #endregion

        #region === Get Save Object ===

        /// <summary>
        /// Retrieves and caches the SaveCustomObject from Resources.
        /// If not already loaded, it will attempt to load and build the internal cache.
        /// </summary>
        /// <returns>Returns the cached SaveCustomObject instance, or null if it could not be loaded.</returns>
        public static SaveCustomObject GetSaveCustomObject()
        {
            // Return cached instance if already loaded.
            if (saveCustomObject != null) return saveCustomObject;

            // Load the ScriptableObject from Resources folder.
            saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data");

            // Validate load result.
            if (saveCustomObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null.");
                return null;
            }

            BuildCache(); // Build lookup cache after successful load.

            return saveCustomObject;
        }

        /// <summary>
        /// Builds the dictionary cache for fast item lookup using itemTag as key.
        /// This avoids repeated linear searches when accessing save data.
        /// </summary>
        private static void BuildCache()
        {
            itemCache.Clear(); // Clear previous cache to avoid stale references.
            if (saveCustomObject == null) return; // Validate main object.
            if (saveCustomObject.saveCustomItems == null) return; // Validate item list.

            // Populate dictionary for O(1) access.
            foreach (var item in saveCustomObject.saveCustomItems)
            {
                // Avoid duplicate keys (safety check).
                if (!itemCache.ContainsKey(item.itemTag)) itemCache.Add(item.itemTag, item);
            }
        }

        #endregion

        #region === Get Methods (Safe) ===

        /// <summary>
        /// Retrieves a float value from the save data.
        /// Returns the stored value if found; otherwise returns default (0f) and logs a warning.
        /// </summary>
        /// <param name="itemTag">The identifier of the save item.</param>
        /// <param name="tag">The identifier of the value inside the item.</param>
        /// <returns>The stored float value, or 0f if not found.</returns>
        public static float GetFloat(string itemTag, string tag)
        {
            // Attempt to retrieve value.
            if (TryGetFloat(itemTag, tag, out var v)) return v;

            // Log fallback usage and return default.
            LogDefaultUsage(itemTag, tag, typeof(float));
            return default;
        }

        /// <summary>
        /// Retrieves an int value from the save data.
        /// Returns the stored value if found; otherwise returns default (0) and logs a warning.
        /// </summary>
        /// <param name="itemTag">The identifier of the save item.</param>
        /// <param name="tag">The identifier of the value inside the item.</param>
        /// <returns>The stored int value, or 0 if not found.</returns>
        public static int GetInt(string itemTag, string tag)
        {
            if (TryGetInt(itemTag, tag, out var v)) return v;

            LogDefaultUsage(itemTag, tag, typeof(int));
            return default;
        }

        /// <summary>
        /// Retrieves a string value from the save data.
        /// Returns the stored value if found; otherwise returns an empty string and logs a warning.
        /// </summary>
        /// <param name="itemTag">The identifier of the save item.</param>
        /// <param name="tag">The identifier of the value inside the item.</param>
        /// <returns>The stored string value, or an empty string if not found.</returns>
        public static string GetString(string itemTag, string tag)
        {
            if (TryGetString(itemTag, tag, out var v)) return v;

            LogDefaultUsage(itemTag, tag, typeof(string));
            return string.Empty;
        }

        /// <summary>
        /// Retrieves a bool value from the save data.
        /// Returns the stored value if found; otherwise returns false and logs a warning.
        /// </summary>
        /// <param name="itemTag">The identifier of the save item.</param>
        /// <param name="tag">The identifier of the value inside the item.</param>
        /// <returns>The stored bool value, or false if not found.</returns>
        public static bool GetBool(string itemTag, string tag)
        {
            if (TryGetBool(itemTag, tag, out var v)) return v;

            LogDefaultUsage(itemTag, tag, typeof(bool));
            return default;
        }

        /// <summary>
        /// Retrieves a Vector4 value from the save data.
        /// Returns the stored value if found; otherwise returns default and logs a warning.
        /// </summary>
        /// <param name="itemTag">The identifier of the save item.</param>
        /// <param name="tag">The identifier of the value inside the item.</param>
        /// <returns>The stored Vector4 value, or default if not found.</returns>
        public static Vector4 GetVector(string itemTag, string tag)
        {
            if (TryGetVector(itemTag, tag, out var v)) return v;

            LogDefaultUsage(itemTag, tag, typeof(Vector4));
            return default;
        }

        #endregion

        #region === TryGet Methods ===

        /// <summary>
        /// Attempts to retrieve a stored float value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Output value if found.</param>
        /// <returns>True if found, otherwise false.</returns>
        public static bool TryGetFloat(string itemTag, string tag, out float value)
            => TryFind(itemTag, tag, i => i.itemFloat, e => e.floatTag, e => e.floatValue, out value);

        /// <summary>
        /// Attempts to retrieve a stored int value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Output value if found.</param>
        /// <returns>True if found, otherwise false.</returns>
        public static bool TryGetInt(string itemTag, string tag, out int value)
            => TryFind(itemTag, tag, i => i.itemInt, e => e.intTag, e => e.intValue, out value);

        /// <summary>
        /// Attempts to retrieve a stored string value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Output value if found.</param>
        /// <returns>True if found, otherwise false.</returns>
        public static bool TryGetString(string itemTag, string tag, out string value)
            => TryFind(itemTag, tag, i => i.itemString, e => e.stringTag, e => e.stringValue, out value);

        /// <summary>
        /// Attempts to retrieve a stored bool value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Output value if found.</param>
        /// <returns>True if found, otherwise false.</returns>
        public static bool TryGetBool(string itemTag, string tag, out bool value)
            => TryFind(itemTag, tag, i => i.itemBool, e => e.boolTag, e => e.boolValue, out value);

        /// <summary>
        /// Attempts to retrieve a stored Vector4 value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Output value if found.</param>
        /// <returns>True if found, otherwise false.</returns>
        public static bool TryGetVector(string itemTag, string tag, out Vector4 value)
            => TryFind(itemTag, tag, i => i.itemVector, e => e.vectorTag, e => e.vectorValue, out value);

        #endregion

        #region === Set Methods ===

        /// <summary>
        /// Sets or creates a float value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Value to store.</param>
        public static void SetFloat(string itemTag, string tag, float value)
            => SetValue(itemTag, tag, value, i => i.itemFloat, (i, l) => i.itemFloat = l,
                e => e.floatTag, (e, v) => e.floatValue = v,
                (t, v) => new SaveCustomFloat { floatTag = t, floatValue = v });

        /// <summary>
        /// Sets or creates an int value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Value to store.</param>
        public static void SetInt(string itemTag, string tag, int value)
            => SetValue(itemTag, tag, value, i => i.itemInt, (i, l) => i.itemInt = l,
                e => e.intTag, (e, v) => e.intValue = v,
                (t, v) => new SaveCustomInt { intTag = t, intValue = v });

        /// <summary>
        /// Sets or creates a string value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Value to store.</param>
        public static void SetString(string itemTag, string tag, string value)
            => SetValue(itemTag, tag, value, i => i.itemString, (i, l) => i.itemString = l,
                e => e.stringTag, (e, v) => e.stringValue = v,
                (t, v) => new SaveCustomString { stringTag = t, stringValue = v });

        /// <summary>
        /// Sets or creates a bool value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Value to store.</param>
        public static void SetBool(string itemTag, string tag, bool value)
            => SetValue(itemTag, tag, value, i => i.itemBool, (i, l) => i.itemBool = l,
                e => e.boolTag, (e, v) => e.boolValue = v,
                (t, v) => new SaveCustomBool { boolTag = t, boolValue = v });

        /// <summary>
        /// Sets a Vector2 value (stored as Vector4).
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="v">Value to store.</param>
        public static void SetVector(string itemTag, string tag, Vector2 v)
            => SetVector4(itemTag, tag, new Vector4(v.x, v.y));

        /// <summary>
        /// Sets a Vector3 value (stored as Vector4).
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="v">Value to store.</param>
        public static void SetVector(string itemTag, string tag, Vector3 v)
            => SetVector4(itemTag, tag, new Vector4(v.x, v.y, v.z));

        /// <summary>
        /// Sets a Vector4 value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="v">Value to store.</param>
        public static void SetVector(string itemTag, string tag, Vector4 v)
            => SetVector4(itemTag, tag, v);

        /// <summary>
        /// Sets a Quaternion value (stored as Vector4).
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="q">Value to store.</param>
        public static void SetVector(string itemTag, string tag, Quaternion q)
            => SetVector4(itemTag, tag, new Vector4(q.x, q.y, q.z, q.w));

        /// <summary>
        /// Sets or creates a Vector4 value.
        /// </summary>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Value to store.</param>
        public static void SetVector4(string itemTag, string tag, Vector4 value)
            => SetValue(itemTag, tag, value, i => i.itemVector, (i, l) => i.itemVector = l,
                e => e.vectorTag, (e, v) => e.vectorValue = v,
                (t, v) => new SaveCustomVector { vectorTag = t, vectorValue = v });

        #endregion

        #region === Core Logic ===

        /// <summary>
        /// Attempts to locate and retrieve a stored value from the cache.
        /// Returns false if the item, list, or entry does not exist.
        /// </summary>
        /// <typeparam name="TList">Entry container type.</typeparam>
        /// <typeparam name="TValue">Stored value type.</typeparam>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="getList">Delegate to access the list.</param>
        /// <param name="getTag">Delegate to compare entry keys.</param>
        /// <param name="getValue">Delegate to extract value.</param>
        /// <param name="value">Output value if found.</param>
        /// <returns>True if found, otherwise false.</returns>
        private static bool TryFind<TList, TValue>(string itemTag, string tag, Func<SaveCustomItem, List<TList>> getList, Func<TList, string> getTag, Func<TList, TValue> getValue, out TValue value)
        {
            value = default; // Initialize output with default to ensure predictable fallback.

            // Ensure save object is loaded before accessing cache.
            var save = GetSaveCustomObject();
            if (save == null) return false;

            // Try to retrieve cached item (O(1) lookup).
            // If not found, item does not exist in save structure.
            if (!itemCache.TryGetValue(itemTag, out var item)) return false;

            var list = getList(item); // Retrieve the specific data list (e.g., floats, ints, vectors).
            if (list == null) return false; // If list is null, this type was never initialized for this item.

            // Iterate through entries to find matching tag.
            // Note: List is expected to be small, so linear search is acceptable.
            foreach (var entry in list)
            {
                if (getTag(entry) == tag)
                {
                    // Match found → extract and return value.
                    value = getValue(entry);
                    return true;
                }
            }

            return false; // Entry not found.
        }

        /// <summary>
        /// Inserts or updates a value in the save structure.
        /// Automatically creates missing items or lists when necessary.
        /// Keeps cache and data synchronized.
        /// </summary>
        /// <typeparam name="TList">Entry container type.</typeparam>
        /// <typeparam name="TValue">Stored value type.</typeparam>
        /// <param name="itemTag">Item identifier.</param>
        /// <param name="tag">Entry identifier.</param>
        /// <param name="value">Value to store.</param>
        /// <param name="getList">Delegate to retrieve list.</param>
        /// <param name="setList">Delegate to assign list.</param>
        /// <param name="getTag">Delegate to compare keys.</param>
        /// <param name="setValue">Delegate to assign value.</param>
        /// <param name="create">Delegate to create new entry.</param>
        private static void SetValue<TList, TValue>(string itemTag, string tag, TValue value, Func<SaveCustomItem, List<TList>> getList, Action<SaveCustomItem, List<TList>> setList, Func<TList, string> getTag, Action<TList, TValue> setValue, Func<string, TValue, TList> create)
        {
            // Ensure save object is available.
            var save = GetSaveCustomObject();
            if (save == null) return;

            // Ensure root list exists.
            save.saveCustomItems ??= new List<SaveCustomItem>();

            // Try to get existing item from cache.
            if (!itemCache.TryGetValue(itemTag, out var item))
            {
                // Item does not exist → create and register it.
                item = new SaveCustomItem { itemTag = itemTag };

                // Add to both data structure and cache for consistency.
                save.saveCustomItems.Add(item);
                itemCache[itemTag] = item;
            }

            // Retrieve the list for this specific type (float, int, etc.).
            var list = getList(item);

            // If list is missing, initialize it.
            if (list == null)
            {
                list = new List<TList>();
                setList(item, list);
            }

            // Try to find existing entry with the same tag.
            foreach (var entry in list)
            {
                if (getTag(entry) == tag)
                {
                    // Entry exists → update value in-place.
                    setValue(entry, value);
                    return;
                }
            }

            list.Add(create(tag, value)); // Entry does not exist → create and append new one.
        }

        #endregion

        #region === Scene Access (Cached) ===

        /// <summary>
        /// Retrieves and caches the SaveCustomInScene component from the scene.
        /// Avoids repeated GameObject.Find calls.
        /// </summary>
        /// <returns>Cached component or null if not found.</returns>
        public static SaveCustomInScene GetComponentSaveCustomInScene()
        {
            // Return cached reference if already resolved.
            // This avoids expensive scene searches on subsequent calls.
            if (cachedSceneComponent != null) return cachedSceneComponent;

            // Attempt to locate the main Save Custom Object in the scene.
            // This uses GameObject.Find, so it should only happen once.
            var obj = GameObject.Find("[Save Custom Object]");

            // Validate that the object exists.
            if (obj == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Object not found.");
                return null;
            }

            // Try to retrieve the required component and cache it.
            // If successful, future calls will skip all lookup logic.
            if (obj.TryGetComponent(out cachedSceneComponent)) return cachedSceneComponent;

            // If the component is missing, log a clear error for debugging.
            Debug.LogError($"{GetCallingMethodInfo()} - Component missing.");
            return null;
        }

        #endregion

        #region === Save Control ===

        /// <summary>
        /// Enables or disables the auto-save system.
        /// </summary>
        /// <param name="state">True to enable, false to disable.</param>
        public static void SetAutoSave(bool state)
        {
            var save = GetSaveCustomObject();
            if (save == null) return;

            save.autosaveEnabled = state;
        }

        /// <summary>
        /// Triggers the auto-save process through the AutoSaveCustom component.
        /// </summary>
        public static void SaveEvent()
        {
            var comp = GetComponentSaveCustomInScene();
            if (comp == null) return;

            if (comp.TryGetComponent(out AutoSaveCustom auto)) auto.SaveAutoGame();
        }

        #endregion
        
        #region === Vector Conversion ===

        /// <summary>
        /// Converts a Vector4 into a normalized Quaternion.
        /// Ensures the quaternion remains valid even if data is corrupted.
        /// </summary>
        /// <param name="vector">The Vector4 representing quaternion data.</param>
        /// <returns>Returns a normalized Quaternion.</returns>
        public static Quaternion Vector4ToQuaternion(Vector4 vector)
        {
            // Create quaternion from vector.
            Quaternion q = new(vector.x, vector.y, vector.z, vector.w);

            // Normalize to avoid invalid rotations.
            float magnitude = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);

            // Avoid division by zero.
            if (magnitude > 0f)
            {
                float inv = 1f / magnitude;
                q.x *= inv;
                q.y *= inv;
                q.z *= inv;
                q.w *= inv;
            }
            else
            {
                // Fallback to identity if corrupted.
                q = Quaternion.identity;
                Debug.LogWarning($"{GetCallingMethodInfo()} - Invalid quaternion data. Using identity.");
            }

            return q;
        }

        #endregion

        #region === Screenshot & Texture Methods ===

        /// <summary>
        /// Captures a screenshot from the specified camera and stores it inside the SaveCustomObject.
        /// Ensures proper restoration of render states and avoids memory leaks.
        /// </summary>
        /// <param name="targetCamera">The camera used to render and capture the screenshot.</param>
        public static void CaptureScreenshot(Camera targetCamera)
        {
            // Retrieve save object.
            var saveObject = GetSaveCustomObject();
            if (saveObject == null) return;

            // Validate camera.
            if (targetCamera == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Camera is null.");
                return;
            }

            // Get screen size.
            int width = Screen.width;
            int height = Screen.height;

            // Calculate aspect ratio.
            float aspect = (float)width / height;

            // Clamp resolution to pixel limit.
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

            // Store previous states (IMPORTANT).
            RenderTexture previousRT = RenderTexture.active;
            RenderTexture previousCameraRT = targetCamera.targetTexture;

            RenderTexture rt = null;
            Texture2D screenshot = null;

            try
            {
                // Create render texture.
                rt = new RenderTexture(width, height, 24);

                targetCamera.targetTexture = rt;
                targetCamera.Render();

                // Create texture.
                screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

                RenderTexture.active = rt;

                // Read pixels.
                screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                screenshot.Apply();

                // Encode to PNG.
                saveObject.screenshot = screenshot.EncodeToPNG();
            }
            finally
            {
                // Restore previous states.
                RenderTexture.active = previousRT;
                targetCamera.targetTexture = previousCameraRT;

                // Cleanup.
                if (rt != null) Object.Destroy(rt);
                if (screenshot != null) Object.Destroy(screenshot);
            }
        }

        /// <summary>
        /// Converts a stored PNG screenshot byte array back into a Texture2D.
        /// Uses non-readable texture to reduce memory usage.
        /// </summary>
        /// <param name="screenshot">The byte array representing the saved screenshot.</param>
        /// <returns>Returns a Texture2D reconstructed from the stored screenshot data, or null if invalid.</returns>
        public static Texture2D RenderScreenshot(byte[] screenshot)
        {
            if (screenshot == null || screenshot.Length == 0)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Screenshot data is null or empty.");
                return null;
            }

            // Create minimal texture.
            Texture2D texture = new(2, 2, TextureFormat.RGB24, false);

            // Load image and mark as non-readable to save memory.
            if (!texture.LoadImage(screenshot, markNonReadable: true))
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Failed to load screenshot.");
                Object.Destroy(texture);
                return null;
            }

            return texture;
        }

        /// <summary>
        /// Converts a Texture2D into a Unity Sprite.
        /// </summary>
        /// <param name="texture">The texture that will be converted into a sprite.</param>
        /// <returns>Returns a Sprite created from the provided texture, or null if invalid.</returns>
        public static Sprite TextureToSprite(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Texture is null.");
                return null;
            }

            // Create sprite.
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f // Pixels per unit (default safe value).
            );
        }

        #endregion
    }
}