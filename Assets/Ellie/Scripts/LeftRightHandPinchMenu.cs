using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

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

    void OnEnable()
    {
        leftIndexPressed.action.Enable();
        rightIndexPressed.action.Enable();
    }

    void OnDisable()
    {
        leftIndexPressed.action.Disable();
        rightIndexPressed.action.Disable();
    }

    void Update()
    {
        // read pinch values (0.0 to 1.0)
        float leftPinch = leftIndexPressed.action.ReadValue<float>();
        float rightPinch = rightIndexPressed.action.ReadValue<float>();

        // true if either hand is pinching
        bool isPinching = (leftPinch > 0.8f) || (rightPinch > 0.8f);

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
            ToggleHands.ToggleHands();
        }
    }
}