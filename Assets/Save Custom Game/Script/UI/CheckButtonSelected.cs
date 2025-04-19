/*
 * ---------------------------------------------------------------------------
 * Description: Ensures UI responsiveness by continuously checking if any button 
 *              is currently selected in the Unity EventSystem. If no active 
 *              selection is found (or if the selected object is inactive), 
 *              this script will re-select a predefined default button. 
 *              Useful for maintaining keyboard or controller navigation.
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