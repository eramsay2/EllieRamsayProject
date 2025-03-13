using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKVRFollow : MonoBehaviour
{
    public Transform VRR;
    public Transform IKR;
    public Transform VRL;
    public Transform IKL;
    public Transform VRHead;
    public Transform IKHead;

    public Vector3 headPositionOffset;
    public Vector3 rotationOffsetL;
    public Vector3 rotationOffsetR;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        IKR.position = VRR.position;
        IKR.rotation = VRR.rotation * Quaternion.Euler(rotationOffsetR);

        IKL.position = VRL.position;
        IKL.rotation = VRL.rotation * Quaternion.Euler(rotationOffsetL);

        IKHead.position = VRHead.position + headPositionOffset;
        IKHead.rotation = VRHead.rotation;
    }
}
