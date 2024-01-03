using UnityEngine;

public class SaveCustomInitialization
{
    public static SaveCustomObject saveCustomObject; // Static reference to the SaveCustomObject.

    // Method to be executed on runtime initialization.
    [RuntimeInitializeOnLoadMethod]
    public static void RunGameInitialization()
    {
        Debug.Log("Save Custom Initialization"); // Log initialization message.
        LoadSettingsData(); // Load Save Custom Object Data from Resources.

        // Check if SaveCustomObject is loaded.
        if (saveCustomObject == null)
        {
            // Log an error if SaveCustomObject is not found.
            Debug.LogError("Could not find saveCustomObject with name 'Save Custom Object Data'");
            return;
        }

        GameObject saveCustomGameObject = new("[Save Custom Object]"); // Create a new GameObject named "[Save Custom Object]".

        // Add SaveCustomInScene and AutoSaveCustom components to the GameObject.
        SaveCustomInScene saveCustomInScene = saveCustomGameObject.AddComponent<SaveCustomInScene>();
        AutoSaveCustom autoSaveCustom = saveCustomGameObject.AddComponent<AutoSaveCustom>();

        // Assign references between components and objects.
        saveCustomInScene.saveCustomObject = saveCustomObject;
        autoSaveCustom.saveCustomInScene = saveCustomInScene;

        Object.DontDestroyOnLoad(saveCustomGameObject); // Ensure the GameObject persists across scene changes.
    }

    // Method to load Save Custom Object Data from Resources.
    private static void LoadSettingsData()
    {
        saveCustomObject = Resources.Load<SaveCustomObject>("Save Custom Object Data");

        // Log an error if Save Custom Object Data fails to load.
        if (saveCustomObject == null) { Debug.LogError("Failed to load Save Custom Object Data from Resources."); }
    }
}