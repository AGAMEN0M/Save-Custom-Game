/*
 * ---------------------------------------------------------------------------
 * Description: Concrete save menu implementation using PagedMenuBaseTMP.
 *              Handles saving into slots, confirmation for overwrites, and slot rendering.
 *
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;
using System.IO;

namespace SaveCustomGame.PagedMenu
{
    [AddComponentMenu("Tools/Save Custom Game/UI/Paged Menu/Save Menu Manager (TMP)")]
    public class SaveMenuManagerTMP : PagedMenuBaseTMP
    {
        #region === Unity Events ===

        /// <summary>
        /// Called when enabled. Ensures base logic runs.
        /// </summary>
        protected override void OnEnable() => base.OnEnable();

        #endregion

        #region === Slot Loading Implementation ===

        /// <summary>
        /// Loads save slot data for the current page and populates the UI.
        /// </summary>
        protected override void LoadSlotsData()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                int slotNumber = i + 1;
                string fileName = $"{(inputField != null ? inputField.text : currentNumber.ToString())} - {slotNumber}";
                string path = GetSavePath(fileName);
                slots[i].path = path;

                if (saveCustomInScene.saveCustomObject.playerPrefs)
                {
                    if (!PlayerPrefs.HasKey(fileName))
                    {
                        SetSlotActive(i, true);
                        continue;
                    }

                    var data = JsonUtility.FromJson<SaveCustomFile>(PlayerPrefs.GetString(fileName));
                    ApplySlotData(data, i);
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        SetSlotActive(i, true);
                        continue;
                    }

                    var data = JsonUtility.FromJson<SaveCustomFile>(File.ReadAllText(path));
                    ApplySlotData(data, i);
                }
            }
        }

        #endregion

        #region === Slot Action ===

        /// <summary>
        /// Handles slot press for saving. Shows confirmation if slot occupied or saves directly if empty.
        /// </summary>
        /// <param name="slot">Slot number pressed (1-based).</param>
        protected override void HandleSlotAction(int slot)
        {
            // If target slot has a texture -> occupied.
            bool occupied = (slots[slot - 1].image != null && slots[slot - 1].image.texture != null);

            if (occupied)
            {
                // Ask for confirmation to overwrite.
                ShowConfirmationForSlot(slot);
            }
            else
            {
                // Save immediately.
                PerformSave(slot);
            }
        }

        /// <summary>
        /// In save mode all slots must remain active. Image should be visible only when a texture exists.
        /// </summary>
        protected override void SetSlotActive(int index, bool active)
        {
            if (slots == null || index < 0 || index >= slots.Length) return;
            var slot = slots[index];

            // Button always interactable when saving.
            if (slot.button != null) slot.button.interactable = true;

            // If there is a texture, show it. If not, transparent.
            if (slot.image != null)
            {
                bool hasTexture = slot.image.texture != null;
                slot.image.color = hasTexture ? Color.white : new Color(1, 1, 1, 0);
            }
        }

        /// <summary>
        /// Performs the actual save operation to the given slot.
        /// </summary>
        /// <param name="slot">Slot number (1-based).</param>
        private void PerformSave(int slot)
        {
            saveCustomInScene.fileName = $"{(inputField != null ? inputField.text : currentNumber.ToString())} - {slot}";
            saveCustomInScene.SaveData();

            // Refresh to display the new save.
            ClearSlotTextures();
            LoadSlotsData();
        }

        /// <summary>
        /// Called when the confirm button is pressed after an overwrite confirmation.
        /// </summary>
        /// <param name="slot">Slot number (1-based).</param>
        protected override void ConfirmAction(int slot)
        {
            PerformSave(slot);

            if (confirmationPanel != null) confirmationPanel.SetActive(false);
        }

        #endregion
    }
}