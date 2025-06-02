using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/* This script helps the pinch and grab animations to work on the hand GameObject on input */

public class AnimateHandOnInput : MonoBehaviour
{
    // Input action property for the pinch animation. This should be assigned in the Inspector.
    public InputActionProperty pinchAnimationAction;

    // Input action property for the grip animation. This should be assigned in the Inspector.
    public InputActionProperty gripAnimationAction;

    // Reference to the Animator component that controls the hand animations.
    public Animator handAnimator;

    // Called before the first frame update. This method is often used for initialization.
    void Start()
    {
        // Currently empty, but initialization code can be added here if needed.
    }

    // Called once per frame. This method is used for continuous updates.
    void Update()
    {
        // Read the current value of the pinch animation input action.
        // This value is typically a float between 0 and 1, representing the degree of the pinch.
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();

        // Set the "Trigger" parameter in the hand animator to the read pinch value.
        // This parameter is likely used in the animator controller to trigger the pinch animation.
        handAnimator.SetFloat("Trigger", triggerValue);

        // Read the current value of the grip animation input action.
        // This value is typically a float between 0 and 1, representing the degree of the grip.
        float gripValue = gripAnimationAction.action.ReadValue<float>();

        // Set the "Grip" parameter in the hand animator to the read grip value.
        // This parameter is likely used in the animator controller to trigger the grip animation.
        handAnimator.SetFloat("Grip", gripValue);
    }
}
