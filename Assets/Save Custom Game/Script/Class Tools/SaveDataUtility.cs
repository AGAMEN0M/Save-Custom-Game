using System.Collections.Generic;
using UnityEngine;

public static class SaveDataUtility
{
    // Retrieve a float value from SaveCustomObject based on item and float tags.
    public static float GetFloat(SaveCustomObject saveObject, string itemTag, string floatTag)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom float within the current item.
                foreach (var customFloat in customItem.itemFloat)
                {
                    // Check if the current float's tag matches the provided floatTag.
                    if (customFloat.floatTag == floatTag)
                    {
                        return customFloat.floatValue; // Return the value of the found floatTag.
                    }
                }
                // Throw an exception if the floatTag is not found within the item.
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Float tag '{floatTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Retrieve an integer value from SaveCustomObject based on item and int tags.
    public static int GetInt(SaveCustomObject saveObject, string itemTag, string intTag)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom int within the current item.
                foreach (var customInt in customItem.itemInt)
                {
                    // Check if the current int's tag matches the provided intTag.
                    if (customInt.intTag == intTag)
                    {
                        return (int)customInt.intValue; // Return the value of the found intTag (converted to int).
                    }
                }
                // Throw an exception if the intTag is not found within the item.
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Int tag '{intTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Retrieve a string value from SaveCustomObject based on item and string tags.
    public static string GetString(SaveCustomObject saveObject, string itemTag, string stringTag)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom string within the current item.
                foreach (var customString in customItem.itemString)
                {
                    // Check if the current string's tag matches the provided stringTag.
                    if (customString.stringTag == stringTag)
                    {
                        return customString.stringValue; // Return the value of the found stringTag.
                    }
                }
                // Throw an exception if the stringTag is not found within the item.
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - String tag '{stringTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Retrieve a boolean value from SaveCustomObject based on item and bool tags.
    public static bool GetBool(SaveCustomObject saveObject, string itemTag, string boolTag)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom bool within the current item.
                foreach (var customBool in customItem.itemBool)
                {
                    // Check if the current bool's tag matches the provided boolTag.
                    if (customBool.boolTag == boolTag)
                    {
                        return customBool.boolValue; // Return the value of the found boolTag.
                    }
                }
                // Throw an exception if the boolTag is not found within the item.
                throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Bool tag '{boolTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"{ExceptionUtility.GetCallingMethodInfo()} - Item tag '{itemTag}' not found\n");
    }

    // Set a float value in SaveCustomObject based on item and float tags.
    public static void SetFloat(SaveCustomObject saveObject, string itemTag, string floatTag, float newValue)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom float within the current item.
                foreach (var customFloat in customItem.itemFloat)
                {
                    // Check if the current float's tag matches the provided floatTag.
                    if (customFloat.floatTag == floatTag)
                    {
                        // Set the value of the found floatTag to the newValue.
                        customFloat.floatValue = newValue;
                        return; // Exit the method after setting the value.
                    }
                }
                // Throw an exception if the floatTag is not found within the item.
                throw new KeyNotFoundException($"Float tag '{floatTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"Item tag '{itemTag}' not found\n");
    }

    // Set an integer value in SaveCustomObject based on item and int tags.
    public static void SetInt(SaveCustomObject saveObject, string itemTag, string intTag, int newValue)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom int within the current item.
                foreach (var customInt in customItem.itemInt)
                {
                    // Check if the current int's tag matches the provided intTag.
                    if (customInt.intTag == intTag)
                    {
                        // Set the value of the found intTag to the newValue.
                        customInt.intValue = newValue;
                        return; // Exit the method after setting the value.
                    }
                }
                // Throw an exception if the intTag is not found within the item.
                throw new KeyNotFoundException($"Int tag '{intTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"Item tag '{itemTag}' not found\n");
    }

    // Set a string value in SaveCustomObject based on item and string tags.
    public static void SetString(SaveCustomObject saveObject, string itemTag, string stringTag, string newValue)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom string within the current item.
                foreach (var customString in customItem.itemString)
                {
                    // Check if the current string's tag matches the provided stringTag.
                    if (customString.stringTag == stringTag)
                    {
                        // Set the value of the found stringTag to the newValue.
                        customString.stringValue = newValue;
                        return; // Exit the method after setting the value.
                    }
                }
                // Throw an exception if the stringTag is not found within the item.
                throw new KeyNotFoundException($"String tag '{stringTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"Item tag '{itemTag}' not found\n");
    }

    // Set a boolean value in SaveCustomObject based on item and bool tags.
    public static void SetBool(SaveCustomObject saveObject, string itemTag, string boolTag, bool newValue)
    {
        // Iterate through each custom item in the SaveCustomObject.
        foreach (var customItem in saveObject.saveCustomItems)
        {
            // Check if the current item's tag matches the provided itemTag.
            if (customItem.itemTag == itemTag)
            {
                // Iterate through each custom bool within the current item.
                foreach (var customBool in customItem.itemBool)
                {
                    // Check if the current bool's tag matches the provided boolTag.
                    if (customBool.boolTag == boolTag)
                    {
                        // Set the value of the found boolTag to the newValue.
                        customBool.boolValue = newValue;
                        return; // Exit the method after setting the value.
                    }
                }
                // Throw an exception if the boolTag is not found within the item.
                throw new KeyNotFoundException($"Bool tag '{boolTag}' not found in item '{itemTag}'\n");
            }
        }
        // Throw an exception if the itemTag is not found in the SaveCustomObject.
        throw new KeyNotFoundException($"Item tag '{itemTag}' not found\n");
    }

    // Capture a screenshot using a target camera and assign it to SaveCustomObject.
    public static void CaptureScreenshot(SaveCustomObject saveObject, Camera targetCamera)
    {
        // Check if either SaveCustomObject or targetCamera is not defined.
        if (saveObject == null || targetCamera == null)
        {
            // Log an error if SaveCustomObject or targetCamera is not defined.
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomObject or Camera are not defined!\n");
            return; // Exit the method if either parameter is null.
        }

        // Get the width and height of the screen.
        int width = Screen.width;
        int height = Screen.height;

        RenderTexture renderTexture = new(width, height, 24); // Create a new render texture with the screen dimensions.
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
    public static void GetComponentSaveCustomInScene(ref SaveCustomInScene saveCustomInScene)
    {
        GameObject saveCustomObject = GameObject.Find("[Save Custom Object]"); // Find the GameObject named "[Save Custom Object]" in the scene.

        // Check if the GameObject is found in the scene.
        if (saveCustomObject != null)
        {
            // Attempt to get the SaveCustomInScene component attached to the GameObject.
            if (saveCustomObject.TryGetComponent(out saveCustomInScene))
            {
                // Log a success message if the component is assigned successfully.
                Debug.Log($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomInScene script has been assigned successfully!\n");
            }
            else
            {
                // Log an error if the SaveCustomInScene component is not found in the GameObject.
                Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - SaveCustomInScene not found in Save Custom Object!\n");
            }
        }
        else
        {
            // Log an error if the GameObject named "[Save Custom Object]" is not found in the scene.
            Debug.LogError($"{ExceptionUtility.GetCallingMethodInfo()} - Save Custom Object not found!\n");
        }
    }

    // Enable auto-saving by setting the autosaveEnabled flag to true in SaveCustomObject.
    public static void EnableAutoSave()
    {
        SaveCustomObject saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data"); // Load the SaveCustomObject from Resources.
        saveCustomObject.autosaveEnabled = true; // Set the autosaveEnabled flag to true.
    }

    public static void DisableAutoSave() 
    {
        SaveCustomObject saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data"); // Load the SaveCustomObject from Resources.
        saveCustomObject.autosaveEnabled = false; // Set the autosaveEnabled flag to false.
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