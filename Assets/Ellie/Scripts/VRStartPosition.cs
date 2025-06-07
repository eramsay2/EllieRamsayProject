using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class VRStartPosition : MonoBehaviour
{
    [SerializeField] private GameObject XROrigin;

    [SerializeField] private Vector3 desiredLocation;
    [SerializeField] private Vector3 desiredForward = Vector3.forward;

    private void Start()
    {
        // Get current headset (camera) forward direction projected on horizontal plane
        Transform cameraTransform = Camera.main.transform;
        Vector3 currentForward = cameraTransform.forward;
        currentForward.y = 0;
        currentForward.Normalize();

        // Project desired direction on horizontal plane
        Vector3 targetForward = desiredForward;
        targetForward.y = 0;
        targetForward.Normalize();

        // Calculate the rotation difference
        Quaternion rotationDelta = Quaternion.FromToRotation(currentForward, targetForward);

        // Rotate the XR Origin
        XROrigin.transform.rotation = rotationDelta * XROrigin.transform.rotation;

        //Position the XR Origin
        XROrigin.transform.position = desiredLocation;
    }
}
