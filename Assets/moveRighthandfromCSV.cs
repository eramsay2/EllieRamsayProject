using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class moveRightHandFromCSV : MonoBehaviour
{
    public Transform[] rightHandJoints; // 0–25 in CSV
    public string fileName = "RecordedData.csv";
    public float secondsPerFrame = 0.033f; // playback speed in seconds per frame

    private List<sTransform> rInput = new List<sTransform>();
    private List<Vector3>[] jointPositions;
    private List<Quaternion>[] jointRotations;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    [System.Serializable]
    public struct sTransform
    {
        public int frame;
        public int jointIndex;
        public float posX, posY, posZ;
        public float rotW, rotX, rotY, rotZ;
        public float scaleX, scaleY, scaleZ;
    }

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

        rInput = ParseCSV(csvText);
        AssignFrames(ref rInput);

        // Prepare storage
        jointPositions = new List<Vector3>[26];
        jointRotations = new List<Quaternion>[26];
        for (int i = 0; i < 26; i++)
        {
            jointPositions[i] = new List<Vector3>();
            jointRotations[i] = new List<Quaternion>();
        }

        // Fill from CSV
        foreach (var entry in rInput)
        {
            if (entry.jointIndex < 0 || entry.jointIndex > 25) continue;

            Vector3 pos = new Vector3(entry.posX, entry.posY, entry.posZ);
            Quaternion rot = new Quaternion(entry.rotX, entry.rotY, entry.rotZ, entry.rotW);

            jointPositions[entry.jointIndex].Add(pos);
            jointRotations[entry.jointIndex].Add(rot);
        }

        Debug.Log("Right hand CSV data loaded.");
    }

    List<sTransform> ParseCSV(string csvText)
    {
        List<sTransform> transformList = new List<sTransform>();
        var lines = csvText.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("FrameIndex"))
                continue;

            var parts = line.Trim().Split(',');
            if (parts.Length < 13)
                continue;

            try
            {
                if (!parts[1].Equals("Joint"))
                    continue;

                string objectName = parts[2];
                if (!objectName.StartsWith("Joint_")) continue;
                int jointIndex = int.Parse(objectName.Replace("Joint_", ""));

                sTransform sCol = new sTransform
                {
                    frame = int.Parse(parts[0]),
                    jointIndex = jointIndex,
                    posX = float.Parse(parts[3]),
                    posY = float.Parse(parts[4]),
                    posZ = float.Parse(parts[5]),
                    rotW = float.Parse(parts[6]),
                    rotX = float.Parse(parts[7]),
                    rotY = float.Parse(parts[8]),
                    rotZ = float.Parse(parts[9]),
                    scaleX = float.Parse(parts[10]),
                    scaleY = float.Parse(parts[11]),
                    scaleZ = float.Parse(parts[12])
                };
                transformList.Add(sCol);
            }
            catch
            {
                Debug.LogWarning("Bad line skipped: " + line);
            }
        }

        return transformList;
    }

    void AssignFrames(ref List<sTransform> transforms)
    {
        int currentFrame = 0;
        for (int i = 0; i < transforms.Count; i++)
        {
            if (i > 0 && transforms[i].jointIndex == 0) // frame starts at R_Wrist
            {
                currentFrame++;
            }

            var t = transforms[i];
            t.frame = currentFrame;
            transforms[i] = t;
        }
    }

    void Update()
    {
        if (jointRotations == null || jointRotations[0].Count == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= secondsPerFrame)
        {
            frameTimer = 0f;
            PlayFrame(currentFrame);
            currentFrame = (currentFrame + 1) % jointRotations[0].Count;
        }
    }

    void PlayFrame(int frame)
    {
        for (int i = 0; i < rightHandJoints.Length; i++)
        {
            if (rightHandJoints[i] == null) continue;

            if (i == 0) // Root joint: global position + rotation
            {
                rightHandJoints[i].position = jointPositions[i][frame];
                rightHandJoints[i].rotation = jointRotations[i][frame];
            }
            else // Children: convert global to local
            {
                Quaternion parentGlobalRot = rightHandJoints[i].parent.rotation;
                Quaternion localRot = Quaternion.Inverse(parentGlobalRot) * jointRotations[i][frame];
                rightHandJoints[i].localRotation = localRot;
            }
        }
    }
}