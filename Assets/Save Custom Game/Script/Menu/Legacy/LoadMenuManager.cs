using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour
{
    [Header("Load Settings")]
    [SerializeField] private SaveCustomInScene saveCustomInScene; // Reference to SaveCustomInScene for loading.
    [Space(10)]
    [Header("Confirmation Panel Settings")]
    [SerializeField] private GameObject confirmationPanel; // Panel for confirmation.
    [SerializeField] private Button confirmButton; // Button to confirm load.
    [SerializeField] private Button cancelButton; // Button to cancel load.
    [Space(10)]
    [Header("Title Systems")]
    [SerializeField] private Text titleLoad; // Title text for load menu.
    public string text = "Load"; // Default load text.
    public string textAutomatic = "Autosave"; // Text for autosave.
    [Space(10)]
    [Header("Button systems")]
    [SerializeField] private string loadPath1; // Path for load slot 1.
    [SerializeField] private Button buttonLoad1; // Button for load slot 1.
    [SerializeField] private RawImage rawImageLoad1; // Image for load slot 1.
    [SerializeField] private Text textLoad1; // Text for load slot 1.
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
    [SerializeField][Tooltip("-->")] private Button right; // Button for moving to the next page.
    [SerializeField][Tooltip("<--")] private Button left; // Button for moving to the previous page.
    [SerializeField] private InputField inputField; // Input field for specifying load slot.

    private int currentLoadNumber = 1; // Current load slot number.
    private bool firstTime; // Flag to determine if it's the first time setting up buttons.

    private void OnEnable()
    {
        SaveDataUtility.GetComponentSaveCustomInScene(ref saveCustomInScene); // Fetching the SaveCustomInScene component reference when the object is enabled.
        if (PlayerPrefs.HasKey("SaveMenuManager")) { currentLoadNumber = PlayerPrefs.GetInt("SaveMenuManager"); } // Check if there's a saved value for the current load number and retrieve it from PlayerPrefs.

        // Set up buttons, input fields, load names, and title on enable.
        SetupButtonsAndInputField();
        SetupInitialLoadName();
        SetTitle();
    }

    public void SetTitle()
    {
        // Check if the current load number is for automatic load or regular load and set the title accordingly.
        if (currentLoadNumber == 0)
        { titleLoad.text = textAutomatic; } // Set the title to the automatic load text.
        else { titleLoad.text = text; } // Set the title to the regular load text.
    }

    private void SetupButtonsAndInputField()
    {
        // Check if it's the first time setting up buttons and input fields and they are not null.
        if (!firstTime && right != null && left != null && inputField != null)
        {
            // Add listeners for buttons and input field events.
            right.onClick.AddListener(IncreaseNumber); // Increase the load number when the right arrow button is clicked.
            left.onClick.AddListener(DecreaseNumber); // Decrease the load number when the left arrow button is clicked.
            inputField.onValidateInput += ValidateInput; // Validate input for the load number field.
            inputField.onEndEdit.AddListener(OnEndEditInputField); // Triggered when the input field editing ends.

            // Add listeners for each load slot button to load the corresponding saved data.
            buttonLoad1.onClick.AddListener(() => LoadGame(1));
            buttonLoad2.onClick.AddListener(() => LoadGame(2));
            buttonLoad3.onClick.AddListener(() => LoadGame(3));
            buttonLoad4.onClick.AddListener(() => LoadGame(4));
            buttonLoad5.onClick.AddListener(() => LoadGame(5));
            buttonLoad6.onClick.AddListener(() => LoadGame(6));

            cancelButton.onClick.AddListener(CancelLoad); // Triggered when the cancel button is clicked.

            firstTime = true; // Set the firstTime flag to true after setting up listeners.
        }
    }

    private void SetupInitialLoadName()
    {
        if (inputField != null)
        {
            inputField.text = currentLoadNumber.ToString(); // Check if the input field exists and set its text to the currentLoadNumber.
            UpdateLoadNames(); // Update the names displayed on the load slots based on the initial load number.
        }
    }

    private void UpdateLoadNames()
    {
        string savePrefix = $"{inputField.text} - "; // Define the prefix for the displayed save names based on the input field text.

        // Update the displayed names for each load slot based on the save prefix and slot number.
        textLoad1.text = savePrefix + "1";
        textLoad2.text = savePrefix + "2";
        textLoad3.text = savePrefix + "3";
        textLoad4.text = savePrefix + "4";
        textLoad5.text = savePrefix + "5";
        textLoad6.text = savePrefix + "6";

        PlayerPrefs.SetInt("SaveMenuManager", currentLoadNumber); // Store the currentLoadNumber in PlayerPrefs for future reference.
        left.interactable = currentLoadNumber > 0; // Enable or disable the left button based on whether the currentLoadNumber is greater than 0.

        // Clear the images and textures on load slots, load data, and set the title.
        ClearRawImagesAndTextures();
        LoadData();
        SetTitle();
    }

    public void IncreaseNumber()
    {
        // Check if the inputField is not null.
        if (inputField != null)
        {
            int.TryParse(inputField.text, out currentLoadNumber); // Parse the text in the input field to get the currentLoadNumber.
            currentLoadNumber++; // Increment the currentLoadNumber.
            inputField.text = currentLoadNumber.ToString(); // Update the inputField text with the new currentLoadNumber.
            UpdateLoadNames(); // Update the load names based on the new currentLoadNumber.
        }
    }

    public void DecreaseNumber()
    {
        // Check if the inputField is not null and currentLoadNumber is greater than 0.
        if (inputField != null && currentLoadNumber > 0)
        {
            int.TryParse(inputField.text, out currentLoadNumber); // Parse the text in the input field to get the currentLoadNumber.
            currentLoadNumber--; // Decrement the currentLoadNumber.
            inputField.text = currentLoadNumber.ToString(); // Update the inputField text with the new currentLoadNumber.
            UpdateLoadNames(); // Update the load names based on the new currentLoadNumber.
        }
    }

    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Check if the added character is a digit.
        if (char.IsDigit(addedChar))
        {
            return addedChar; // If it's a digit, return the added character.
        }

        return '\0'; // If not a digit, return null character.
    }

    public void OnEndEditInputField(string value)
    {
        // Check if the input field exists.
        if (inputField != null)
        {
            int.TryParse(value, out currentLoadNumber); // Try parsing the input value to an integer and update the current load number.
            UpdateLoadNames(); // Update the load names based on the new input value.
        }
    }

    public void LoadGame(int button)
    {
        // Check if the slot for loading is occupied.
        if (IsLoadSlotOccupied(button))
        {
            confirmationPanel.SetActive(true); // If the slot is occupied, activate the confirmation panel.

            // Clear all previous click listeners and add a new listener to confirm the load for the specific slot.
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => ConfirmLoad(button));
        }
    }

    private bool IsLoadSlotOccupied(int slotNumber)
    {
        // Check if the specified slot number for loading has data associated with it.
        return slotNumber switch
        {
            // Check each slot number and return true if the associated raw image texture is not null (indicating data presence).
            1 => rawImageLoad1.texture != null,
            2 => rawImageLoad2.texture != null,
            3 => rawImageLoad3.texture != null,
            4 => rawImageLoad4.texture != null,
            5 => rawImageLoad5.texture != null,
            6 => rawImageLoad6.texture != null,
            _ => false, // Return false if the slot number is out of expected range.
        };
    }

    public void ClearRawImagesAndTextures()
    {
        // Clear the textures of all raw image slots to reset them.
        rawImageLoad1.texture = null;
        rawImageLoad2.texture = null;
        rawImageLoad3.texture = null;
        rawImageLoad4.texture = null;
        rawImageLoad5.texture = null;
        rawImageLoad6.texture = null;
    }

    public void ConfirmLoad(int button)
    {
        saveCustomInScene.fileName = $"{inputField.text} - {button}"; // Set the file name for loading based on the selected slot.

        // Switch based on the button number to determine the load path for the selected slot.
        switch (button)
        {
            case 1:
                saveCustomInScene.savePath = loadPath1;
                break;
            case 2:
                saveCustomInScene.savePath = loadPath2;
                break;
            case 3:
                saveCustomInScene.savePath = loadPath3;
                break;
            case 4:
                saveCustomInScene.savePath = loadPath4;
                break;
            case 5:
                saveCustomInScene.savePath = loadPath5;
                break;
            case 6:
                saveCustomInScene.savePath = loadPath6;
                break;
            default:
                Debug.LogWarning("Slot number out of range!");
                break;
        }

        saveCustomInScene.LoadData(); // Trigger the loading of data associated with the selected file.
    }

    public void CancelLoad() { confirmationPanel.SetActive(false); } // Hide the confirmation panel when canceling the load action.

    public void LoadData()
    {
        // Clear all load paths before retrieving new data.
        loadPath1 = "";
        loadPath2 = "";
        loadPath3 = "";
        loadPath4 = "";
        loadPath5 = "";
        loadPath6 = "";

        // Loop through the available save slots to load data.
        for (int i = 1; i <= 6; i++)
        {
            string fileName = $"{inputField.text} - {i}"; // Construct the file name for the current save slot.
            string savePath = DetermineLoadPath(fileName); // Determine the path to locate the saved file.

            if (saveCustomInScene.saveCustomObject.playerPrefs)
            {
                // If using PlayerPrefs, retrieve saved data.
                string jsonData = PlayerPrefs.GetString(fileName);
                var data = JsonUtility.FromJson<SaveCustomFile>(jsonData);

                if (PlayerPrefs.HasKey(fileName))
                {
                    // Load data into UI elements for the current slot.
                    LoadDataString(data, i, "");
                    InteractableSlot(i, true);
                }
                else
                {
                    InteractableSlot(i, false); // Slot is empty, disable interaction and display appropriately.
                }
            }
            else
            {
                if (File.Exists(savePath))
                {
                    // Read the saved file and deserialize JSON data.
                    string jsonData = File.ReadAllText(savePath);
                    var data = JsonUtility.FromJson<SaveCustomFile>(jsonData);

                    // Load data into UI elements for the current slot.
                    LoadDataString(data, i, savePath);
                    InteractableSlot(i, true);
                }
                else
                {
                    InteractableSlot(i, false); // Slot is empty, disable interaction and display appropriately.
                }
            }
        }
    }

    private string DetermineLoadPath(string fileName)
    {
        string savePath;

        // Determine the path based on the saveCustomInScene settings.
        if (saveCustomInScene.saveCustomObject.playerPrefs)
        {
            savePath = ""; // For PlayerPrefs, no specific path is needed.
        }
        else if (saveCustomInScene.saveCustomObject.localLow)
        {
            savePath = Path.Combine(Application.persistentDataPath, "saves", fileName + ".json"); // If using local low, construct the path within the persistent data directory.
        }
        else
        {
#if UNITY_EDITOR
            savePath = Path.Combine(Application.dataPath, "Editor/saves", fileName + ".json"); // In the editor, set a specific path for easy access to saved files.
#else
            savePath = Path.Combine(Application.dataPath, "saves", fileName + ".json"); // For other platforms, use a standard save path within the application data directory.
#endif
        }

        // Output the determined save path for debugging purposes.
        Debug.Log(savePath);
        return savePath;
    }

    private void InteractableSlot(int slotNumber, bool interactable)
    {
        switch (slotNumber)
        {
            case 1:
                buttonLoad1.interactable = interactable; // Enable or disable the interaction of buttonLoad1 based on the 'interactable' parameter.
                rawImageLoad1.color = interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f); // Set the visibility of rawImageLoad1 based on the 'interactable' parameter.
                break;
            case 2:
                buttonLoad2.interactable = interactable; // Enable or disable the interaction of buttonLoad2 based on the 'interactable' parameter.
                rawImageLoad2.color = interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f); // Set the visibility of rawImageLoad2 based on the 'interactable' parameter.
                break;
            case 3:
                buttonLoad3.interactable = interactable; // Enable or disable the interaction of buttonLoad3 based on the 'interactable' parameter.
                rawImageLoad3.color = interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f); // Set the visibility of rawImageLoad3 based on the 'interactable' parameter.
                break;
            case 4:
                buttonLoad4.interactable = interactable; // Enable or disable the interaction of buttonLoad4 based on the 'interactable' parameter.
                rawImageLoad4.color = interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f); // Set the visibility of rawImageLoad4 based on the 'interactable' parameter.
                break;
            case 5:
                buttonLoad5.interactable = interactable; // Enable or disable the interaction of buttonLoad5 based on the 'interactable' parameter.
                rawImageLoad5.color = interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f); // Set the visibility of rawImageLoad5 based on the 'interactable' parameter.
                break;
            case 6:
                buttonLoad6.interactable = interactable; // Enable or disable the interaction of buttonLoad6 based on the 'interactable' parameter.
                rawImageLoad6.color = interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f); // Set the visibility of rawImageLoad6 based on the 'interactable' parameter.
                break;
            default:
                Debug.LogWarning("Slot number out of range!"); // If the slot number is out of range, log a warning.
                break;
        }
    }

    private void LoadDataString(SaveCustomFile data, int slotNumber, string savePath)
    {
        switch (slotNumber)
        {
            case 1:
                textLoad1.text = $"{inputField.text} - 1 ({data.gameTime})"; // Update the display text for the first load slot with the game time information.
                rawImageLoad1.texture = SaveDataUtility.RenderScreenshot(data.screenshot); // Assign the screenshot to the raw image for the first load slot.
                loadPath1 = savePath; // Set the load path for the first slot.
                break;
            case 2:
                textLoad2.text = $"{inputField.text} - 2 ({data.gameTime})"; // Update the display text for the second load slot with the game time information.
                rawImageLoad2.texture = SaveDataUtility.RenderScreenshot(data.screenshot); // Assign the screenshot to the raw image for the second load slot.
                loadPath2 = savePath; // Set the load path for the second slot.
                break;
            case 3:
                textLoad3.text = $"{inputField.text} - 3 ({data.gameTime})"; // Update the display text for the third load slot with the game time information.
                rawImageLoad3.texture = SaveDataUtility.RenderScreenshot(data.screenshot); // Assign the screenshot to the raw image for the third load slot.
                loadPath3 = savePath; // Set the load path for the third slot.
                break;
            case 4:
                textLoad4.text = $"{inputField.text} - 4 ({data.gameTime})"; // Update the display text for the fourth load slot with the game time information.
                rawImageLoad4.texture = SaveDataUtility.RenderScreenshot(data.screenshot); // Assign the screenshot to the raw image for the fourth load slot.
                loadPath4 = savePath; // Set the load path for the fourth slot.
                break;
            case 5:
                textLoad5.text = $"{inputField.text} - 5 ({data.gameTime})"; // Update the display text for the fifth load slot with the game time information.
                rawImageLoad5.texture = SaveDataUtility.RenderScreenshot(data.screenshot); // Assign the screenshot to the raw image for the fifth load slot.
                loadPath5 = savePath; // Set the load path for the fifth slot.
                break;
            case 6:
                textLoad6.text = $"{inputField.text} - 6 ({data.gameTime})"; // Update the display text for the sixth load slot with the game time information.
                rawImageLoad6.texture = SaveDataUtility.RenderScreenshot(data.screenshot); // Assign the screenshot to the raw image for the sixth load slot.
                loadPath6 = savePath; // Set the load path for the sixth slot.
                break;
            default:
                Debug.LogWarning("Slot number out of range!"); // If the slot number is out of range, log a warning.
                break;
        }
    }
}