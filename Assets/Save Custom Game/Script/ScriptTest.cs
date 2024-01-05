using UnityEngine;

public class ScriptTest : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode activateAndSave = KeyCode.Space; // Key to activate and trigger auto-save.
    [SerializeField] private KeyCode disable = KeyCode.Escape; // Key to disable auto-save.
    [Space(10)]
    [SerializeField] private GameObject[] gameObjects; // Array of GameObjects to toggle.

    private void LateUpdate()
    {
        // Check if the key for activation and save is pressed.
        if (Input.GetKeyDown(activateAndSave))
        {
            // Enable auto-save and trigger save event.
            SaveDataUtility.EnableAutoSave();
            SaveDataUtility.SaveEvent();
        }

        // Check if the key for disabling auto-save is pressed.
        if (Input.GetKeyDown(disable))
        {
            SaveDataUtility.DisableAutoSave(); // Disable auto-save.
        }
    }

    public void SwitchGameObject()
    {
        // Loop through each GameObject in the gameObjects array.
        foreach (GameObject obj in gameObjects)
        {            
            obj.SetActive(!obj.activeSelf); // Invert the current activation state of the GameObject.
        }
    }
}