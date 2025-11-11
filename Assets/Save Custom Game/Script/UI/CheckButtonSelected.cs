/*
 * ---------------------------------------------------------------------------
 * Description: Ensures UI responsiveness by continuously checking if any button 
 *              is currently selected in the Unity EventSystem. If no active 
 *              selection is found (or if the selected object is inactive), 
 *              this script will re-select a predefined default button. 
 *              Useful for maintaining keyboard or controller navigation.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[AddComponentMenu("UI/Save Custom Game/Check Button Selected")]
public class CheckButtonSelected : MonoBehaviour
{
    #region === Serialized Fields ===

    [SerializeField, Tooltip("Reference to the default button that will be selected if the UI loses focus.")]
    private Button defaultButton; // Reference to the default button to select when no UI element is selected.

    #endregion

    #region === Properties ===

    /// <summary>
    /// Gets or sets the default button to be selected when no UI element is focused.
    /// </summary>
    public Button DefaultButton
    {
        get => defaultButton;
        set => defaultButton = value;
    }

    #endregion

    #region === Unity Methods ===

    /// <summary>
    /// Called once per frame. Ensures the EventSystem always has an active selected UI element.
    /// If no element is selected or the current selection is inactive, the default button is selected instead.
    /// </summary>
    private void Update()
    {
        // Exit immediately if no EventSystem exists in the scene.
        if (EventSystem.current == null) return; // No event system means no UI selection handling.

        // Check if there is no selected UI object or if the selected one became inactive.
        if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
        {
            // Select the default button to maintain input navigation.
            defaultButton.Select();
        }
    }

    #endregion
}