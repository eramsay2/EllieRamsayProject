using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class LeftHandPinchMenu : MonoBehaviour
{
    [Header("Input Action Reference")]
    public InputActionProperty leftIndexPressed;

    [Header("Menu to Toggle")]
    public GameObject menuUI;

    [SerializeField] private EnableDisableHands ToggleHands;

    private bool wasPinching = false;

    private void Start()
    {
        ToggleHands = GameObject.Find("XR Origin (XR Rig)").GetComponent<EnableDisableHands>();
    }

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
            ToggleHands.ToggleHands();
        }
    }
}
