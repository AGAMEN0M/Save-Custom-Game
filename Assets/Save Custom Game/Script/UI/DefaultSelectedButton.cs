/*
 * ---------------------------------------------------------------------------
 * Description: Automatically highlights and selects a specific UI button when 
 *              the associated GameObject becomes active. This helps enforce a 
 *              consistent navigation experience by resetting the EventSystem’s 
 *              selection to the defined default button on UI enable.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[AddComponentMenu("UI/Save Custom Game/Default Selected Button")]
public class DefaultSelectedButton : MonoBehaviour
{
    #region === Serialized Fields ===

    [SerializeField, Tooltip("Reference to the button that should be selected when this GameObject is enabled.")]
    private Button button; // Reference to the button we want to select by default.

    #endregion

    #region === Properties ===

    /// <summary>
    /// Gets or sets the UI button that will be automatically selected when this object is enabled.
    /// </summary>
    public Button Button
    {
        get => button;
        set => button = value;
    }

    #endregion

    #region === Unity Methods ===

    /// <summary>
    /// Called automatically when the GameObject becomes active.
    /// Ensures the EventSystem resets selection and highlights the desired default button.
    /// </summary>
    private void OnEnable()
    {
        // Clear any previously selected UI object to avoid unwanted retained selection.
        EventSystem.current.SetSelectedGameObject(null);

        // Select the assigned default button to ensure proper navigation focus.
        button.Select();
    }

    #endregion
}