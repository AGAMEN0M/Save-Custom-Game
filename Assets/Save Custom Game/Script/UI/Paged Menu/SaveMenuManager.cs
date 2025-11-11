/*
 * ---------------------------------------------------------------------------
 * Description: Manages the in-game save menu UI for creating and managing saved
 *              game states. Handles dynamic slot labeling, page navigation,
 *              and confirmation dialogs for overwriting saves. Supports both
 *              PlayerPrefs and external file saving modes, automatically
 *              detecting existing save data and rendering associated screenshots
 *              and metadata per slot. Enables flexible saving to manual and
 *              autosave slots across multiple pages.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using SaveCustomGame;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;

using static SaveCustomGame.SaveDataUtility;

namespace SaveCustomGame.PagedMenu
{
    [AddComponentMenu("UI/Save Custom Game/Paged Menu/Save Menu Manager (Legacy)")]
    public class SaveMenuManager : MonoBehaviour
    {
        #region === Structs ===

        /// <summary>
        /// Represents a group of UI components associated with a single save slot.
        /// Used to display screenshot preview, save name, and respond to player input.
        /// </summary>
        [Serializable]
        public struct SlotUI
        {
            [Tooltip("Button that triggers saving to this slot.")]
            public Button button;

            [Tooltip("RawImage that displays the screenshot preview for this save slot.")]
            public RawImage image;

            [Tooltip("Text field displaying the name and metadata of this save slot.")]
            public Text label;
        }

        #endregion

        #region === Inspector Fields ===

        [Header("Confirmation Panel Settings")]
        [SerializeField, Tooltip("UI panel displayed to confirm overwriting an existing save.")]
        private GameObject confirmationPanel;

        [SerializeField, Tooltip("Button that confirms the overwrite action.")]
        private Button confirmButton;

        [SerializeField, Tooltip("Button that cancels the save confirmation.")]
        private Button cancelButton;

        [Header("Title Display")]
        [SerializeField, Tooltip("UI text displaying the current save mode title.")]
        private Text titleLoad;

        [SerializeField, Tooltip("Label displayed for regular save slots.")]
        private string text = "Save";

        [SerializeField, Tooltip("Label displayed for the autosave slot (page 0).")]
        private string textAutomatic = "Autosave";

        [Header("Save Slot UI (1 - 6)")]
        [SerializeField, Tooltip("All save slots displayed on the menu.")]
        private SlotUI[] slots = new SlotUI[6];

        [Header("Page Navigation")]
        [SerializeField, Tooltip("Button used to advance to the next save page.")]
        private Button right;

        [SerializeField, Tooltip("Button used to return to the previous save page.")]
        private Button left;

        [SerializeField, Tooltip("Input field used to type the desired save page number.")]
        private InputField inputField;

        #endregion

        #region === Private Fields ===

        private SaveCustomInScene saveCustomInScene; // Reference to the Save system component.
        private int currentSaveNumber = 0; // Current selected save page.
        private bool initializedUI = false; // Ensures listeners are added only once.
        private readonly string saveKey = "SaveMenuManager"; // PlayerPrefs key for current page.

        #endregion

        #region === Public Properties ===

        /// <summary>
        /// Reference to the confirmation panel used during save selection.
        /// </summary>
        public GameObject ConfirmationPanel
        {
            get => confirmationPanel;
            set => confirmationPanel = value;
        }

        /// <summary>
        /// Reference to the button that confirms an overwrite action.
        /// </summary>
        public Button ConfirmButton
        {
            get => confirmButton;
            set => confirmButton = value;
        }

        /// <summary>
        /// Reference to the button that cancels the save confirmation dialog.
        /// </summary>
        public Button CancelButton
        {
            get => cancelButton;
            set => cancelButton = value;
        }

        /// <summary>
        /// Reference to the UI text displaying the current title.
        /// </summary>
        public Text TitleLoad
        {
            get => titleLoad;
            set => titleLoad = value;
        }

        /// <summary>
        /// Displayed title for manual save pages.
        /// </summary>
        public string Text
        {
            get => text;
            set => text = value;
        }

        /// <summary>
        /// Displayed title for the autosave page.
        /// </summary>
        public string TextAutomatic
        {
            get => textAutomatic;
            set => textAutomatic = value;
        }

        /// <summary>
        /// Exposes UI slot references for external inspection or modification.
        /// </summary>
        public SlotUI[] Slots
        {
            get => slots;
            set => slots = value;
        }

        /// <summary>
        /// Navigation button for advancing to the next page of save slots.
        /// </summary>
        public Button Right
        {
            get => right;
            set => right = value;
        }

        /// <summary>
        /// Navigation button for moving to the previous page of save slots.
        /// </summary>
        public Button Left
        {
            get => left;
            set => left = value;
        }

        /// <summary>
        /// Input field used to enter the current save page number.
        /// </summary>
        public InputField InputField
        {
            get => inputField;
            set => inputField = value;
        }

        #endregion

        #region === Unity Events ===

        /// <summary>
        /// Called when the save menu becomes active. Initializes UI, restores page state,
        /// and refreshes slot display.
        /// </summary>
        private void OnEnable()
        {
            saveCustomInScene = GetComponentSaveCustomInScene();

            if (PlayerPrefs.HasKey(saveKey)) currentSaveNumber = PlayerPrefs.GetInt(saveKey);

            SetupUI();
            SetupInitialPage();
            SetTitle();
        }

        /// <summary>
        /// Called when the menu is disabled. Clears slot textures to free memory.
        /// </summary>
        private void OnDisable() => ClearSlotTextures();

        #endregion

        #region === Title ===

        /// <summary>
        /// Updates the displayed title depending on whether viewing autosave or manual save.
        /// </summary>
        public void SetTitle() => titleLoad.text = currentSaveNumber == 0 ? textAutomatic : text;

        #endregion

        #region === Setup UI ===

        /// <summary>
        /// Adds UI listeners for navigation, input, and buttons. Ensures initialization only occurs once.
        /// </summary>
        private void SetupUI()
        {
            if (initializedUI) return;

            right.onClick.AddListener(IncreasePage);
            left.onClick.AddListener(DecreasePage);

            inputField.onValidateInput += ValidateNumericInput;
            inputField.onEndEdit.AddListener(OnInputPageChanged);

            cancelButton.onClick.AddListener(CancelSave);

            for (int i = 0; i < slots.Length; i++)
            {
                int index = i + 1;
                slots[i].button.onClick.AddListener(() => TrySaveToSlot(index));
            }

            initializedUI = true;
        }

        /// <summary>
        /// Initializes the input field and refreshes the slot display.
        /// </summary>
        private void SetupInitialPage()
        {
            inputField.text = currentSaveNumber.ToString();
            RefreshSlotDisplay();
        }

        #endregion

        #region === Page Navigation ===

        /// <summary>
        /// Moves to the next save page and refreshes the UI.
        /// </summary>
        public void IncreasePage()
        {
            int.TryParse(inputField.text, out currentSaveNumber);
            currentSaveNumber++;
            inputField.text = currentSaveNumber.ToString();
            RefreshSlotDisplay();
        }

        /// <summary>
        /// Moves to the previous save page and refreshes the UI.
        /// </summary>
        public void DecreasePage()
        {
            if (currentSaveNumber <= 0) return;

            int.TryParse(inputField.text, out currentSaveNumber);
            currentSaveNumber--;
            inputField.text = currentSaveNumber.ToString();
            RefreshSlotDisplay();
        }

        /// <summary>
        /// Validates numeric input for the page input field.
        /// </summary>
        private char ValidateNumericInput(string text, int index, char c) => char.IsDigit(c) ? c : '\0';

        /// <summary>
        /// Updates the current page when the input field editing ends.
        /// </summary>
        public void OnInputPageChanged(string value)
        {
            int.TryParse(value, out currentSaveNumber);
            RefreshSlotDisplay();
        }

        #endregion

        #region === Slot UI Refresh ===

        /// <summary>
        /// Refreshes slot labels, updates navigation buttons, and loads slot previews.
        /// </summary>
        private void RefreshSlotDisplay()
        {
            PlayerPrefs.SetInt(saveKey, currentSaveNumber);
            left.interactable = currentSaveNumber > 0;

            for (int i = 0; i < slots.Length; i++)
                slots[i].label.text = $"{inputField.text} - {i + 1}";

            ClearSlotTextures();
            LoadSlotData();
            SetTitle();
        }

        #endregion

        #region === Save Process ===

        /// <summary>
        /// Attempts to save to the given slot. If occupied, displays confirmation panel.
        /// </summary>
        public void TrySaveToSlot(int slot)
        {
            if (slots[slot - 1].image.texture != null)
            {
                // Slot occupied.
                confirmationPanel.SetActive(true);
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() => ConfirmSave(slot));
            }
            else
            {
                // Slot empty.
                PerformSave(slot);
            }
        }

        /// <summary>
        /// Performs the save operation immediately.
        /// </summary>
        private void PerformSave(int slot)
        {
            saveCustomInScene.fileName = $"{inputField.text} - {slot}";
            saveCustomInScene.SaveData();

            ClearSlotTextures();
            LoadSlotData();
        }

        /// <summary>
        /// Confirms save and overwrites the existing slot.
        /// </summary>
        public void ConfirmSave(int slot)
        {
            PerformSave(slot);
            confirmationPanel.SetActive(false);
        }

        /// <summary>
        /// Cancels save operation and closes the confirmation panel.
        /// </summary>
        public void CancelSave() => confirmationPanel.SetActive(false);

        #endregion

        #region === Load and Display Data ===

        /// <summary>
        /// Loads all slot data and applies UI updates.
        /// </summary>
        public void LoadSlotData()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                int slotNumber = i + 1;
                string fileName = $"{inputField.text} - {slotNumber}";
                string path = GetSavePath(fileName);

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

        /// <summary>
        /// Determines the save path based on PlayerPrefs or file mode.
        /// </summary>
        private string GetSavePath(string fileName)
        {
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
        /// Applies loaded data to the UI slot.
        /// </summary>
        private void ApplySlotData(SaveCustomFile data, int index)
        {
            slots[index].label.text = $"{inputField.text} - {index + 1} ({data.gameTime})";
            slots[index].image.texture = RenderScreenshot(data.screenshot);
            SetSlotActive(index, true);
        }

        /// <summary>
        /// Updates slot visual and interactive state.
        /// </summary>
        private void SetSlotActive(int index, bool active)
        {
            slots[index].button.interactable = active;
            slots[index].image.color = active ? Color.white : new Color(1, 1, 1, 0);
        }

        #endregion

        #region === Cleanup ===

        /// <summary>
        /// Clears all textures from the save slots to avoid memory leaks.
        /// </summary>
        public void ClearSlotTextures()
        {
            foreach (var slot in slots)
            {
                if (slot.image.texture != null)
                {
                    Destroy(slot.image.texture);
                    slot.image.texture = null;
                }
            }
        }

        #endregion
    }
}