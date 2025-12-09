/*
 * ---------------------------------------------------------------------------
 * Description: Concrete load menu implementation using PagedMenuBaseUI.
 *              Handles loading from slots, confirmation before applying load, and slot rendering.
 *
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine;
using System.IO;

namespace SaveCustomGame.PagedMenu
{
    [AddComponentMenu("UI/Save Custom Game/Paged Menu/Load Menu Manager (Legacy)")]
    public class LoadMenuManager : PagedMenuBase
    {
        #region === Unity Events ===

        /// <summary>
        /// Called when enabled. Ensures base logic runs.
        /// </summary>
        protected override void OnEnable() => base.OnEnable();

        #endregion

        #region === Slot Loading Implementation ===

        /// <summary>
        /// Loads load-slot data for the current page and populates the UI.
        /// </summary>
        protected override void LoadSlotsData()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                string fileName = $"{(inputField != null ? inputField.text : currentNumber.ToString())} - {i + 1}";
                string path = GetSavePath(fileName);
                slots[i].path = path;

                if (saveCustomInScene.saveCustomObject.playerPrefs)
                {
                    if (!PlayerPrefs.HasKey(fileName))
                    {
                        SetSlotActive(i, false);
                        continue;
                    }

                    var data = JsonUtility.FromJson<SaveCustomFile>(PlayerPrefs.GetString(fileName));
                    ApplySlotData(data, i);
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        SetSlotActive(i, false);
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
        /// Handles slot press for loading. Shows confirmation panel if slot has data.
        /// </summary>
        /// <param name="slot">Slot number pressed (1-based).</param>
        protected override void HandleSlotAction(int slot)
        {
            // If the slot has no texture or is inactive, ignore.
            if (slots[slot - 1].image == null || slots[slot - 1].image.texture == null) return;

            // Ask for confirmation before loading.
            ShowConfirmationForSlot(slot);
        }

        /// <summary>
        /// Called when the confirm button is pressed for a load action.
        /// </summary>
        /// <param name="slot">Slot number (1-based).</param>
        protected override void ConfirmAction(int slot)
        {
            saveCustomInScene.fileName = $"{(inputField != null ? inputField.text : currentNumber.ToString())} - {slot}";
            saveCustomInScene.savePath = slots[slot - 1].path;
            saveCustomInScene.LoadData();

            if (confirmationPanel != null) confirmationPanel.SetActive(false);
        }

        #endregion
    }
}