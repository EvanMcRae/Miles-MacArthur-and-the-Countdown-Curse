using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

// This script prevents Alt+Enter (fullscreen input on Windows/Linux) from triggering Submit actions on keyboard
[RequireComponent(typeof(InputSystemUIInputModule))]
public class FixAltEnterSubmit : MonoBehaviour
{
    private InputSystemUIInputModule _uiModule;

    void Awake()
    {
        _uiModule = GetComponent<InputSystemUIInputModule>();
    }

    void OnEnable()
    {
        // Hook into the "performed" callback of whatever action is assigned to Submit
        if (_uiModule != null && _uiModule.submit.action != null)
        {
            _uiModule.submit.action.started += FilterSubmit;
        }
    }

    void OnDisable()
    {
        if (_uiModule != null && _uiModule.submit.action != null)
        {
            _uiModule.submit.action.started -= FilterSubmit;
        }
    }

    private void FilterSubmit(InputAction.CallbackContext context)
    {
        if (Keyboard.current != null && Keyboard.current.altKey.isPressed)
        {
            // Reset the action so the UI module doesn't perform the submit
            context.action.Reset();
        }
    }
}