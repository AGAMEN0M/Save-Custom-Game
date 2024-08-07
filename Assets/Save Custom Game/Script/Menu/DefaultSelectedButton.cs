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