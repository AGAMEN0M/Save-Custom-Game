/*
 * ---------------------------------------------------------------------------
 * Description: This script ensures that a default button is selected when no other UI 
 *              elements are currently selected or when the currently selected element is 
 *              inactive. It checks in each frame whether the EventSystem has a selected 
 *              object and if it is active in the hierarchy, defaulting to a specified button 
 *              if these conditions are not met. This helps maintain consistent UI navigation.
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
    [SerializeField] private Button defaultButton;  // Reference to the default button that will be selected if none are selected or accessible.

    private void Update()
    {
        // Checks whether there are no objects currently selected by the EventSystem or whether any objects in the hierarchy are disabled.
        if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
        {
            defaultButton.Select(); // If there is no object selected or if any object in the hierarchy is disabled, select the default button.
        }
    }
}