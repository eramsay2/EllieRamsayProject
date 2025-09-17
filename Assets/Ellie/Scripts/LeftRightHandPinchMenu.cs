using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BothHandsPinchMenu : MonoBehaviour
{
    [Header("Input Action References")]
    public InputActionProperty leftIndexPressed;
    public InputActionProperty rightIndexPressed;

    [Header("Menu to Toggle")]
    public GameObject menuUI;

    private EnableDisableHands ToggleHands;
    private bool wasPinching = false;

    private void Start()
    {
        ToggleHands = GameObject.Find("XR Origin (XR Rig)").GetComponent<EnableDisableHands>();
    }

    private void OnEnable()
    {
        leftIndexPressed.action.Enable();
        rightIndexPressed.action.Enable();
    }

    private void OnDisable()
    {
        leftIndexPressed.action.Disable();
        rightIndexPressed.action.Disable();
    }

    private void Update()
    {
        // read pinch values (0.0 to 1.0)
        float leftPinch = leftIndexPressed.action.ReadValue<float>();
        float rightPinch = rightIndexPressed.action.ReadValue<float>();

        // true if either hand is pinching
        bool isPinching = (leftPinch > 0.8f) || (rightPinch > 0.8f);

        if (isPinching && !wasPinching)
        {
            // if menu is not active yet -> open it
            if (!menuUI.activeSelf)
            {
                ToggleMenu();
            }
            else
            {
                // if menu is active -> treat pinch as a click
                TryClickUI();
            }
        }

        wasPinching = isPinching;
    }

    private void ToggleMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(!menuUI.activeSelf);
            ToggleHands.ToggleHands();
        }
    }

    private void TryClickUI()
    {
        // This uses Unity’s EventSystem to simulate a button click
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }
}

