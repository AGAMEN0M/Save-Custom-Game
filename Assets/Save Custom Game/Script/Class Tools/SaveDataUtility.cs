/*
 * ---------------------------------------------------------------------------
 * Description: This utility script provides static methods for managing and 
 *              accessing save data within a Unity project. It includes functions 
 *              to retrieve and set various data types (float, int, string, bool) 
 *              in a `SaveCustomObject`, capture screenshots from a camera, convert 
 *              textures to sprites, and manage auto-saving. It also handles errors 
 *              related to missing data and components, ensuring smooth data 
 *              management and auto-save functionality.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/
using System.Collections.Generic;
using UnityEngine;

public static class SaveDataUtility
{
    // Load and return the SaveCustomObject from Resources.
    public static SaveCustomObject GetSaveCustomObject()
    {
        SaveCustomObject saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data");
        if (saveCustomObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
            return null;
        }
        return saveCustomObject;
    }

    // Retrieve a float value from SaveCustomObject based on item and float tags.
    public static float GetFloat(string itemTag, string floatTag)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Float tag '{floatTag}' not found in item '{itemTag}'\n");
            }
        }
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Retrieve an integer value from SaveCustomObject based on item and int tags.
    public static int GetInt(string itemTag, string intTag)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Int tag '{intTag}' not found in item '{itemTag}'\n");
            }
        }
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Retrieve a string value from SaveCustomObject based on item and string tags.
    public static string GetString(string itemTag, string stringTag)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - String tag '{stringTag}' not found in item '{itemTag}'\n");
            }
        }
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Retrieve a boolean value from SaveCustomObject based on item and bool tags.
    public static bool GetBool(string itemTag, string boolTag)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Bool tag '{boolTag}' not found in item '{itemTag}'\n");
            }
        }
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Set a float value in SaveCustomObject based on item and float tags.
    public static void SetFloat(string itemTag, string floatTag, float newValue)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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

    // Set an integer value in SaveCustomObject based on item and int tags.
    public static void SetInt(string itemTag, string intTag, int newValue)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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

    // Set a string value in SaveCustomObject based on item and string tags.
    public static void SetString(string itemTag, string stringTag, string newValue)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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

    // Set a boolean value in SaveCustomObject based on item and bool tags.
    public static void SetBool(string itemTag, string boolTag, bool newValue)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
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

    // Capture a screenshot using a target camera and assign it to SaveCustomObject.
    public static void CaptureScreenshot(Camera targetCamera)
    {
        SaveCustomObject saveObject = GetSaveCustomObject();
        if (saveObject == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n");
            return;
        }

        // Check if either targetCamera is not defined.
        if (targetCamera == null)
        {
            // Log an error if targetCamera is not defined.
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - Camera are not defined!\n");
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

    // Render a screenshot using the provided byte array and return it as a Texture2D.
    public static Texture2D RenderScreenshot(byte[] screenshot)
    {
        // Check if the screenshot byte array is not defined.
        if (screenshot == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - screenshot are not defined!\n"); // Log an error if the screenshot is not defined.
            return null; // Return null if the screenshot is null.
        }

        Texture2D texture = new(1, 1); // Create a new Texture2D.
        texture.LoadImage(screenshot); // Load the image data from the screenshot byte array into the texture.
        return texture; // Return the generated texture.
    }

    // Convert a Texture2D to a Sprite.
    public static Sprite TextureToSprite(Texture2D texture)
    {
        // Check if the provided texture is not defined.
        if (texture == null)
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - The texture is empty!\n"); // Log an error if the texture is not defined.
            return null; // Return null if the texture is null.
        }

        Rect rect = new(0, 0, texture.width, texture.height); // Define the rectangle using the full dimensions of the texture.
        Vector2 pivot = new(0.5f, 0.5f); // Define the pivot point at the center of the texture.
        Sprite sprite = Sprite.Create(texture, rect, pivot); // Create a Sprite using the provided texture, rectangle, and pivot.
        return sprite; // Return the created sprite.
    }

    // Find and assign the SaveCustomInScene component if found in the scene.
    public static SaveCustomInScene GetComponentSaveCustomInScene()
    {
        // Find the GameObject named "[Save Custom Object]" in the scene.
        GameObject saveCustomObject = GameObject.Find("[Save Custom Object]");

        // Check if the GameObject is found in the scene.
        if (saveCustomObject != null)
        {
            // Attempt to get the SaveCustomInScene component attached to the GameObject.
            if (saveCustomObject.TryGetComponent(out SaveCustomInScene saveCustomInScene))
            {
                // Log a success message if the component is assigned successfully.
                Debug.Log($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomInScene script has been assigned successfully!\n");
                return saveCustomInScene;
            }
            else
            {
                // Log an error if the SaveCustomInScene component is not found in the GameObject.
                Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomInScene not found in Save Custom Object!\n");
                return null;
            }
        }
        else
        {
            // Log an error if the GameObject named "[Save Custom Object]" is not found in the scene.
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - Save Custom Object not found!\n");
            return null;
        }
    }

    // Enable auto-saving by setting the autosaveEnabled flag to true in SaveCustomObject.
    public static void EnableAutoSave()
    {
        SaveCustomObject saveCustomObject = GetSaveCustomObject(); // Load the SaveCustomObject from Resources.
        
        if (saveCustomObject != null)
        {
            saveCustomObject.autosaveEnabled = true; // Set autosaveEnabled flag to true.
        }
        else
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n"); // Log an error if the SaveCustomObject is null.
        }
    }

    // Disable auto-saving by setting the autosaveEnabled flag to false in SaveCustomObject.
    public static void DisableAutoSave()
    {
        SaveCustomObject saveCustomObject = GetSaveCustomObject(); // Load the SaveCustomObject from Resources.
        
        if (saveCustomObject != null)
        {
            saveCustomObject.autosaveEnabled = false; // Set autosaveEnabled flag to false.
        }
        else
        {
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject is null!\n"); // Log an error if the SaveCustomObject is null.
        }
    }

    // Trigger an autosave event by calling SaveAutoGame() on AutoSaveCustom component.
    public static void SaveEvent()
    {
        GameObject saveCustomObject = GameObject.Find("[Save Custom Object]"); // Find the GameObject named "[Save Custom Object]" in the scene.

        // Check if the GameObject is found in the scene.
        if (saveCustomObject != null)
        {
            // Attempt to get the AutoSaveCustom component attached to the GameObject.
            if (saveCustomObject.TryGetComponent(out AutoSaveCustom autoSaveCustom))
            {
                autoSaveCustom.SaveAutoGame(); // Trigger the SaveAutoGame method on the AutoSaveCustom component.
                Debug.Log($"{ExceptionUtility.GetCallingMethodInfo()} - Autosave has been done!\n"); // Log a success message after triggering the save event.
            }
            else
            {
                // Log an error if the AutoSaveCustom component is not found in the GameObject.
                Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - AutoSaveCustom not found in Save Custom Object!\n");
            }
        }
        else
        {
            // Log an error if the GameObject named "[Save Custom Object]" is not found in the scene.
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - Save Custom Object not found!\n");
        }
    }
}