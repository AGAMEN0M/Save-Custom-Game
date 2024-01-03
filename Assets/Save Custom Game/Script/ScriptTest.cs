using UnityEngine;

public class ScriptTest : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode activateAndSave = KeyCode.Space;
    [SerializeField] private KeyCode disable = KeyCode.Escape;

    void LateUpdate()
    {
        if (Input.GetKeyDown(activateAndSave))
        {
            SaveDataUtility.EnableAutoSave();
            SaveDataUtility.SaveEvent();
        }

        if (Input.GetKeyDown(disable))
        {
            SaveDataUtility.DisableAutoSave();
        }
    }
}