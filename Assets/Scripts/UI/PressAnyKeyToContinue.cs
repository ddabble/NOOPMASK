using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PressAnyKeyToContinue : MonoBehaviour
{
    private InputAction action;

    [SerializeField]
    private string firstLevelName;

    void Start()
    {
        action = InputSystem.actions.FindAction("Move");
        action.performed += OnPress;
    }

    private void OnDestroy()
    {
        action.performed -= OnPress;
    }

    private void OnPress(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(firstLevelName);
    }
}
