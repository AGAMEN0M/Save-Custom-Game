/*
 * ---------------------------------------------------------------------------
 * Description: Manages the in-game load menu UI for selecting and loading saved 
 *              game states. Handles dynamic slot labeling, page navigation, and 
 *              confirmation dialogs. Supports both PlayerPrefs and external file 
 *              saving modes, automatically detecting available data and rendering 
 *              associated thumbnails and metadata per slot. Enables flexible 
 *              loading of autosaves and manual saves across multiple pages.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using SaveCustomGame;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

using static SaveCustomGame.SaveDataUtility;

[AddComponentMenu("UI/Save Custom Game/Load Menu Manager (Legacy)")]
public class LoadMenuManager : MonoBehaviour
{
    [Header("Load Settings")]
    [SerializeField] private SaveCustomInScene saveCustomInScene; // Reference to the SaveCustomInScene component that handles loading data.
    [Space(10)]
    [Header("Confirmation Panel Settings")]
    [SerializeField] private GameObject confirmationPanel; // Panel that prompts for confirmation before loading a save.
    [SerializeField] private Button confirmButton; // Button to confirm the load.
    [SerializeField] private Button cancelButton; // Button to cancel the load operation.
    [Space(10)]
    [Header("Title Systems")]
    [SerializeField] private Text titleLoad; // Text component that displays the title based on the current load slot.
    public string text = "Load"; // Default text for load slots.
    public string textAutomatic = "Autosave"; // Text used for automatic save slots.
    [Space(10)]
    // References to buttons, raw images, and text for each load slot.
    [Header("Button systems")]
    [SerializeField] private string loadPath1;
    [SerializeField] private Button buttonLoad1;
    [SerializeField] private RawImage rawImageLoad1;
    [SerializeField] private Text textLoad1;
    [Space(5)]
    [SerializeField] private string loadPath2;
    [SerializeField] private Button buttonLoad2;
    [SerializeField] private RawImage rawImageLoad2;
    [SerializeField] private Text textLoad2;
    [Space(5)]
    [SerializeField] private string loadPath3;
    [SerializeField] private Button buttonLoad3;
    [SerializeField] private RawImage rawImageLoad3;
    [SerializeField] private Text textLoad3;
    [Space(5)]
    [SerializeField] private string loadPath4;
    [SerializeField] private Button buttonLoad4;
    [SerializeField] private RawImage rawImageLoad4;
    [SerializeField] private Text textLoad4;
    [Space(5)]
    [SerializeField] private string loadPath5;
    [SerializeField] private Button buttonLoad5;
    [SerializeField] private RawImage rawImageLoad5;
    [SerializeField] private Text textLoad5;
    [Space(5)]
    [SerializeField] private string loadPath6;
    [SerializeField] private Button buttonLoad6;
    [SerializeField] private RawImage rawImageLoad6;
    [SerializeField] private Text textLoad6;
    [Space(10)]
    [Header("Page systems")]
    // Navigation buttons and input field for selecting and navigating load slots.
    [SerializeField][Tooltip("-->")] private Button right; // Button to go to the next page.
    [SerializeField][Tooltip("<--")] private Button left; // Button to go to the previous page.
    [SerializeField] private InputField inputField; // Input field for selecting the load slot number.

    private int currentLoadNumber = 0; // Current selected load slot number.
    private bool firstTime; // Flag to track if this is the first time setting up the buttons and input field.
    private readonly string loadKey = "SaveMenuManager"; // Key used for saving the current load slot number in PlayerPrefs.

    // Initializes the SaveCustomInScene component, retrieves the current load slot from PlayerPrefs, and sets up the UI elements (buttons, input field, and title).
    private void OnEnable()
    {
        saveCustomInScene = GetComponentSaveCustomInScene(); // Get the SaveCustomInScene component if not assigned.

        // Retrieve the last selected load slot number from PlayerPrefs.
        if (PlayerPrefs.HasKey(loadKey))
        {
            currentLoadNumber = PlayerPrefs.GetInt(loadKey); // Retrieve the current load slot number.
        }

        // Setup the UI elements for buttons, input field, load name, and title display.
        SetupButtonsAndInputField();
        SetupInitialLoadName();
        SetTitle();
    }

    // Clear existing preview images and textures.
    private void OnDisable()
    {
        ClearRawImagesAndTextures();
    }

    /// <summary>
    /// Sets the title text in the UI based on the current load number.
    /// Displays "Autosave" if the load number is 0, otherwise displays the regular load name.
    /// </summary>
    public void SetTitle()
    {
        // Set the title based on whether it's an autosave or a regular load slot.
        if (currentLoadNumber == 0)
        {
            titleLoad.text = textAutomatic; // Show "Autosave" title.
        }
        else
        {
            titleLoad.text = text; // Show the regular load title.
        }
    }

    // Sets up the navigation buttons, input field, and listeners for load buttons.
    // Ensures that setup only occurs once, and updates the UI with relevant listeners and interactions.
    private void SetupButtonsAndInputField()
    {
        // Ensure setup happens only once.
        if (!firstTime && right != null && left != null && inputField != null)
        {
            // Add listeners for navigation buttons and input field events.
            right.onClick.AddListener(IncreaseNumber); // Navigate to the next load slot.
            left.onClick.AddListener(DecreaseNumber); // Navigate to the previous load slot.
            inputField.onValidateInput += ValidateInput; // Validate user input in the input field.
            inputField.onEndEdit.AddListener(OnEndEditInputField); // Handle input field editing completion.

            // Add listeners for each load button.
            buttonLoad1.onClick.AddListener(() => LoadGame(1)); // load data to slot 1.
            buttonLoad2.onClick.AddListener(() => LoadGame(2)); // load data to slot 2.
            buttonLoad3.onClick.AddListener(() => LoadGame(3)); // load data to slot 3.
            buttonLoad4.onClick.AddListener(() => LoadGame(4)); // load data to slot 4.
            buttonLoad5.onClick.AddListener(() => LoadGame(5)); // load data to slot 5.
            buttonLoad6.onClick.AddListener(() => LoadGame(6)); // load data to slot 6.

            cancelButton.onClick.AddListener(CancelLoad); // Handle cancellation of the load operation.

            firstTime = true; // Mark the setup as complete.
        }
    }

    // Initializes the input field with the current load number and updates the displayed load slot names accordingly.
    private void SetupInitialLoadName()
    {
        if (inputField != null)
        {
            inputField.text = currentLoadNumber.ToString(); // Set the input field text to reflect the current load number.
            UpdateLoadNames(); // Refresh load slot labels and related data based on the current load number.
        }
    }

    // Updates load slot labels, stores the current load number, and refreshes UI data.
    private void UpdateLoadNames()
    {
        string savePrefix = $"{inputField.text} -"; // Generate prefix using the current load number from the input field.

        // Update the label for each load slot with the current load prefix and slot index.
        textLoad1.text = $"{savePrefix} 1";
        textLoad2.text = $"{savePrefix} 2";
        textLoad3.text = $"{savePrefix} 3";
        textLoad4.text = $"{savePrefix} 4";
        textLoad5.text = $"{savePrefix} 5";
        textLoad6.text = $"{savePrefix} 6";

        PlayerPrefs.SetInt(loadKey, currentLoadNumber); // Save the current load number persistently using PlayerPrefs.
        left.interactable = currentLoadNumber > 0; // Enable the left navigation button only if the number is greater than 0.

        ClearRawImagesAndTextures(); // Remove previously displayed screenshots.
        LoadData(); // Load save data and screenshots for each slot based on the updated load number.
        SetTitle(); // Refresh the title label in the UI based on the current context (e.g., autosave or manual).
    }

    /// <summary>
    /// Increments the load number and refreshes UI elements to reflect the new state.
    /// </summary>
    public void IncreaseNumber()
    {
        if (inputField != null)
        {
            int.TryParse(inputField.text, out currentLoadNumber); // Attempt to parse the current value from the input field.
            currentLoadNumber++; // Increment the parsed load number.
            inputField.text = currentLoadNumber.ToString(); // Reflect the new value back in the input field.
            UpdateLoadNames(); // Refresh load slot labels and related data based on the new number.
        }
    }

    /// <summary>
    /// Decreases the load slot number and updates the UI elements accordingly.
    /// </summary>
    public void DecreaseNumber()
    {
        if (inputField != null && currentLoadNumber > 0)
        {
            int.TryParse(inputField.text, out currentLoadNumber); // Parse the current load number from the input field text.
            currentLoadNumber--; // Decrement the load number if it is greater than zero.
            inputField.text = currentLoadNumber.ToString(); // Update the input field text with the new load number.
            UpdateLoadNames(); // Refresh load slot labels and related data using the new load number.
        }
    }

    // Validates input to ensure only numeric characters are allowed in the input field.
    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        return char.IsDigit(addedChar) ? addedChar : '\0'; // Allow only digits in the input field.
    }

    /// <summary>
    /// Parses the input field value and updates the load slot display when editing ends.
    /// </summary>
    /// <param name="value">The text entered by the user in the input field.</param>
    public void OnEndEditInputField(string value)
    {
        if (inputField != null)
        {
            int.TryParse(value, out currentLoadNumber); // Convert the entered text to an integer and update currentLoadNumber.
            UpdateLoadNames(); // Update load slot labels and related data to reflect the new number.
        }
    }

    /// <summary>
    /// Triggers loading of game data from a specific slot. If data is present, shows a confirmation panel before loading.
    /// </summary>
    /// <param name="button">The slot number to load into (1 to 6).</param>
    public void LoadGame(int button)
    {
        // Check if the load slot is occupied.
        if (IsLoadSlotOccupied(button))
        {
            confirmationPanel.SetActive(true); // Show confirmation dialog if the selected load slot is already used.

            // Clear existing listeners and set new confirmation action for overwriting.
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => ConfirmLoad(button));
        }
    }

    /// <summary>
    /// Checks whether the specified load slot is currently occupied (i.e., has a load texture).
    /// </summary>
    /// <param name="slotNumber">The slot number to check (1 to 6).</param>
    /// <returns>True if the slot contains Load data; otherwise, false.</returns>
    private bool IsLoadSlotOccupied(int slotNumber)
    {
        return slotNumber switch
        {
            1 => rawImageLoad1.texture != null,
            2 => rawImageLoad2.texture != null,
            3 => rawImageLoad3.texture != null,
            4 => rawImageLoad4.texture != null,
            5 => rawImageLoad5.texture != null,
            6 => rawImageLoad6.texture != null,
            _ => false, // Return false for unknown slot numbers.
        };
    }

    /// <summary>
    /// Clears all textures from the load slot preview images to remove existing thumbnails.
    /// </summary>
    public void ClearRawImagesAndTextures()
    {
        RawImage[] rawImages = { rawImageLoad1, rawImageLoad2, rawImageLoad3, rawImageLoad4, rawImageLoad5, rawImageLoad6 };

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
    /// Confirms the load operation and retrieves game data from the specified save slot.
    /// </summary>
    /// <param name="button">The slot number confirmed for saving (1 to 6).</param>
    public void ConfirmLoad(int button)
    {
        saveCustomInScene.fileName = $"{inputField.text} - {button}"; // Construct the file name using the input field value and selected slot number.

        // Set the appropriate load path based on the selected slot.
        switch (button)
        {
            case 1: saveCustomInScene.savePath = loadPath1; break;
            case 2: saveCustomInScene.savePath = loadPath2; break;
            case 3: saveCustomInScene.savePath = loadPath3; break;
            case 4: saveCustomInScene.savePath = loadPath4; break;
            case 5: saveCustomInScene.savePath = loadPath5; break;
            case 6: saveCustomInScene.savePath = loadPath6; break;
            default:
                Debug.LogWarning("Slot number out of range!");
                break;
        }

        saveCustomInScene.LoadData(); // Load the selected game.
        confirmationPanel.SetActive(false); // Close the confirmation dialog.
    }

    /// <summary>
    /// Cancels the pending Load operation and closes the confirmation panel without loading.
    /// </summary>
    public void CancelLoad()
    {
        confirmationPanel.SetActive(false); // Hide the confirmation UI without making any changes.
    }

    /// <summary>
    /// Loads and displays save data for all six load slots, from PlayerPrefs or local files.
    /// </summary>
    public void LoadData()
    {
        // Clear all load paths before retrieving new data.
        loadPath1 = "";
        loadPath2 = "";
        loadPath3 = "";
        loadPath4 = "";
        loadPath5 = "";
        loadPath6 = "";

        // Loop through each load slot index (1 to 6).
        for (int i = 1; i <= 6; i++)
        {
            string fileName = $"{inputField.text} - {i}"; // Compose the file name based on the current input and slot index.
            string savePath = DetermineLoadPath(fileName); // Resolve the full load path for this file name.

            if (saveCustomInScene.saveCustomObject.playerPrefs)
            {
                // If saving through PlayerPrefs is enabled.
                string jsonData = PlayerPrefs.GetString(fileName); // Try to load JSON string from PlayerPrefs.
                var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize the data to an object.

                if (PlayerPrefs.HasKey(fileName))
                {
                    LoadDataString(data, i, ""); // Populate load slot with data.
                    InteractableSlot(i, true); // Display occupied load slot preview.
                }
                else
                {
                    InteractableSlot(i, false); // Show empty load slot if no data found.
                }
            }
            else
            {
                // If loading via file system is enabled.
                if (File.Exists(savePath))
                {
                    string jsonData = File.ReadAllText(savePath); // Load JSON from file.
                    var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize to SaveCustomFile.

                    LoadDataString(data, i, savePath); // Populate load slot with data.
                    InteractableSlot(i, true); // Display occupied load slot preview.
                }
                else
                {
                    InteractableSlot(i, false); // Show empty load slot if file doesn't exist.
                }
            }
        }
    }

    // Determines the full file path for a given load file name, based on load configuration.
    private string DetermineLoadPath(string fileName)
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
    /// Sets the visibility of the raw image for a specific load slot.
    /// </summary>
    /// <param name="slotNumber">The load slot number (1–6).</param>
    /// <param name="interactable">True to enable the slot and show image; false to disable and hide image.
    private void InteractableSlot(int slotNumber, bool interactable)
    {
        // Set color to white if available, or fully transparent if not.
        Color slotColor = interactable ? Color.white : new Color(1f, 1f, 1f, 0f);

        // Apply the color to the corresponding raw image based on slot number.
        switch (slotNumber)
        {
            case 1:
                buttonLoad1.interactable = interactable;
                rawImageLoad1.color = slotColor;
                break;
            case 2:
                buttonLoad2.interactable = interactable;
                rawImageLoad2.color = slotColor;
                break;
            case 3:
                buttonLoad3.interactable = interactable;
                rawImageLoad3.color = slotColor;
                break;
            case 4:
                buttonLoad4.interactable = interactable;
                rawImageLoad4.color = slotColor;
                break;
            case 5:
                buttonLoad5.interactable = interactable;
                rawImageLoad5.color = slotColor;
                break;
            case 6:
                buttonLoad6.interactable = interactable;
                rawImageLoad6.color = slotColor;
                break;
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
    private void LoadDataString(SaveCustomFile data, int slotNumber, string savePath)
    {
        string displayText = $"{inputField.text} - {slotNumber} ({data.gameTime})"; // Compose the text showing slot ID and game time.
        Texture screenshot = RenderScreenshot(data.screenshot); // Convert the saved screenshot data into a Texture.

        // Assign the text and image to the appropriate UI elements based on slot number.
        switch (slotNumber)
        {
            case 1:
                textLoad1.text = displayText;
                rawImageLoad1.texture = screenshot;
                loadPath1 = savePath;
                break;
            case 2:
                textLoad2.text = displayText;
                rawImageLoad2.texture = screenshot;
                loadPath2 = savePath;
                break;
            case 3:
                textLoad3.text = displayText;
                rawImageLoad3.texture = screenshot;
                loadPath3 = savePath;
                break;
            case 4:
                textLoad4.text = displayText;
                rawImageLoad4.texture = screenshot;
                loadPath4 = savePath;
                break;
            case 5:
                textLoad5.text = displayText;
                rawImageLoad5.texture = screenshot;
                loadPath5 = savePath;
                break;
            case 6:
                textLoad6.text = displayText;
                rawImageLoad6.texture = screenshot;
                loadPath6 = savePath;
                break;
            default:
                Debug.LogWarning("Slot number out of range!");
                break;
        }
    }
}