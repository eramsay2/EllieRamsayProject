using UnityEngine;

[RequireComponent(typeof(PositionReplayer))]
public class AutoJointAssigner : MonoBehaviour
{
    void Awake()
    {
        PositionReplayer replayer = GetComponent<PositionReplayer>();
        if (replayer == null) return;

        string[] jointNames = new string[]
        {
            // Right hand joints (0–25)
            "R_Wrist", "R_IndexMetacarpal", "R_IndexProximal", "R_IndexIntermediate", "R_IndexDistal", "R_IndexTip",
            "R_LittleMetacarpal", "R_LittleProximal", "R_LittleIntermediate", "R_LittleDistal", "R_LittleTip",
            "R_MiddleMetacarpal", "R_MiddleProximal", "R_MiddleIntermediate", "R_MiddleDistal", "R_MiddleTip",
            "R_Palm", "R_RingMetacarpal", "R_RingProximal", "R_RingIntermediate", "R_RingDistal", "R_RingTip",
            "R_ThumbMetacarpal", "R_ThumbProximal", "R_ThumbDistal", "R_ThumbTip",

            // Left hand joints (26–51)
            "L_Wrist", "L_IndexMetacarpal", "L_IndexProximal", "L_IndexIntermediate", "L_IndexDistal", "L_IndexTip",
            "L_LittleMetacarpal", "L_LittleProximal", "L_LittleIntermediate", "L_LittleDistal", "L_LittleTip",
            "L_MiddleMetacarpal", "L_MiddleProximal", "L_MiddleIntermediate", "L_MiddleDistal", "L_MiddleTip",
            "L_Palm", "L_RingMetacarpal", "L_RingProximal", "L_RingIntermediate", "L_RingDistal", "L_RingTip",
            "L_ThumbMetacarpal", "L_ThumbProximal", "L_ThumbDistal", "L_ThumbTip",

            // Optional wrist anchors (52–58)
            "R_WristAnchor", "L_WristAnchor", "R_HandAnchor", "L_HandAnchor", "R_Elbow", "L_Elbow", "Chest"
        };

        if (replayer.joint == null || replayer.joint.Length != jointNames.Length)
            replayer.joint = new GameObject[jointNames.Length];

        for (int i = 0; i < jointNames.Length; i++)
        {
            GameObject found = GameObject.Find(jointNames[i]);
            if (found != null)
            {
                replayer.joint[i] = found;
            }
            else
            {
                Debug.LogWarning($"Joint GameObject '{jointNames[i]}' not found in scene.");
            }
        }

        Debug.Log("Auto-assigned joints to PositionReplayer.");
    }
}