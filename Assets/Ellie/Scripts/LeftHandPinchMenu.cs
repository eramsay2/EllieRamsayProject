using UnityEngine;
using UnityEngine.InputSystem;

public class LeftHandPinchMenu : MonoBehaviour
{
    [Header("Input Action Reference")]
    public InputActionProperty leftIndexPressed;

    [Header("Menu to Toggle")]
    public GameObject menuUI;

    private bool wasPinching = false;

    void OnEnable()
    {
        leftIndexPressed.action.Enable();
    }

    void OnDisable()
    {
        leftIndexPressed.action.Disable();
    }

    void Update()
    {
        // indexPressed is typically a float (0.0 to 1.0)
        float pinchValue = leftIndexPressed.action.ReadValue<float>();
        bool isPinching = pinchValue > 0.8f;

        if (isPinching && !wasPinching)
        {
            ToggleMenu();
        }

        wasPinching = isPinching;
    }

    void ToggleMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(!menuUI.activeSelf);
        }
    }
}
