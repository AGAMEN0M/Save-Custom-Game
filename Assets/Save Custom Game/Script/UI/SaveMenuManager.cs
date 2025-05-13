/*
 * ---------------------------------------------------------------------------
 * Description: The SaveMenuManager script manages the UI system for saving game progress 
 *              using up to six save slots per page in a Unity project. It handles user 
 *              interactions with UI elements such as buttons, text labels, thumbnails, 
 *              and input fields. The script supports both PlayerPrefs and file-based 
 *              saving, manages navigation between slot pages, and updates UI dynamically 
 *              based on save availability. It also provides confirmation dialogs to 
 *              prevent accidental overwrites and displays contextual save information, 
 *              including screenshots and timestamps.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using SaveCustomGame;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

using static SaveCustomGame.SaveDataUtility;

[AddComponentMenu("UI/Save Custom Game/Save Menu Manager (Legacy)")]
public class SaveMenuManager : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private SaveCustomInScene saveCustomInScene; // Reference to the SaveCustomInScene component that handles saving data.
    [Space(10)]
    [Header("Confirmation Panel Settings")]
    [SerializeField] private GameObject confirmationPanel; // Panel that prompts for confirmation when overwriting a save.
    [SerializeField] private Button confirmButton; // Button to confirm the save.
    [SerializeField] private Button cancelButton; // Button to cancel the save operation.
    [Space(10)]
    [Header("Title Systems")]
    [SerializeField] private Text titleLoad; // Text component that displays the title based on the current save slot.
    public string text = "Save"; // Default text for save slots.
    public string textAutomatic = "Autosave"; // Text used for automatic save slots.
    [Space(10)]
    [Header("Button systems")]
    // References to buttons, raw images, and text for each save slot.
    [SerializeField] private Button buttonSave1;
    [SerializeField] private RawImage rawImageSave1;
    [SerializeField] private Text textSave1;
    [Space(5)]
    [SerializeField] private Button buttonSave2;
    [SerializeField] private RawImage rawImageSave2;
    [SerializeField] private Text textSave2;
    [Space(5)]
    [SerializeField] private Button buttonSave3;
    [SerializeField] private RawImage rawImageSave3;
    [SerializeField] private Text textSave3;
    [Space(5)]
    [SerializeField] private Button buttonSave4;
    [SerializeField] private RawImage rawImageSave4;
    [SerializeField] private Text textSave4;
    [Space(5)]
    [SerializeField] private Button buttonSave5;
    [SerializeField] private RawImage rawImageSave5;
    [SerializeField] private Text textSave5;
    [Space(5)]
    [SerializeField] private Button buttonSave6;
    [SerializeField] private RawImage rawImageSave6;
    [SerializeField] private Text textSave6;
    [Space(10)]
    [Header("Page systems")]
    // Navigation buttons and input field for selecting and navigating save slots.
    [SerializeField][Tooltip("-->")] private Button right; // Button to go to the next page.
    [SerializeField][Tooltip("<--")] private Button left; // Button to go to the previous page.
    [SerializeField] private InputField inputField; // Input field for selecting the save slot number.

    private int currentSaveNumber = 0; // Current selected save slot number.
    private bool firstTime; // Flag to track if this is the first time setting up the buttons and input field.
    private readonly string saveKey = "SaveMenuManager"; // Key used for saving the current save slot number in PlayerPrefs.

    // Initializes the SaveCustomInScene component, retrieves the current save slot from PlayerPrefs, and sets up the UI elements (buttons, input field, and title).
    private void OnEnable()
    {
        saveCustomInScene = GetComponentSaveCustomInScene(); // Get the SaveCustomInScene component if not assigned.

        // Retrieve the last selected save slot number from PlayerPrefs.
        if (PlayerPrefs.HasKey(saveKey))
        {
            currentSaveNumber = PlayerPrefs.GetInt(saveKey); // Retrieve the current save slot number.
        }

        // Setup the UI elements for buttons, input field, save name, and title display.
        SetupButtonsAndInputField();
        SetupInitialSaveName();
        SetTitle();
    }

    // Clear existing preview images and textures.
    private void OnDisable()
    {
        ClearRawImagesAndTextures();
    }

    /// <summary>
    /// Sets the title text in the UI based on the current save number.
    /// Displays "Autosave" if the save number is 0, otherwise displays the regular save name.
    /// </summary>
    public void SetTitle()
    {
        // Set the title based on whether it's an autosave or a regular save slot.
        if (currentSaveNumber == 0)
        {
            titleLoad.text = textAutomatic; // Show "Autosave" title.
        }
        else
        {
            titleLoad.text = text; // Show the regular save title.
        }
    }

    // Sets up the navigation buttons, input field, and listeners for save buttons.
    // Ensures that setup only occurs once, and updates the UI with relevant listeners and interactions.
    private void SetupButtonsAndInputField()
    {
        // Ensure setup happens only once.
        if (!firstTime && right != null && left != null && inputField != null)
        {
            // Add listeners for navigation buttons and input field events.
            right.onClick.AddListener(IncreaseNumber); // Navigate to the next save slot.
            left.onClick.AddListener(DecreaseNumber); // Navigate to the previous save slot.
            inputField.onValidateInput += ValidateInput; // Validate user input in the input field.
            inputField.onEndEdit.AddListener(OnEndEditInputField); // Handle input field editing completion.

            // Add listeners for each save button.
            buttonSave1.onClick.AddListener(() => SaveGame(1)); // Save data to slot 1.
            buttonSave2.onClick.AddListener(() => SaveGame(2)); // Save data to slot 2.
            buttonSave3.onClick.AddListener(() => SaveGame(3)); // Save data to slot 3.
            buttonSave4.onClick.AddListener(() => SaveGame(4)); // Save data to slot 4.
            buttonSave5.onClick.AddListener(() => SaveGame(5)); // Save data to slot 5.
            buttonSave6.onClick.AddListener(() => SaveGame(6)); // Save data to slot 6.

            cancelButton.onClick.AddListener(CancelSave); // Handle cancellation of the save operation.

            firstTime = true; // Mark the setup as complete.
        }
    }

    // Initializes the input field with the current save number and updates the displayed save slot names accordingly.
    private void SetupInitialSaveName()
    {
        if (inputField != null)
        {
            inputField.text = currentSaveNumber.ToString(); // Set the input field text to reflect the current save number.
            UpdateSaveNames(); // Refresh save slot labels and related data based on the current save number.
        }
    }

    // Updates save slot labels, stores the current save number, and refreshes UI data.
    private void UpdateSaveNames()
    {
        string savePrefix = $"{inputField.text} -"; // Generate prefix using the current save number from the input field.

        // Update the label for each save slot with the current save prefix and slot index.
        textSave1.text = $"{savePrefix} 1";
        textSave2.text = $"{savePrefix} 2";
        textSave3.text = $"{savePrefix} 3";
        textSave4.text = $"{savePrefix} 4";
        textSave5.text = $"{savePrefix} 5";
        textSave6.text = $"{savePrefix} 6";

        PlayerPrefs.SetInt(saveKey, currentSaveNumber); // Save the current save number persistently using PlayerPrefs.
        left.interactable = currentSaveNumber > 0; // Enable the left navigation button only if the number is greater than 0.

        ClearRawImagesAndTextures(); // Remove previously displayed screenshots.
        LoadData(); // Load save data and screenshots for each slot based on the updated save number.
        SetTitle(); // Refresh the title label in the UI based on the current context (e.g., autosave or manual).
    }

    /// <summary>
    /// Increments the save number and refreshes UI elements to reflect the new state.
    /// </summary>
    public void IncreaseNumber()
    {
        if (inputField != null)
        {
            int.TryParse(inputField.text, out currentSaveNumber); // Attempt to parse the current value from the input field.
            currentSaveNumber++; // Increment the parsed save number.
            inputField.text = currentSaveNumber.ToString(); // Reflect the new value back in the input field.
            UpdateSaveNames(); // Refresh save slot labels and related data based on the new number.
        }
    }

    /// <summary>
    /// Decreases the save slot number and updates the UI elements accordingly.
    /// </summary>
    public void DecreaseNumber()
    {
        if (inputField != null && currentSaveNumber > 0)
        {
            int.TryParse(inputField.text, out currentSaveNumber); // Parse the current save number from the input field text.
            currentSaveNumber--; // Decrement the save number if it is greater than zero.
            inputField.text = currentSaveNumber.ToString(); // Update the input field text with the new save number.
            UpdateSaveNames(); // Refresh save slot labels and related data using the new save number.
        }
    }

    // Validates input to ensure only numeric characters are allowed in the input field.
    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        return char.IsDigit(addedChar) ? addedChar : '\0'; // Allow only digits in the input field.
    }

    /// <summary>
    /// Parses the input field value and updates the save slot display when editing ends.
    /// </summary>
    /// <param name="value">The text entered by the user in the input field.</param>
    public void OnEndEditInputField(string value)
    {
        if (inputField != null)
        {
            int.TryParse(value, out currentSaveNumber); // Convert the entered text to an integer and update currentSaveNumber.
            UpdateSaveNames(); // Update save slot labels and related data to reflect the new number.
        }
    }

    /// <summary>
    /// Saves game data to a specific slot. If the slot is already occupied, shows a confirmation panel before overwriting.
    /// </summary>
    /// <param name="button">The slot number to save into (1 to 6).</param>
    public void SaveGame(int button)
    {
        // Check if the save slot is occupied.
        if (IsSaveSlotOccupied(button))
        {
            confirmationPanel.SetActive(true); // Show confirmation dialog if the selected save slot is already used.

            // Clear existing listeners and set new confirmation action for overwriting.
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => ConfirmSave(button));
        }
        else
        {
            saveCustomInScene.fileName = $"{inputField.text} - {button}"; // Construct file name using current save number and selected slot.
            saveCustomInScene.SaveData(); // Perform the save operation.

            ClearRawImagesAndTextures(); // Clear current preview images.
            LoadData(); // Reload save data to reflect the new save.
        }
    }

    /// <summary>
    /// Checks whether the specified save slot is currently occupied (i.e., has a saved texture).
    /// </summary>
    /// <param name="slotNumber">The slot number to check (1 to 6).</param>
    /// <returns>True if the slot contains saved data; otherwise, false.</returns>
    private bool IsSaveSlotOccupied(int slotNumber)
    {
        return slotNumber switch
        {
            1 => rawImageSave1.texture != null,
            2 => rawImageSave2.texture != null,
            3 => rawImageSave3.texture != null,
            4 => rawImageSave4.texture != null,
            5 => rawImageSave5.texture != null,
            6 => rawImageSave6.texture != null,
            _ => false, // Return false for unknown slot numbers.
        };
    }

    /// <summary>
    /// Clears all textures from the save slot preview images to remove existing thumbnails.
    /// </summary>
    public void ClearRawImagesAndTextures()
    {
        RawImage[] rawImages = { rawImageSave1, rawImageSave2, rawImageSave3, rawImageSave4, rawImageSave5, rawImageSave6 };

        foreach (var rawImage in rawImages)
        {
            if (rawImage.texture != null)
            {
                Destroy(rawImage.texture);
                rawImage.texture = null;
            }
        }
    }

    /// <summary>
    /// Confirms the save operation and writes game data to the specified save slot.
    /// </summary>
    /// <param name="button">The slot number confirmed for saving (1 to 6).</param>
    public void ConfirmSave(int button)
    {
        saveCustomInScene.fileName = $"{inputField.text} - {button}"; // Construct the file name using the input field value and selected slot number.
        saveCustomInScene.SaveData(); // Save the current game data to the specified file.

        LoadData(); // Reload data to reflect the new save in the UI.
        confirmationPanel.SetActive(false); // Close the confirmation dialog.
    }

    /// <summary>
    /// Cancels the pending save operation and closes the confirmation panel without saving.
    /// </summary>
    public void CancelSave()
    {
        confirmationPanel.SetActive(false); // Hide the confirmation UI without making any changes.
    }

    /// <summary>
    /// Loads and displays save data for all six save slots, from PlayerPrefs or local files.
    /// </summary>
    public void LoadData()
    {
        // Loop through each save slot index (1 to 6).
        for (int i = 1; i <= 6; i++)
        {
            string fileName = $"{inputField.text} - {i}"; // Compose the file name based on the current input and slot index.
            string savePath = DetermineSavePath(fileName); // Resolve the full save path for this file name.

            if (saveCustomInScene.saveCustomObject.playerPrefs)
            {
                // If saving through PlayerPrefs is enabled.
                string jsonData = PlayerPrefs.GetString(fileName); // Try to load JSON string from PlayerPrefs.
                var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize the data to an object.

                if (PlayerPrefs.HasKey(fileName))
                {
                    LoadDataString(data, i); // Populate save slot with data.
                    RenderImageSlot(i, true); // Display occupied save slot preview.
                }
                else
                {
                    RenderImageSlot(i, false); // Show empty save slot if no data found.
                }
            }
            else
            {
                // If saving via file system is enabled.
                if (File.Exists(savePath))
                {
                    string jsonData = File.ReadAllText(savePath); // Load JSON from file.
                    var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize to SaveCustomFile.

                    LoadDataString(data, i); // Populate save slot with data.
                    RenderImageSlot(i, true); // Display occupied save slot preview.
                }
                else
                {
                    RenderImageSlot(i, false); // Show empty save slot if file doesn't exist.
                }
            }
        }
    }

    // Determines the full file path for a given save file name, based on save configuration.
    private string DetermineSavePath(string fileName)
    {
        string savePath;

        if (saveCustomInScene.saveCustomObject.playerPrefs)
        {
            savePath = ""; // No file path is needed if using PlayerPrefs.
        }
        else if (saveCustomInScene.saveCustomObject.localLow)
        {
            // If using persistent data path (LocalLow), combine it with folder and filename.
            savePath = Path.Combine(Application.persistentDataPath, "saves", $"{fileName}.json");
        }
        else
        {
            // For other setups (e.g., Editor or build output folder), choose based on platform.
        #if UNITY_EDITOR
            savePath = Path.Combine(Application.dataPath, "Editor/saves", $"{fileName}.json");
        #else
            savePath = Path.Combine(Application.dataPath, "saves", $"{fileName}.json");
        #endif
        }

        Debug.Log(savePath);
        return savePath;
    }

    /// <summary>
    /// Sets the visibility of the raw image for a specific save slot.
    /// </summary>
    /// <param name="slotNumber">The save slot number (1–6).</param>
    /// <param name="available">True to show the slot image; false to hide it.</param>
    private void RenderImageSlot(int slotNumber, bool available)
    {
        // Set color to white if available, or fully transparent if not.
        Color slotColor = available ? Color.white : new Color(1f, 1f, 1f, 0f);

        // Apply the color to the corresponding raw image based on slot number.
        switch (slotNumber)
        {
            case 1: rawImageSave1.color = slotColor; break;
            case 2: rawImageSave2.color = slotColor; break;
            case 3: rawImageSave3.color = slotColor; break;
            case 4: rawImageSave4.color = slotColor; break;
            case 5: rawImageSave5.color = slotColor; break;
            case 6: rawImageSave6.color = slotColor; break;
            default:
                Debug.LogWarning("Slot number out of range!");
                break;
        }
    }

    /// <summary>
    /// Loads the save data into the corresponding UI elements for the specified slot.
    /// </summary>
    /// <param name="data">The deserialized save data.</param>
    /// <param name="slotNumber">The slot number to populate (1–6).</param>
    private void LoadDataString(SaveCustomFile data, int slotNumber)
    {
        string displayText = $"{inputField.text} - {slotNumber} ({data.gameTime})"; // Compose the text showing slot ID and game time.
        Texture screenshot = RenderScreenshot(data.screenshot); // Convert the saved screenshot data into a Texture.

        // Assign the text and image to the appropriate UI elements based on slot number.
        switch (slotNumber)
        {
            case 1:
                textSave1.text = displayText;
                rawImageSave1.texture = screenshot;
                break;
            case 2:
                textSave2.text = displayText;
                rawImageSave2.texture = screenshot;
                break;
            case 3:
                textSave3.text = displayText;
                rawImageSave3.texture = screenshot;
                break;
            case 4:
                textSave4.text = displayText;
                rawImageSave4.texture = screenshot;
                break;
            case 5:
                textSave5.text = displayText;
                rawImageSave5.texture = screenshot;
                break;
            case 6:
                textSave6.text = displayText;
                rawImageSave6.texture = screenshot;
                break;
            default:
                Debug.LogWarning("Slot number out of range!");
                break;
        }
    }
}