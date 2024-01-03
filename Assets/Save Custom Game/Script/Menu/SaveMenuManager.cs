using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuManager : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private SaveCustomInScene saveCustomInScene; // Variables related to save settings.
    [Space(10)]
    [Header("Confirmation Panel Settings")]
    [SerializeField] private GameObject confirmationPanel; // Panel for confirmation.
    [SerializeField] private Button confirmButton; // Button to confirm Save.
    [SerializeField] private Button cancelButton; // Button to cancel Save.
    [Space(10)]
    [Header("Title Systems")]
    [SerializeField] private TMP_Text titleLoad; // Title text for Save menu.
    public string text = "Save"; // Default text for save slots.
    public string textAutomatic = "Autosave"; // Text for automatic save slot.
    [Space(10)]
    [Header("Button systems")]
    [SerializeField] private Button buttonSave1; // Button for Save slot 1.
    [SerializeField] private RawImage rawImageSave1; // Image for Save slot 1.
    [SerializeField] private TMP_Text textSave1; // Text for Save slot 1.
    [Space(5)]
    [SerializeField] private Button buttonSave2;
    [SerializeField] private RawImage rawImageSave2;
    [SerializeField] private TMP_Text textSave2;
    [Space(5)]
    [SerializeField] private Button buttonSave3;
    [SerializeField] private RawImage rawImageSave3;
    [SerializeField] private TMP_Text textSave3;
    [Space(5)]
    [SerializeField] private Button buttonSave4;
    [SerializeField] private RawImage rawImageSave4;
    [SerializeField] private TMP_Text textSave4;
    [Space(5)]
    [SerializeField] private Button buttonSave5;
    [SerializeField] private RawImage rawImageSave5;
    [SerializeField] private TMP_Text textSave5;
    [Space(5)]
    [SerializeField] private Button buttonSave6;
    [SerializeField] private RawImage rawImageSave6;
    [SerializeField] private TMP_Text textSave6;
    [Space(10)]
    [Header("Page systems")]
    [SerializeField][Tooltip("-->")] private Button right; // Button to navigate to the next page.
    [SerializeField][Tooltip("<--")] private Button left; // Button to navigate to the previous page.
    [SerializeField] private TMP_InputField inputField; // Input field for selecting a save slot number.

    private int currentSaveNumber = 1; // The currently selected save slot number.
    private bool firstTime; // Flag to track the first time setup.

    private void OnEnable()
    {
        SaveDataUtility.GetComponentSaveCustomInScene(ref saveCustomInScene); // Get the SaveCustomInScene component if not assigned.
        if (PlayerPrefs.HasKey("SaveMenuManager")) { currentSaveNumber = PlayerPrefs.GetInt("SaveMenuManager"); } // Retrieve the last selected save slot number from PlayerPrefs.

        // Setup buttons, input field, initial save name, and title display.
        SetupButtonsAndInputField();
        SetupInitialSaveName();
        SetTitle();
    }

    public void SetTitle()
    {
        // Display different titles based on the current save number.
        if (currentSaveNumber == 0)
        { titleLoad.text = textAutomatic; } // Show the automatic save title.
        else { titleLoad.text = text; } // Show the regular save title.
    }

    private void SetupButtonsAndInputField()
    {
        // Ensure setup occurs only once and the required UI elements are available.
        if (!firstTime && right != null && left != null && inputField != null)
        {
            // Add listeners to the navigation buttons and input field events.
            right.onClick.AddListener(IncreaseNumber); // Increase save slot number.
            left.onClick.AddListener(DecreaseNumber); // Decrease save slot number.
            inputField.onValidateInput += ValidateInput; // Validate input characters.
            inputField.onEndEdit.AddListener(OnEndEditInputField); // Handle input field's end edit event.

            // Add listeners to save buttons.
            buttonSave1.onClick.AddListener(() => SaveGame(1)); // Save data to slot 1
            buttonSave2.onClick.AddListener(() => SaveGame(2)); // Save data to slot 2
            buttonSave3.onClick.AddListener(() => SaveGame(3)); // Save data to slot 3
            buttonSave4.onClick.AddListener(() => SaveGame(4)); // Save data to slot 4
            buttonSave5.onClick.AddListener(() => SaveGame(5)); // Save data to slot 5
            buttonSave6.onClick.AddListener(() => SaveGame(6)); // Save data to slot 6

            cancelButton.onClick.AddListener(CancelSave); // Handle cancel save button.

            firstTime = true; // Mark setup as completed.
        }
    }

    private void SetupInitialSaveName()
    {
        if (inputField != null)
        {
            inputField.text = currentSaveNumber.ToString(); // Set the input field's text to the current save number.
            UpdateSaveNames(); // Update the displayed save names based on the current save number.
        }
    }

    private void UpdateSaveNames()
    {
        string savePrefix = $"{inputField.text} - "; // Prefix for save names based on the input field text (save number).

        // Update the displayed save names for each button based on the save number.
        textSave1.text = savePrefix + "1";
        textSave2.text = savePrefix + "2";
        textSave3.text = savePrefix + "3";
        textSave4.text = savePrefix + "4";
        textSave5.text = savePrefix + "5";
        textSave6.text = savePrefix + "6";

        PlayerPrefs.SetInt("SaveMenuManager", currentSaveNumber); // Store the current save number in PlayerPrefs.
        left.interactable = currentSaveNumber > 0; // Enable/disable the left button based on the current save number.

        // Clear the images and textures and load data for the save slots.
        ClearRawImagesAndTextures();
        LoadData();
        SetTitle();
    }

    public void IncreaseNumber()
    {
        if (inputField != null)
        {
            int.TryParse(inputField.text, out currentSaveNumber); // Get the current save number from the input field text.
            currentSaveNumber++; // Increment the save number.
            inputField.text = currentSaveNumber.ToString(); // Set the input field text to the updated save number.
            UpdateSaveNames(); // Update the save names and corresponding data for the new save number.
        }
    }

    public void DecreaseNumber()
    {
        if (inputField != null && currentSaveNumber > 0)
        {
            int.TryParse(inputField.text, out currentSaveNumber); // Get the current save number from the input field text.
            currentSaveNumber--; // Decrease the save number if it's greater than 0.
            inputField.text = currentSaveNumber.ToString(); // Set the input field text to the updated save number.
            UpdateSaveNames(); // Update the save names and corresponding data for the new save number.
        }
    }

    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Validate input: Allow only digits to be entered in the input field.
        if (char.IsDigit(addedChar)) 
        { 
            return addedChar; // If the entered character is a digit, allow it.
        }

        return '\0'; // If not a digit, don't allow the character to be added to the input field.
    }

    public void OnEndEditInputField(string value)
    {
        if (inputField != null)
        {
            int.TryParse(value, out currentSaveNumber); // Parse the text from the input field to an integer and update the currentSaveNumber.
            UpdateSaveNames(); // Update the save names based on the modified currentSaveNumber.
        }
    }

    public void SaveGame(int button)
    {
        // Check if the save slot is occupied.
        if (IsSaveSlotOccupied(button))
        {
            confirmationPanel.SetActive(true); // If the slot is occupied, display the confirmation panel.

            // Remove all previous listeners and add a listener to confirm the save.
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => ConfirmSave(button));
        }
        else // If the slot is not occupied, perform the save directly.
        {
            saveCustomInScene.fileName = $"{inputField.text} - {button}"; // Set the file name for saving.
            saveCustomInScene.SaveData(); // Save the data.

            // Clear the raw images and textures and reload the data to display changes.
            ClearRawImagesAndTextures();
            LoadData();
        }
    }

    private bool IsSaveSlotOccupied(int slotNumber)
    {
        // Check if the given save slot number has data.
        return slotNumber switch
        {
            // Check each slot number and return true if the associated raw image has a texture (occupied).
            1 => rawImageSave1.texture != null,
            2 => rawImageSave2.texture != null,
            3 => rawImageSave3.texture != null,
            4 => rawImageSave4.texture != null,
            5 => rawImageSave5.texture != null,
            6 => rawImageSave6.texture != null,
            // If an unknown slot number is provided, return false.
            _ => false,
        };
    }

    public void ClearRawImagesAndTextures()
    {
        // Clear all RawImage textures by setting them to null (empty texture).
        rawImageSave1.texture = null;
        rawImageSave2.texture = null;
        rawImageSave3.texture = null;
        rawImageSave4.texture = null;
        rawImageSave5.texture = null;
        rawImageSave6.texture = null;
    }

    public void ConfirmSave(int button)
    {
        saveCustomInScene.fileName = $"{inputField.text} - {button}"; // Set the file name for the save based on the input field and the selected button.
        saveCustomInScene.SaveData(); // Save the current data to the specified slot.
        ClearRawImagesAndTextures(); // Clear the UI images and textures.
        LoadData(); // Reload and display updated save data in the UI.
        confirmationPanel.SetActive(false); // Hide the confirmation panel after the save operation is confirmed.
    }

    public void CancelSave() { confirmationPanel.SetActive(false); } // Hide the confirmation panel without saving any data.

    public void LoadData()
    {
        // Loop through each save slot from 1 to 6.
        for (int i = 1; i <= 6; i++)
        {
            string fileName = $"{inputField.text} - {i}"; // Create the file name based on the slot number.
            string savePath = DetermineSavePath(fileName); // Determine the save path based on the file name.

            if (saveCustomInScene.saveCustomObject.playerPrefs)
            {
                string jsonData = PlayerPrefs.GetString(fileName); // Load data from PlayerPrefs if playerPrefs are used for saving.
                var data = JsonUtility.FromJson<SaveCustomFile>(jsonData); // Deserialize JSON data to SaveCustomFile object.

                if (PlayerPrefs.HasKey(fileName))
                {
                    // Load data and render image slot if the data exists in PlayerPrefs.
                    LoadDataString(data, i);
                    RenderImageSlot(i, true);
                }
                else
                {
                    RenderImageSlot(i, false); // Render an empty image slot if no data found in PlayerPrefs.
                }
            }
            else
            {
                if (File.Exists(savePath))
                {
                    // Load data from file if the file exists.
                    string jsonData = File.ReadAllText(savePath);
                    var data = JsonUtility.FromJson<SaveCustomFile>(jsonData);

                    // Load data and render image slot if the file exists.
                    LoadDataString(data, i);
                    RenderImageSlot(i, true);
                }
                else
                {
                    RenderImageSlot(i, false); // Render an empty image slot if the file doesn't exist.
                }
            }
        }
    }

    private string DetermineSavePath(string fileName)
    {
        string savePath;

        if (saveCustomInScene.saveCustomObject.playerPrefs)
        {
            savePath = ""; // If using PlayerPrefs for saving, set savePath as an empty string.
        }
        else if(saveCustomInScene.saveCustomObject.localLow)
        {
            savePath = Path.Combine(Application.persistentDataPath, "saves", fileName + ".json"); // If localLow is set, savePath points to the persistent data path.
        }
        else
        {
            // For other cases (not using PlayerPrefs or localLow), determine the save path based on the platform (Unity Editor or other platforms).
        #if UNITY_EDITOR
            savePath = Path.Combine(Application.dataPath, "Editor/saves", fileName + ".json");
        #else
            savePath = Path.Combine(Application.dataPath, "saves", fileName + ".json");
        #endif
        }

        // Output the determined save path to the console for debugging purposes.
        Debug.Log(savePath);
        return savePath;
    }

    private void RenderImageSlot(int slotNumber, bool available)
    {
        switch (slotNumber)
        {
            case 1:
                // Set the transparency of the RawImage for slot 1 based on availability.
                rawImageSave1.color = available ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
                break;
            case 2:
                // Set the transparency of the RawImage for slot 2 based on availability.
                rawImageSave2.color = available ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
                break;
            case 3:
                // Set the transparency of the RawImage for slot 3 based on availability.
                rawImageSave3.color = available ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
                break;
            case 4:
                // Set the transparency of the RawImage for slot 4 based on availability.
                rawImageSave4.color = available ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
                break;
            case 5:
                // Set the transparency of the RawImage for slot 5 based on availability.
                rawImageSave5.color = available ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
                break;
            case 6:
                // Set the transparency of the RawImage for slot 6 based on availability.
                rawImageSave6.color = available ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
                break;
            default:
                Debug.LogWarning("Slot number out of range!"); // Log a warning if the slot number is out of range.
                break;
        }
    }

    private void LoadDataString(SaveCustomFile data, int slotNumber)
    {
        switch (slotNumber)
        {
            case 1:
                // Update the text for slot 1 with the game time and set the associated texture.
                textSave1.text = $"{inputField.text} - 1 ({data.gameTime})";
                rawImageSave1.texture = SaveDataUtility.RenderScreenshot(data.screenshot);
                break;
            case 2:
                // Update the text for slot 2 with the game time and set the associated texture.
                textSave2.text = $"{inputField.text} - 2 ({data.gameTime})";
                rawImageSave2.texture = SaveDataUtility.RenderScreenshot(data.screenshot);
                break;
            case 3:
                // Update the text for slot 3 with the game time and set the associated texture.
                textSave3.text = $"{inputField.text} - 3 ({data.gameTime})";
                rawImageSave3.texture = SaveDataUtility.RenderScreenshot(data.screenshot);
                break;
            case 4:
                // Update the text for slot 4 with the game time and set the associated texture.
                textSave4.text = $"{inputField.text} - 4 ({data.gameTime})";
                rawImageSave4.texture = SaveDataUtility.RenderScreenshot(data.screenshot);
                break;
            case 5:
                // Update the text for slot 5 with the game time and set the associated texture.
                textSave5.text = $"{inputField.text} - 5 ({data.gameTime})";
                rawImageSave5.texture = SaveDataUtility.RenderScreenshot(data.screenshot);
                break;
            case 6:
                // Update the text for slot 6 with the game time and set the associated texture.
                textSave6.text = $"{inputField.text} - 6 ({data.gameTime})";
                rawImageSave6.texture = SaveDataUtility.RenderScreenshot(data.screenshot);
                break;
            default:
                Debug.LogWarning("Slot number out of range!"); // Log a warning if the slot number is out of range.
                break;
        }
    }
}