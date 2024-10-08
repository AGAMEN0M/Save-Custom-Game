/*
 * ---------------------------------------------------------------------------
 * Description: This script automatically selects a specified button when the UI element 
 *              it's attached to is enabled. It is useful in UI navigation, ensuring that 
 *              the correct button is highlighted and ready for interaction by default.
 *              The script clears any previously selected UI object before selecting 
 *              the desired button.
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
    [SerializeField] private Button button; // Reference to the button we want to select by default.

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null); // Clears any previously selected object.
        button.Select(); // Select the desired button.
    }
}