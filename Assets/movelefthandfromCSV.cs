using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class moveLeftHandFromCSV : MonoBehaviour
{
    public Transform L_Wrist;
    public Transform L_ThumbTip;
    public string fileName = "RecordedData.csv";

    List<sTransform> lInput = new List<sTransform>();

    List<Vector3> listL_WristPos = new List<Vector3>();
    List<Quaternion> listL_WristQua = new List<Quaternion>();
    List<Quaternion> listL_ThumbTipQua = new List<Quaternion>();

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

        lInput = ParseCSV(csvText);
        AssignFrames(ref lInput);

        foreach (var entry in lInput)
        {
            Vector3 pos = new Vector3(entry.posX, entry.posY, entry.posZ);
            Quaternion rot = new Quaternion(entry.rotX, entry.rotY, entry.rotZ, entry.rotW);

            if (entry.jointIndex == 26) // L_Wrist
            {
                listL_WristPos.Add(pos);
                listL_WristQua.Add(rot);
            }
            else if (entry.jointIndex == 51) // L_ThumbTip
            {
                listL_ThumbTipQua.Add(rot);
            }
        }

        Debug.Log($"Loaded {listL_WristPos.Count} left wrist frames and {listL_ThumbTipQua.Count} left thumb tip frames.");
    }

    List<sTransform> ParseCSV(string csvText)
    {
        List<sTransform> transformList = new List<sTransform>();
        var lines = csvText.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("frame"))
                continue;

            var parts = line.Trim().Split(',');
            if (parts.Length == 12)
            {
                try
                {
                    sTransform sCol = new sTransform
                    {
                        frame = int.Parse(parts[0]),
                        jointIndex = int.Parse(parts[1]),
                        posX = float.Parse(parts[2]),
                        posY = float.Parse(parts[3]),
                        posZ = float.Parse(parts[4]),
                        rotW = float.Parse(parts[5]),
                        rotX = float.Parse(parts[6]),
                        rotY = float.Parse(parts[7]),
                        rotZ = float.Parse(parts[8]),
                        scaleX = float.Parse(parts[9]),
                        scaleY = float.Parse(parts[10]),
                        scaleZ = float.Parse(parts[11])
                    };
                    transformList.Add(sCol);
                }
                catch
                {
                    Debug.LogWarning("Bad line skipped: " + line);
                }
            }
        }

        return transformList;
    }

    void AssignFrames(ref List<sTransform> transforms)
    {
        int currentFrame = 0;
        for (int i = 0; i < transforms.Count; i++)
        {
            if (i > 0 && transforms[i].jointIndex == 26) // frame starts at L_Wrist
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
        if (listL_WristPos.Count == 0 || listL_ThumbTipQua.Count == 0)
            return;

        if (fc < listL_WristPos.Count)
        {
            L_Wrist.position = listL_WristPos[fc];
            L_Wrist.rotation = listL_WristQua[fc];

            if (fc < listL_ThumbTipQua.Count)
            {
                L_ThumbTip.localRotation = listL_ThumbTipQua[fc];
            }

            fc++;
        }
        else
        {
            fc = 0;
        }
    }
}
