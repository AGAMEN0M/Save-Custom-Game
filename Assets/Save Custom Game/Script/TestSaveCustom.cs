/*
 * ---------------------------------------------------------------------------
 * Description: This script manages auto-save functionality and allows toggling the 
 *              active state of specific GameObjects. It enables auto-save and triggers 
 *              a save event when a specified key is pressed and disables auto-save with 
 *              another key. Additionally, it provides a method to toggle the activation 
 *              state of GameObjects in a predefined array.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using SaveCustomGame;
using UnityEngine;

[AddComponentMenu("UI/Save Custom Game/In Background/Test Save Custom")]
public class TestSaveCustom : MonoBehaviour
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