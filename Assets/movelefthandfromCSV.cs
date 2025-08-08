using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class moveLeftHandFromCSV : MonoBehaviour
{
    [Tooltip("All left-hand joints in order: Joint_26 to Joint_51")]
    public Transform[] leftHandJoints;

    public string fileName = "TrialPillsBothHandsRepeated.csv";
    public float secondsPerFrame = 0.033f;

    private struct JointFrame
    {
        public int frame;
        public int jointIndex;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    private List<JointFrame> csvData = new List<JointFrame>();
    private int currentFrame = 0;
    private int totalFrames = 0;

    IEnumerator Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string csvText = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load file: " + filePath);
            yield break;
        }
        csvText = www.downloadHandler.text;
#else
        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV file not found: " + filePath);
            yield break;
        }
        csvText = File.ReadAllText(filePath);
#endif

        ParseCSV(csvText);
        AssignFrames();
        totalFrames = csvData[csvData.Count - 1].frame + 1;

        Debug.Log($"Loaded {totalFrames} frames for left hand.");
        StartCoroutine(Playback());
    }

    void ParseCSV(string csvText)
    {
        var lines = csvText.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("FrameIndex"))
                continue;

            var parts = line.Trim().Split(',');
            if (parts.Length < 13)
                continue;

            // Only joints
            if (!parts[1].Equals("Joint"))
                continue;

            string objectName = parts[2];
            if (!objectName.StartsWith("Joint_")) continue;
            int jointIndex = int.Parse(objectName.Replace("Joint_", ""));

            // Only process left-hand joints (26–51)
            if (jointIndex < 26 || jointIndex > 51) continue;

            JointFrame jf = new JointFrame
            {
                frame = int.Parse(parts[0]),
                jointIndex = jointIndex,
                position = new Vector3(
                    float.Parse(parts[3]),
                    float.Parse(parts[4]),
                    float.Parse(parts[5])
                ),
                rotation = new Quaternion(
                    float.Parse(parts[7]),
                    float.Parse(parts[8]),
                    float.Parse(parts[9]),
                    float.Parse(parts[6])
                ),
                scale = new Vector3(
                    float.Parse(parts[10]),
                    float.Parse(parts[11]),
                    float.Parse(parts[12])
                )
            };

            csvData.Add(jf);
        }
    }

    void AssignFrames()
    {
        int currentFrameNum = 0;
        for (int i = 0; i < csvData.Count; i++)
        {
            if (i > 0 && csvData[i].jointIndex == 26) // new frame starts at L_Wrist
                currentFrameNum++;

            var temp = csvData[i];
            temp.frame = currentFrameNum;
            csvData[i] = temp;
        }
    }

    IEnumerator Playback()
    {
        while (true)
        {
            ApplyFrame(currentFrame);
            currentFrame++;
            if (currentFrame >= totalFrames) currentFrame = 0;
            yield return new WaitForSeconds(secondsPerFrame);
        }
    }

    void ApplyFrame(int frameNum)
    {
        // Apply all joints for this frame
        for (int j = 0; j < leftHandJoints.Length; j++)
        {
            if (leftHandJoints[j] == null) continue;
            int csvJointIndex = 26 + j;

            // Find the data for this frame/joint
            JointFrame? jf = csvData.Find(x => x.frame == frameNum && x.jointIndex == csvJointIndex);
            if (jf == null) continue;

            if (csvJointIndex == 26)
            {
                // Root joint gets position + global rotation
                leftHandJoints[j].position = jf.Value.position;
                leftHandJoints[j].rotation = jf.Value.rotation;
            }
            else
            {
                // Convert global to local rotation
                Quaternion parentGlobal = leftHandJoints[j].parent.rotation;
                Quaternion localRot = Quaternion.Inverse(parentGlobal) * jf.Value.rotation;
                leftHandJoints[j].localRotation = localRot;
            }

            // Optional: Apply scale
            leftHandJoints[j].localScale = jf.Value.scale;
        }
    }
}