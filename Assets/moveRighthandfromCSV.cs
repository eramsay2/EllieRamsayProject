using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class moveRightHandFromCSV : MonoBehaviour
{
    public Transform R_Wrist;
    public Transform R_ThumbTip;
    public string fileName = "RecordedData.csv";

    List<sTransform> rInput = new List<sTransform>();

    List<Vector3> listR_WristPos = new List<Vector3>();
    List<Quaternion> listR_WristQua = new List<Quaternion>();
    List<Quaternion> listR_ThumbTipQua = new List<Quaternion>();

    public struct sTransform
    {
        public int frame;
        public int jointIndex;
        public float posX, posY, posZ;
        public float rotW, rotX, rotY, rotZ;
        public float scaleX, scaleY, scaleZ;

        public sTransform(int frame, int jointIndex,
            float posX, float posY, float posZ,
            float rotW, float rotX, float rotY, float rotZ,
            float scaleX, float scaleY, float scaleZ)
        {
            this.frame = frame;
            this.jointIndex = jointIndex;
            this.posX = posX; this.posY = posY; this.posZ = posZ;
            this.rotW = rotW; this.rotX = rotX; this.rotY = rotY; this.rotZ = rotZ;
            this.scaleX = scaleX; this.scaleY = scaleY; this.scaleZ = scaleZ;
        }
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

        foreach (var entry in rInput)
        {
            Vector3 pos = new Vector3(entry.posX, entry.posY, entry.posZ);
            Quaternion rot = new Quaternion(entry.rotX, entry.rotY, entry.rotZ, entry.rotW);

            if (entry.jointIndex == 0) // R_Wrist
            {
                listR_WristPos.Add(pos);
                listR_WristQua.Add(rot);
            }
            else if (entry.jointIndex == 25) // R_ThumbTip
            {
                listR_ThumbTipQua.Add(rot);
            }
        }

        Debug.Log($"Loaded {listR_WristPos.Count} right wrist frames and {listR_ThumbTipQua.Count} right thumb tip frames.");
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

    int fc = 0;

    void Update()
    {
        if (listR_WristPos.Count == 0 || listR_ThumbTipQua.Count == 0)
            return;

        if (fc < listR_WristPos.Count)
        {
            R_Wrist.position = listR_WristPos[fc];
            R_Wrist.rotation = listR_WristQua[fc];

            if (fc < listR_ThumbTipQua.Count)
            {
                R_ThumbTip.localRotation = listR_ThumbTipQua[fc];
            }

            fc++;
        }
        else
        {
            fc = 0;
        }
    }
}
