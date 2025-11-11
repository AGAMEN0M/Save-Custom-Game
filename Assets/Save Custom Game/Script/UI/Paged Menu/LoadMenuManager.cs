/*
 * ---------------------------------------------------------------------------
 * Description: Manages the in-game load menu UI for selecting and loading saved 
 *              game states. Handles dynamic slot labeling, page navigation, and 
 *              confirmation dialogs. Supports both PlayerPrefs and external file 
 *              saving modes, automatically detecting available data and rendering 
 *              associated thumbnails and metadata per slot. Enables flexible 
 *              loading of autosaves and manual saves across multiple pages.
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
    [AddComponentMenu("UI/Save Custom Game/Paged Menu/Load Menu Manager (Legacy)")]
    public class LoadMenuManager : MonoBehaviour
    {
        #region === Structs ===

        /// <summary>
        /// Represents a group of UI components associated with a single load slot.
        /// Used to display screenshot preview, save name, and respond to player input.
        /// </summary>
        [Serializable]
        public struct SlotUI
        {
            [Tooltip("Full save file path used when loading from a file-based save system.")]
            public string path;

            [Tooltip("Button that triggers loading the save file assigned to this slot.")]
            public Button button;

            [Tooltip("RawImage that displays the screenshot preview for this save slot.")]
            public RawImage image;

            [Tooltip("Text field displaying the name and metadata of this save slot.")]
            public Text label;
        }

        #endregion

        #region === Inspector Fields ===

        [Header("Confirmation Panel Settings")]
        [SerializeField, Tooltip("UI panel that appears to confirm the player's decision to load a save.")]
        private GameObject confirmationPanel;

        [SerializeField, Tooltip("Button that confirms the load operation when pressed.")]
        private Button confirmButton;

        [SerializeField, Tooltip("Button that cancels the load operation and closes the confirmation panel.")]
        private Button cancelButton;

        [Header("Title Systems")]
        [SerializeField, Tooltip("UI text that displays the current title, changing based on autosave/manual save mode.")]
        private Text titleLoad;

        [SerializeField, Tooltip("Title to display when browsing manual save slots.")]
        private string text = "Load";

        [SerializeField, Tooltip("Title to display when browsing the autosave slot page.")]
        private string textAutomatic = "Autosave";

        [Header("Slot UI (1 - 6)")]
        [SerializeField, Tooltip("All loadable save slots shown on-screen, each containing screenshot and metadata.")]
        private SlotUI[] slots = new SlotUI[6];

        [Header("Page Systems")]
        [SerializeField, Tooltip("Button used to increase the current save page number.")]
        private Button right;

        [SerializeField, Tooltip("Button used to decrease the current save page number.")]
        private Button left;

        [SerializeField, Tooltip("Numeric field showing the current save page index.")]
        private InputField inputField;

        #endregion

        #region === Private Fields ===

        private SaveCustomInScene saveCustomInScene;
        private int currentLoadNumber = 0;
        private bool firstTime;
        private readonly string loadKey = "SaveMenuManager";

        #endregion

        #region === Public Properties ===

        /// <summary>
        /// Reference to the confirmation panel used during load selection.
        /// </summary>
        public GameObject ConfirmationPanel
        {
            get => confirmationPanel;
            set => confirmationPanel = value;
        }

        /// <summary>
        /// Reference to the button that confirms a load action.
        /// </summary>
        public Button ConfirmButton
        {
            get => confirmButton;
            set => confirmButton = value;
        }

        /// <summary>
        /// Reference to the button that cancels the load confirmation dialog.
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
        /// Displayed title for manual load pages.
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
        /// Called when the menu becomes active. Initializes UI state and loads save previews.
        /// </summary>
        private void OnEnable()
        {
            saveCustomInScene = GetComponentSaveCustomInScene();

            if (PlayerPrefs.HasKey(loadKey)) currentLoadNumber = PlayerPrefs.GetInt(loadKey);

            SetupButtonsAndInputField();
            SetupInitialLoadName();
            SetTitle();
        }

        /// <summary>
        /// Called when the menu is disabled. Ensures all temporary textures are safely destroyed.
        /// </summary>
        private void OnDisable() => ClearRawImagesAndTextures();

        #endregion

        #region === Title ===

        /// <summary>
        /// Updates the displayed title to reflect whether the user is viewing autosaves or manual saves.
        /// </summary>
        public void SetTitle() => titleLoad.text = currentLoadNumber == 0 ? textAutomatic : text;

        #endregion

        #region === Setup UI ===

        /// <summary>
        /// Binds all slot buttons and UI event handlers. Ensures setup runs only once per activation.
        /// </summary>
        private void SetupButtonsAndInputField()
        {
            if (firstTime) return;

            right.onClick.AddListener(IncreaseNumber);
            left.onClick.AddListener(DecreaseNumber);

            inputField.onValidateInput += ValidateInput;
            inputField.onEndEdit.AddListener(OnEndEditInputField);

            cancelButton.onClick.AddListener(CancelLoad);

            for (int i = 0; i < slots.Length; i++)
            {
                int index = i + 1;
                slots[i].button.onClick.AddListener(() => LoadGame(index));
            }

            firstTime = true;
        }

        /// <summary>
        /// Initializes the displayed page index and refreshes visible slot data.
        /// </summary>
        private void SetupInitialLoadName()
        {
            inputField.text = currentLoadNumber.ToString();
            UpdateLoadNames();
        }

        #endregion

        #region === Number Navigation ===

        /// <summary>
        /// Moves to the next save slot page.
        /// </summary>
        public void IncreaseNumber()
        {
            int.TryParse(inputField.text, out currentLoadNumber);
            currentLoadNumber++;
            inputField.text = currentLoadNumber.ToString();
            UpdateLoadNames();
        }

        /// <summary>
        /// Moves to the previous page, if possible.
        /// </summary>
        public void DecreaseNumber()
        {
            if (currentLoadNumber <= 0) return;

            int.TryParse(inputField.text, out currentLoadNumber);
            currentLoadNumber--;
            inputField.text = currentLoadNumber.ToString();
            UpdateLoadNames();
        }

        /// <summary>
        /// Validates numeric input for the page input field.
        /// </summary>
        private char ValidateInput(string text, int index, char c) => char.IsDigit(c) ? c : '\0';

        /// <summary>
        /// Called when user finishes editing the page number input field.
        /// </summary>
        public void OnEndEditInputField(string value)
        {
            int.TryParse(value, out currentLoadNumber);
            UpdateLoadNames();
        }

        #endregion

        #region === Slot UI Refresh ===

        /// <summary>
        /// Updates labels, page state and clears any old screenshot previews.
        /// </summary>
        private void UpdateLoadNames()
        {
            PlayerPrefs.SetInt(loadKey, currentLoadNumber);
            left.interactable = currentLoadNumber > 0;

            string prefix = $"{inputField.text} -";

            for (int i = 0; i < slots.Length; i++)
                slots[i].label.text = $"{prefix} {i + 1}";

            ClearRawImagesAndTextures();
            LoadData();
            SetTitle();
        }

        #endregion

        #region === Load Data ===

        /// <summary>
        /// Loads screenshot previews and save metadata for the currently displayed page.
        /// </summary>
        public void LoadData()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                string fileName = $"{inputField.text} - {i + 1}";
                string path = DetermineLoadPath(fileName);
                slots[i].path = path;

                if (saveCustomInScene.saveCustomObject.playerPrefs)
                {
                    if (!PlayerPrefs.HasKey(fileName))
                    {
                        InteractableSlot(i, false);
                        continue;
                    }

                    var data = JsonUtility.FromJson<SaveCustomFile>(PlayerPrefs.GetString(fileName));
                    LoadDataToSlot(data, i);
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        InteractableSlot(i, false);
                        continue;
                    }

                    var data = JsonUtility.FromJson<SaveCustomFile>(File.ReadAllText(path));
                    LoadDataToSlot(data, i);
                }
            }
        }

        private string DetermineLoadPath(string fileName)
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

        private void LoadDataToSlot(SaveCustomFile data, int i)
        {
            slots[i].label.text = $"{inputField.text} - {i + 1} ({data.gameTime})";
            slots[i].image.texture = RenderScreenshot(data.screenshot);
            InteractableSlot(i, true);
        }

        private void InteractableSlot(int index, bool active)
        {
            slots[index].button.interactable = active;
            slots[index].image.color = active ? Color.white : new Color(1, 1, 1, 0);
        }

        #endregion

        #region === Load Action ===

        /// <summary>
        /// Called when a load slot is clicked. Opens the confirmation panel.
        /// </summary>
        public void LoadGame(int slot)
        {
            if (slots[slot - 1].image.texture == null) return;

            confirmationPanel.SetActive(true);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => ConfirmLoad(slot));
        }

        /// <summary>
        /// Confirms and performs the load process for the selected slot.
        /// </summary>
        public void ConfirmLoad(int slot)
        {
            saveCustomInScene.fileName = $"{inputField.text} - {slot}";
            saveCustomInScene.savePath = slots[slot - 1].path;
            saveCustomInScene.LoadData();
            confirmationPanel.SetActive(false);
        }

        /// <summary>
        /// Cancels the load confirmation dialog.
        /// </summary>
        public void CancelLoad() => confirmationPanel.SetActive(false);

        #endregion

        #region === Cleanup ===

        /// <summary>
        /// Destroys screenshot textures to avoid memory leaks.
        /// </summary>
        public void ClearRawImagesAndTextures()
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