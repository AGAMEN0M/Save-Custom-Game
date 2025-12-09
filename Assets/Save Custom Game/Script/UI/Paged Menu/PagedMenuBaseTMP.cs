/*
 * ---------------------------------------------------------------------------
 * Description: Base class for paged save/load menu systems using TMP and Unity UI.
 *              Consolidates shared UI wiring, page navigation, slot rendering, and cleanup.
 *              Child classes must implement the slot action and confirm action behavior.
 *
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;
using TMPro;

using static SaveCustomGame.SaveDataUtility;

namespace SaveCustomGame.PagedMenu
{
    public abstract class PagedMenuBaseTMP : MonoBehaviour
    {
        #region === Struct ===

        [Serializable]
        public struct SlotUI
        {
            [Tooltip("Button that triggers action on this slot.")]
            public Button button;

            [Tooltip("RawImage that displays the screenshot preview for this slot.")]
            public RawImage image;

            [Tooltip("Text field displaying the name and metadata of this save slot.")]
            public TMP_Text label;

            [Tooltip("Full save file path used when loading from a file-based save system.")]
            public string path;
        }

        #endregion

        #region === Inspector Fields ===

        [Header("Confirmation Panel Settings")]
        [SerializeField, Tooltip("UI panel used to confirm an overwrite or a load action.")]
        protected GameObject confirmationPanel;

        [SerializeField, Tooltip("Button that confirms the current action.")]
        protected Button confirmButton;

        [SerializeField, Tooltip("Button that cancels the current action and closes the confirmation panel.")]
        protected Button cancelButton;

        [Header("Save Limit Settings")]
        [SerializeField, Tooltip("Maximum number of save pages. Use -1 for unlimited pages.")]
        protected int maxSavePages = -1;

        [Header("Title Display")]
        [SerializeField, Tooltip("UI text displaying the current save/load mode title.")]
        protected TMP_Text titleLoad;

        [SerializeField, Tooltip("Label displayed for regular pages.")]
        protected string text = "Page";

        [SerializeField, Tooltip("Label displayed for the autosave page (page 0).")]
        protected string textAutomatic = "Autosave";

        [Header("Slot UI (1 - 6)")]
        [SerializeField, Tooltip("All slots displayed on the menu.")]
        protected SlotUI[] slots = new SlotUI[6];

        [Header("Page Navigation")]
        [SerializeField, Tooltip("Button used to advance to the next page.")]
        protected Button right;

        [SerializeField, Tooltip("Button used to return to the previous page.")]
        protected Button left;

        [SerializeField, Tooltip("Input field used to type the desired page number.")]
        protected TMP_InputField inputField;

        #endregion

        #region === Private / Protected Fields ===

        protected SaveCustomInScene saveCustomInScene;
        protected int currentNumber = 0;
        protected bool initializedUI = false;
        protected readonly string prefsKeyBase = "PagedMenu_Key";

        #endregion

        #region === Public Properties ===

        /// <summary>
        /// Gets or sets the confirmation panel used when the user must approve an action such as overwriting or loading a save file.
        /// </summary>
        public GameObject ConfirmationPanel
        {
            get => confirmationPanel;
            set => confirmationPanel = value;
        }

        /// <summary>
        /// Gets or sets the button responsible for confirming the current action displayed in the confirmation panel.
        /// </summary>
        public Button ConfirmButton
        {
            get => confirmButton;
            set => confirmButton = value;
        }

        /// <summary>
        /// Gets or sets the button responsible for canceling the current action and closing the confirmation panel.
        /// </summary>
        public Button CancelButton
        {
            get => cancelButton;
            set => cancelButton = value;
        }

        /// <summary>
        /// Gets or sets the maximum number of save pages available to the user. A value of -1 indicates unlimited pages.
        /// </summary>
        public int MaxSavePages
        {
            get => maxSavePages;
            set => maxSavePages = value;
        }

        /// <summary>
        /// Gets or sets the UI text component responsible for displaying the menu title.
        /// </summary>
        public TMP_Text TitleLoad
        {
            get => titleLoad;
            set => titleLoad = value;
        }

        /// <summary>
        /// Gets or sets the text displayed as the title when viewing a regular page.
        /// </summary>
        public string Text
        {
            get => text;
            set => text = value;
        }

        /// <summary>
        /// Gets or sets the text displayed as the title when viewing the automatic page (page zero).
        /// </summary>
        public string TextAutomatic
        {
            get => textAutomatic;
            set => textAutomatic = value;
        }

        /// <summary>
        /// Gets or sets the array of slot UI objects displayed within the paged save/load menu.
        /// </summary>
        public SlotUI[] Slots
        {
            get => slots;
            set => slots = value;
        }

        /// <summary>
        /// Gets or sets the button used to navigate forward through the save/load pages.
        /// </summary>
        public Button Right
        {
            get => right;
            set => right = value;
        }

        /// <summary>
        /// Gets or sets the button used to navigate backward through the save/load pages.
        /// </summary>
        public Button Left
        {
            get => left;
            set => left = value;
        }

        /// <summary>
        /// Gets or sets the input field where the user can manually type a page number.
        /// </summary>
        public TMP_InputField InputField
        {
            get => inputField;
            set => inputField = value;
        }

        #endregion

        #region === Unity Events ===

        /// <summary>
        /// Called when the menu becomes active. Initializes UI state and loads slot previews.
        /// </summary>
        protected virtual void OnEnable()
        {
            saveCustomInScene = GetComponentSaveCustomInScene();

            // Restore last page index if stored.
            if (PlayerPrefs.HasKey(prefsKeyBase)) currentNumber = PlayerPrefs.GetInt(prefsKeyBase);

            SetupUI();
            SetupInitialPage();
            UpdateTitle();
        }

        /// <summary>
        /// Called when the menu is disabled. Ensures textures are cleaned to avoid memory leaks.
        /// </summary>
        protected virtual void OnDisable() => ClearSlotTextures();

        #endregion

        #region === Setup UI ===

        /// <summary>
        /// Sets up UI listeners and binds slot click events. Runs once per lifetime.
        /// </summary>
        protected virtual void SetupUI()
        {
            if (initializedUI) return;

            if (right != null) right.onClick.AddListener(IncreasePage);
            if (left != null) left.onClick.AddListener(DecreasePage);

            if (inputField != null)
            {
                inputField.onValidateInput += ValidateNumericInput;
                inputField.onEndEdit.AddListener(OnEndEditInputField);
            }

            if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelPressed);

            // Bind slot buttons to generic slot click handler.
            for (int i = 0; i < slots.Length; i++)
            {
                int index = i + 1;
                if (slots[i].button != null)
                    slots[i].button.onClick.AddListener(() => OnSlotPressed(index));
            }

            initializedUI = true;
        }

        /// <summary>
        /// Initializes the input field value and refreshes the visible slots.
        /// </summary>
        protected virtual void SetupInitialPage()
        {
            if (inputField != null) inputField.text = currentNumber.ToString();
            RefreshSlots();
        }

        #endregion

        #region === Title ===

        /// <summary>
        /// Updates the displayed title depending on whether viewing the autosave page or normal pages.
        /// </summary>
        public virtual void UpdateTitle() => titleLoad.text = currentNumber == 0 ? textAutomatic : text;

        #endregion

        #region === Page Navigation ===

        /// <summary>
        /// Moves to the next page and refreshes the UI, respecting the optional page limit.
        /// </summary>
        public virtual void IncreasePage()
        {
            int.TryParse(inputField.text, out currentNumber);
            if (maxSavePages >= 0 && currentNumber >= maxSavePages) return;
            currentNumber++;
            inputField.text = currentNumber.ToString();
            RefreshSlots();
        }

        /// <summary>
        /// Moves to the previous page and refreshes the UI.
        /// </summary>
        public virtual void DecreasePage()
        {
            if (currentNumber <= 0) return;

            int.TryParse(inputField.text, out currentNumber);
            currentNumber--;
            inputField.text = currentNumber.ToString();
            RefreshSlots();
        }

        /// <summary>
        /// Validate numeric input for the page input field.
        /// </summary>
        protected virtual char ValidateNumericInput(string text, int index, char c) => char.IsDigit(c) ? c : '\0';

        /// <summary>
        /// Called when the user finishes editing the page field manually.
        /// Ensures the page number stays within allowed limits and refreshes the UI.
        /// </summary>
        public virtual void OnEndEditInputField(string value)
        {
            int.TryParse(value, out currentNumber);
            if (currentNumber < 0) currentNumber = 0;
            if (maxSavePages >= 0 && currentNumber > maxSavePages) currentNumber = maxSavePages;

            if (inputField != null) inputField.text = currentNumber.ToString();
            RefreshSlots();
        }

        #endregion

        #region === Slot UI Refresh ===

        /// <summary>
        /// Refreshes labels, interactivity and reloads slot data for the current page.
        /// </summary>
        protected virtual void RefreshSlots()
        {
            // Enforce limits.
            if (maxSavePages >= 0 && currentNumber > maxSavePages) currentNumber = maxSavePages;
            if (currentNumber < 0) currentNumber = 0;

            PlayerPrefs.SetInt(prefsKeyBase, currentNumber);

            if (left != null) left.interactable = currentNumber > 0;
            if (right != null) right.interactable = maxSavePages < 0 || currentNumber < maxSavePages;

            // Update labels.
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].label != null)
                    slots[i].label.text = $"{(inputField != null ? inputField.text : currentNumber.ToString())} - {i + 1}";
            }

            // Clear previous textures and reload data.
            ClearSlotTextures();
            LoadSlotsData();
            UpdateTitle();
        }

        /// <summary>
        /// Loads slot data for the currently displayed page. Must be implemented by child classes.
        /// </summary>
        protected abstract void LoadSlotsData();

        #endregion

        #region === Slot Action / Confirmation ===

        /// <summary>
        /// Generic slot pressed handler that routes to child behavior.
        /// </summary>
        protected void OnSlotPressed(int slot)
        {
            // If slot has no image or button is null or not interactable, ignore.
            if (slots == null || slot - 1 < 0 || slot - 1 >= slots.Length) return;
            var s = slots[slot - 1];
            if (s.button == null || !s.button.interactable) return;

            // Delegate to concrete class.
            HandleSlotAction(slot);
        }

        /// <summary>
        /// Called by the base when cancel button or cancel flow is requested.
        /// Child classes may override if they need different cancel behavior.
        /// </summary>
        protected virtual void OnCancelPressed()
        {
            if (confirmationPanel != null) confirmationPanel.SetActive(false);
        }

        /// <summary>
        /// Child classes implement what happens when a slot is pressed.
        /// Typically, this will either open a confirmation or perform the action directly.
        /// </summary>
        /// <param name="slot">Slot number pressed (1-based).</param>
        protected abstract void HandleSlotAction(int slot);

        /// <summary>
        /// Child classes implement what happens after the user confirms the action.
        /// </summary>
        /// <param name="slot">Slot number (1-based).</param>
        protected abstract void ConfirmAction(int slot);

        /// <summary>
        /// Helper used by children to show confirmation for a slot and wire the confirm button.
        /// </summary>
        /// <param name="slot">Slot number (1-based) to confirm.</param>
        protected void ShowConfirmationForSlot(int slot)
        {
            if (confirmationPanel == null || confirmButton == null) return;

            confirmationPanel.SetActive(true);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => ConfirmAction(slot));
        }

        #endregion

        #region === Slot Helpers ===

        /// <summary>
        /// Sets the visual and interactive state of the slot.
        /// </summary>
        /// <param name="index">Zero-based slot index.</param>
        /// <param name="active">Whether the slot should be interactive and visible.</param>
        protected virtual void SetSlotActive(int index, bool active)
        {
            if (slots == null || index < 0 || index >= slots.Length) return;
            var slot = slots[index];
            if (slot.button != null) slot.button.interactable = active;
            if (slot.image != null) slot.image.color = active ? Color.white : new Color(1, 1, 1, 0);
        }

        /// <summary>
        /// Utility to determine save/load path for a given file name.
        /// This follows PlayerPrefs mode and file mode logic from original system.
        /// </summary>
        /// <param name="fileName">Filename without extension.</param>
        /// <returns>Full path or empty string if PlayerPrefs mode is used.</returns>
        protected virtual string GetSavePath(string fileName)
        {
            if (saveCustomInScene == null || saveCustomInScene.saveCustomObject == null) return "";

            if (saveCustomInScene.saveCustomObject.playerPrefs) return "";

            string file = $"{fileName}.json";

            if (saveCustomInScene.saveCustomObject.localLow)
                return Path.Combine(Application.persistentDataPath, "saves", file);

        #if UNITY_EDITOR
            return Path.Combine(Application.dataPath, "Editor/saves", file);
        #else
            return Path.Combine(Application.dataPath, "saves", file);
        #endif
        }

        /// <summary>
        /// Applies loaded data into the slot UI. Child classes may call this helper.
        /// </summary>
        /// <param name="data">Parsed SaveCustomFile data instance.</param>
        /// <param name="index">Zero-based slot index.</param>
        protected virtual void ApplySlotData(SaveCustomFile data, int index)
        {
            if (slots == null || index < 0 || index >= slots.Length) return;

            if (slots[index].label != null)
                slots[index].label.text = $"{(inputField != null ? inputField.text : currentNumber.ToString())} - {index + 1} ({data.gameTime})";

            if (slots[index].image != null)
                slots[index].image.texture = RenderScreenshot(data.screenshot);

            SetSlotActive(index, true);
        }

        #endregion

        #region === Cleanup ===

        /// <summary>
        /// Clears textures assigned to the slots to avoid memory leaks.
        /// </summary>
        public virtual void ClearSlotTextures()
        {
            if (slots == null) return;

            foreach (var slot in slots)
            {
                if (slot.image != null && slot.image.texture != null)
                {
                    Destroy(slot.image.texture);
                    slot.image.texture = null;
                }
            }
        }

        #endregion
    }
}