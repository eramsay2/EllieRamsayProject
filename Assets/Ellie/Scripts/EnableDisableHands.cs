using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class EnableDisableHands : MonoBehaviour
{
    // This scripts enables or disables the hands during runtime
    [Header("Components")]
    [SerializeField] private XRInputModalityManager modalityManager;
    [SerializeField] private GameObject LeftHand;
    [SerializeField] private GameObject RightHand;

    private bool enableHands = false;

    public void ToggleHands()
    {
        enableHands = !enableHands;

        modalityManager.enabled = (enableHands);

        LeftHand.SetActive(enableHands);
        RightHand.SetActive(enableHands);
    }
}
