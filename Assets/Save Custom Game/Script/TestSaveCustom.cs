/*
 * ---------------------------------------------------------------------------
 * Description: Handles runtime auto-save control and testing utilities.
 *              Designed to be triggered via UI buttons or external systems,
 *              removing dependency on keyboard input. Provides methods for
 *              enabling/disabling auto-save, triggering saves, and toggling
 *              GameObjects for debugging and testing purposes.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;

using static SaveCustomGame.SaveDataUtility;

namespace SaveCustomGame
{
    [AddComponentMenu("Tools/Save Custom Game/In Background/Test Save Custom")]
    public class TestSaveCustom : MonoBehaviour
    {
        #region === Inspector Fields ===

        [Header("Test Objects")]
        [SerializeField, Tooltip("Array of GameObjects to toggle active state for testing.")]
        private GameObject[] gameObjects;

        [Header("Optional Save Slot")]
        [SerializeField, Tooltip("Optional: Specific save slot to test saving.")]
        private int testSaveSlot = 1;

        #endregion

        #region === Public Methods (UI / Events) ===

        /// <summary>
        /// Enables auto-save and immediately triggers a save operation.
        /// Intended to be called from UI buttons or external systems.
        /// </summary>
        public void ActivateAndSave()
        {
            SetAutoSave(true);
            SaveTestSlot();
        }

        /// <summary>
        /// Disables the auto-save system.
        /// Intended to be called from UI buttons or external systems.
        /// </summary>
        public void DisableAutoSave()
        {
            SetAutoSave(false);
            Debug.Log($"[{nameof(TestSaveCustom)}] Auto-save disabled.", this);
        }

        /// <summary>
        /// Triggers a save operation manually.
        /// Uses the optional test slot if defined.
        /// </summary>
        public void TriggerSave() => SaveTestSlot();

        #endregion

        #region === Save Testing ===

        /// <summary>
        /// Performs a save event and logs contextual information.
        /// If a test slot is defined, logs the slot index.
        /// </summary>
        private void SaveTestSlot()
        {
            SaveEvent();

            if (testSaveSlot > 0)
            {
                Debug.Log($"[{nameof(TestSaveCustom)}] Save triggered in slot {testSaveSlot}.", this);
            }
            else
            {
                Debug.Log($"[{nameof(TestSaveCustom)}] Save triggered (no specific slot).", this);
            }
        }

        #endregion

        #region === GameObject Toggle ===

        /// <summary>
        /// Toggles the active state of all assigned GameObjects.
        /// Useful for testing runtime changes and persistence behavior.
        /// </summary>
        public void SwitchGameObject()
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                Debug.LogWarning($"[{nameof(TestSaveCustom)}] No GameObjects assigned.", this);
                return;
            }

            foreach (var obj in gameObjects)
            {
                if (obj == null) continue;
                obj.SetActive(!obj.activeSelf);
            }

            Debug.Log($"[{nameof(TestSaveCustom)}] Toggled GameObjects state.", this);
        }

        #endregion
    }
}