/*
 * ---------------------------------------------------------------------------
 * Description: Central utility class for interacting with SaveCustomObject data 
 *              in a Unity project. Provides static methods to retrieve and modify 
 *              float, int, string, bool, and vector values; capture and render 
 *              screenshots; convert textures to sprites; manage auto-save flags; 
 *              and trigger save events. Ensures smooth data handling and error 
 *              logging throughout the save system.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Collections.Generic;
using UnityEngine;

using static SaveCustomGame.ExceptionUtility;

namespace SaveCustomGame
{
    public static class SaveDataUtility
    {
        /// <summary>
        /// Load and return the SaveCustomObject from Resources.
        /// Logs an error if the object is not found.
        /// </summary>
        public static SaveCustomObject GetSaveCustomObject()
        {
            var saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data");
            if (saveCustomObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return null;
            }
            return saveCustomObject;
        }

        /// <summary>
        /// Retrieve a Vector4 value from SaveCustomObject based on item and vector tags.
        /// Throws KeyNotFoundException if the tag is not found.
        /// </summary>
        public static Vector4 GetVector(string itemTag, string vectorTag)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return Vector4.zero;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the vector with the specified vectorTag.
                    foreach (var customVector in customItem.itemVector)
                    {
                        if (customVector.vectorTag == vectorTag)
                        {
                            return customVector.vectorValue;
                        }
                    }
                    throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Vector tag '{vectorTag}' not found in item '{itemTag}'\n");
                }
            }
            throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
        }

        /// <summary>
        /// Retrieve a float value from SaveCustomObject based on item and float tags.
        /// Throws KeyNotFoundException if the tag is not found.
        /// </summary>
        public static float GetFloat(string itemTag, string floatTag)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return 0;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the float with the specified floatTag.
                    foreach (var customFloat in customItem.itemFloat)
                    {
                        if (customFloat.floatTag == floatTag)
                        {
                            return customFloat.floatValue;
                        }
                    }
                    throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Float tag '{floatTag}' not found in item '{itemTag}'\n");
                }
            }
            throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
        }

        /// <summary>
        /// Retrieve an integer value from SaveCustomObject based on item and int tags.
        /// Throws KeyNotFoundException if the tag is not found.
        /// </summary>
        public static int GetInt(string itemTag, string intTag)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return 0;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the int with the specified intTag.
                    foreach (var customInt in customItem.itemInt)
                    {
                        if (customInt.intTag == intTag)
                        {
                            return customInt.intValue;
                        }
                    }
                    throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Int tag '{intTag}' not found in item '{itemTag}'\n");
                }
            }
            throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
        }

        /// <summary>
        /// Retrieve a string value from SaveCustomObject based on item and string tags.
        /// Throws KeyNotFoundException if the tag is not found.
        /// </summary>
        public static string GetString(string itemTag, string stringTag)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return "Error: SaveCustomObject is null!";
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the string with the specified stringTag.
                    foreach (var customString in customItem.itemString)
                    {
                        if (customString.stringTag == stringTag)
                        {
                            return customString.stringValue;
                        }
                    }
                    throw new KeyNotFoundException($"{GetCallingMethodInfo()} - String tag '{stringTag}' not found in item '{itemTag}'\n");
                }
            }
            throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
        }

        /// <summary>
        /// Retrieve a boolean value from SaveCustomObject based on item and bool tags.
        /// Throws KeyNotFoundException if the tag is not found.
        /// </summary>
        public static bool GetBool(string itemTag, string boolTag)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return false;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the bool with the specified boolTag.
                    foreach (var customBool in customItem.itemBool)
                    {
                        if (customBool.boolTag == boolTag)
                        {
                            return customBool.boolValue;
                        }
                    }
                    throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Bool tag '{boolTag}' not found in item '{itemTag}'\n");
                }
            }
            throw new KeyNotFoundException($"{GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
        }

        /// <summary>
        /// Set a Vector4-compatible value (Vector2, Vector3, Vector4, or Quaternion) in SaveCustomObject.
        /// Logs an error if the type is unsupported.
        /// </summary>
        public static void SetVector(string itemTag, string vectorTag, object newValue)
        {
            if (newValue is Vector2 v2)
            {
                SetVector4(itemTag, vectorTag, new Vector4(v2.x, v2.y, 0f, 0f));
            }
            else if (newValue is Vector3 v3)
            {
                SetVector4(itemTag, vectorTag, new Vector4(v3.x, v3.y, v3.z, 0f));
            }
            else if (newValue is Vector4 v4)
            {
                SetVector4(itemTag, vectorTag, v4);
            }
            else if (newValue is Quaternion q)
            {
                SetVector4(itemTag, vectorTag, new Vector4(q.x, q.y, q.z, q.w));
            }
            else
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Unsupported type for SetVector.\n");
            }
        }

        /// <summary>
        /// Set a Vector4 value in SaveCustomObject based on item and vector tags.
        /// Creates new items or tags if they do not exist.
        /// </summary>
        public static void SetVector4(string itemTag, string vectorTag, Vector4 newValue)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the vector with the specified vectorTag.
                    foreach (var customVector in customItem.itemVector)
                    {
                        if (customVector.vectorTag == vectorTag)
                        {
                            customVector.vectorValue = newValue;
                            return;
                        }
                    }

                    // Vector tag not found, create a new one.
                    SaveCustomVector newCustomVector = new()
                    {
                        vectorTag = vectorTag,
                        vectorValue = newValue
                    };

                    customItem.itemVector.Add(newCustomVector);
                    return;
                }
            }

            // Item tag not found, create a new item with the vector.
            SaveCustomItem newCustomItem = new()
            {
                itemTag = itemTag,
                itemVector = new List<SaveCustomVector> { new() { vectorTag = vectorTag, vectorValue = newValue } }
            };

            saveObject.saveCustomItems.Add(newCustomItem);
        }

        /// <summary>
        /// Set a float value in SaveCustomObject based on item and float tags.
        /// Creates new items or tags if they do not exist.
        /// </summary>
        public static void SetFloat(string itemTag, string floatTag, float newValue)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the float with the specified floatTag.
                    foreach (var customFloat in customItem.itemFloat)
                    {
                        if (customFloat.floatTag == floatTag)
                        {
                            customFloat.floatValue = newValue;
                            return;
                        }
                    }

                    // Float tag not found, create a new one.
                    SaveCustomFloat newCustomFloat = new()
                    {
                        floatTag = floatTag,
                        floatValue = newValue
                    };

                    customItem.itemFloat.Add(newCustomFloat);
                    return;
                }
            }

            // Item tag not found, create a new item with the float.
            SaveCustomItem newCustomItem = new()
            {
                itemTag = itemTag,
                itemFloat = new List<SaveCustomFloat> { new() { floatTag = floatTag, floatValue = newValue } }
            };

            saveObject.saveCustomItems.Add(newCustomItem);
        }

        /// <summary>
        /// Set an integer value in SaveCustomObject based on item and int tags.
        /// Creates new items or tags if they do not exist.
        /// </summary>
        public static void SetInt(string itemTag, string intTag, int newValue)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the int with the specified intTag.
                    foreach (var customInt in customItem.itemInt)
                    {
                        if (customInt.intTag == intTag)
                        {
                            customInt.intValue = newValue;
                            return;
                        }
                    }

                    // Int tag not found, create a new one.
                    SaveCustomInt newCustomInt = new()
                    {
                        intTag = intTag,
                        intValue = newValue
                    };

                    customItem.itemInt.Add(newCustomInt);
                    return;
                }
            }

            // Item tag not found, create a new item with the int.
            SaveCustomItem newCustomItem = new()
            {
                itemTag = itemTag,
                itemInt = new List<SaveCustomInt> { new() { intTag = intTag, intValue = newValue } }
            };

            saveObject.saveCustomItems.Add(newCustomItem);
        }

        /// <summary>
        /// Set a string value in SaveCustomObject based on item and string tags.
        /// Creates new items or tags if they do not exist.
        /// </summary>
        public static void SetString(string itemTag, string stringTag, string newValue)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the string with the specified stringTag.
                    foreach (var customString in customItem.itemString)
                    {
                        if (customString.stringTag == stringTag)
                        {
                            customString.stringValue = newValue;
                            return;
                        }
                    }

                    // String tag not found, create a new one.
                    SaveCustomString newCustomString = new()
                    {
                        stringTag = stringTag,
                        stringValue = newValue
                    };

                    customItem.itemString.Add(newCustomString);
                    return;
                }
            }

            // Item tag not found, create a new item with the string.
            SaveCustomItem newCustomItem = new()
            {
                itemTag = itemTag,
                itemString = new List<SaveCustomString> { new() { stringTag = stringTag, stringValue = newValue } }
            };

            saveObject.saveCustomItems.Add(newCustomItem);
        }

        /// <summary>
        /// Set a boolean value in SaveCustomObject based on item and bool tags.
        /// Creates new items or tags if they do not exist.
        /// </summary>
        public static void SetBool(string itemTag, string boolTag, bool newValue)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return;
            }

            // Find the item with the specified itemTag.
            foreach (var customItem in saveObject.saveCustomItems)
            {
                if (customItem.itemTag == itemTag)
                {
                    // Find the bool with the specified boolTag.
                    foreach (var customBool in customItem.itemBool)
                    {
                        if (customBool.boolTag == boolTag)
                        {
                            customBool.boolValue = newValue;
                            return;
                        }
                    }

                    // Bool tag not found, create a new one.
                    SaveCustomBool newCustomBool = new()
                    {
                        boolTag = boolTag,
                        boolValue = newValue
                    };

                    customItem.itemBool.Add(newCustomBool);
                    return;
                }
            }

            // Item tag not found, create a new item with the bool.
            SaveCustomItem newCustomItem = new()
            {
                itemTag = itemTag,
                itemBool = new List<SaveCustomBool> { new() { boolTag = boolTag, boolValue = newValue } }
            };

            saveObject.saveCustomItems.Add(newCustomItem);
        }

        /// <summary>
        /// Capture a screenshot using a target camera and assign it to SaveCustomObject.
        /// Respects pixel limits and aspect ratio defined in the object.
        /// </summary>
        public static void CaptureScreenshot(Camera targetCamera)
        {
            var saveObject = GetSaveCustomObject();
            if (saveObject == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
                return;
            }

            // Check if either targetCamera is not defined.
            if (targetCamera == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Camera are not defined!\n");
                return; // Exit the method if either parameter is null.
            }

            // Get the original width and height of the screen.
            int originalWidth = Screen.width;
            int originalHeight = Screen.height;

            // Calculate new width and height that maintains the aspect ratio and fits within the pixel limit.
            int newWidth, newHeight;
            float aspectRatio = (float)originalWidth / originalHeight;
            if (originalWidth > originalHeight)
            {
                newWidth = Mathf.Min(originalWidth, saveObject.pixelLimit);
                newHeight = Mathf.RoundToInt(newWidth / aspectRatio);
            }
            else
            {
                newHeight = Mathf.Min(originalHeight, saveObject.pixelLimit);
                newWidth = Mathf.RoundToInt(newHeight * aspectRatio);
            }

            RenderTexture renderTexture = new(newWidth, newHeight, 24); // Create a new render texture with the calculated dimensions.
            targetCamera.targetTexture = renderTexture; // Set the target texture of the camera to the render texture.
            targetCamera.Render(); // Render the target camera.
            while (!renderTexture.IsCreated()) { Debug.Log("Waiting for camera rendering..."); } // Wait until the render texture is created.

            Texture2D screenshotTexture = new(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false); // Create a new Texture2D to read the pixels from the render texture.
            RenderTexture.active = renderTexture; // Set the active render texture to the render texture.

            // Read the pixels from the render texture and apply them to the screenshot texture.
            screenshotTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            screenshotTexture.Apply();

            RenderTexture.active = null; // Reset the active render texture.
            saveObject.screenshot = screenshotTexture.EncodeToPNG(); // Encode the screenshot texture to PNG format and assign it to SaveCustomObject.
            Object.Destroy(screenshotTexture); // Destroy the screenshot texture to release memory.
            targetCamera.targetTexture = null; // Set the target texture of the camera back to null.
        }

        /// <summary>
        /// Render a screenshot from a byte array and return it as a Texture2D.
        /// </summary>
        public static Texture2D RenderScreenshot(byte[] screenshot)
        {
            // Check if the screenshot byte array is not defined.
            if (screenshot == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - screenshot are not defined!\n");
                return null; // Return null if the screenshot is null.
            }

            Texture2D texture = new(1, 1); // Create a new Texture2D.
            texture.LoadImage(screenshot); // Load the image data from the screenshot byte array into the texture.
            return texture; // Return the generated texture.
        }

        /// <summary>
        /// Convert a Texture2D to a Sprite using full dimensions and centered pivot.
        /// </summary>
        public static Sprite TextureToSprite(Texture2D texture)
        {
            // Check if the provided texture is not defined.
            if (texture == null)
            {
                Debug.LogError($"{GetCallingMethodInfo()} - The texture is empty!\n");
                return null; // Return null if the texture is null.
            }

            Rect rect = new(0, 0, texture.width, texture.height); // Define the rectangle using the full dimensions of the texture.
            Vector2 pivot = new(0.5f, 0.5f); // Define the pivot point at the center of the texture.
            Sprite sprite = Sprite.Create(texture, rect, pivot); // Create a Sprite using the provided texture, rectangle, and pivot.
            return sprite; // Return the created sprite.
        }

        /// <summary>
        /// Find and return the SaveCustomInScene component from the scene if present.
        /// Logs errors if not found.
        /// </summary>
        public static SaveCustomInScene GetComponentSaveCustomInScene()
        {
            // Find the GameObject named "[Save Custom Object]" in the scene.
            var saveCustomObject = GameObject.Find("[Save Custom Object]");

            // Check if the GameObject is found in the scene.
            if (saveCustomObject != null)
            {
                // Attempt to get the SaveCustomInScene component attached to the GameObject.
                if (saveCustomObject.TryGetComponent(out SaveCustomInScene saveCustomInScene))
                {
                    Debug.Log($"{GetCallingMethodInfo()} - SaveCustomInScene script has been assigned successfully!\n");
                    return saveCustomInScene;
                }
                else
                {
                    Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomInScene not found in Save Custom Object!\n");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Save Custom Object not found!\n");
                return null;
            }
        }

        /// <summary>
        /// Enable auto-saving by setting the autosaveEnabled flag to true in SaveCustomObject.
        /// </summary>
        public static void EnableAutoSave()
        {
            var saveCustomObject = GetSaveCustomObject(); // Load the SaveCustomObject from Resources.

            if (saveCustomObject != null)
            {
                saveCustomObject.autosaveEnabled = true; // Set autosaveEnabled flag to true.
            }
            else
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
            }
        }

        /// <summary>
        /// Disable auto-saving by setting the autosaveEnabled flag to false in SaveCustomObject.
        /// </summary>
        public static void DisableAutoSave()
        {
            var saveCustomObject = GetSaveCustomObject(); // Load the SaveCustomObject from Resources.

            if (saveCustomObject != null)
            {
                saveCustomObject.autosaveEnabled = false; // Set autosaveEnabled flag to false.
            }
            else
            {
                Debug.LogError($"{GetCallingMethodInfo()} - SaveCustomObject is null!\n");
            }
        }

        /// <summary>
        /// Trigger an autosave event by calling SaveAutoGame() on AutoSaveCustom component.
        /// Logs the operation or errors if components are missing.
        /// </summary>
        public static void SaveEvent()
        {
            var saveCustomObject = GameObject.Find("[Save Custom Object]"); // Find the GameObject named "[Save Custom Object]" in the scene.

            // Check if the GameObject is found in the scene.
            if (saveCustomObject != null)
            {
                // Attempt to get the AutoSaveCustom component attached to the GameObject.
                if (saveCustomObject.TryGetComponent(out AutoSaveCustom autoSaveCustom))
                {
                    autoSaveCustom.SaveAutoGame(); // Trigger the SaveAutoGame method on the AutoSaveCustom component.
                    Debug.Log($"{GetCallingMethodInfo()} - Autosave has been done!\n");
                }
                else
                {
                    Debug.LogError($"{GetCallingMethodInfo()} - AutoSaveCustom not found in Save Custom Object!\n");
                }
            }
            else
            {
                Debug.LogError($"{GetCallingMethodInfo()} - Save Custom Object not found!\n");
            }
        }
    }
}