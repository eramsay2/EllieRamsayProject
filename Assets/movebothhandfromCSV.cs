using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class moveLeftHandFromCSV : MonoBehaviour
{
    public Transform L_Wrist;
    public Transform L_ThumbTip;
    public string path = @"C:\Temp\@Data\RecordedData.csv";  // Update this if needed

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

    List<sTransform> ReadTransformsFromCSV(string filePath)
    {
        List<sTransform> transformList = new List<sTransform>();
        var lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("frame"))
                continue;

            string[] parts = line.Split(',');

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
                catch (System.FormatException ex)
                {
                    Debug.LogWarning($"Skipping line due to format error: {line}\n{ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Skipping malformed line: {line}");
            }
        }

        return transformList;
    }

    void AssignFrames(ref List<sTransform> transforms)
    {
        int currentFrame = 0;

        for (int i = 0; i < transforms.Count; i++)
        {
            if (i > 0 && transforms[i].jointIndex == 0)
            {
                currentFrame++;
            }

            var t = transforms[i];
            t.frame = currentFrame;
            transforms[i] = t;
        }
    }

    void Start()
    {
        lInput = ReadTransformsFromCSV(path);
        AssignFrames(ref lInput);

        foreach (var entry in lInput)
        {
            Vector3 pos = new Vector3(entry.posX, entry.posY, entry.posZ);
            Quaternion rot = new Quaternion(entry.rotX, entry.rotY, entry.rotZ, entry.rotW);

            if (entry.jointIndex == 26)
            {
                listL_WristPos.Add(pos);
                listL_WristQua.Add(rot);
            }
            else if (entry.jointIndex == 51)
            {
                listL_ThumbTipQua.Add(rot);
            }
        }

        Debug.Log($"Loaded {listL_WristPos.Count} wrist frames and {listL_ThumbTipQua.Count} thumb tip frames.");
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
        }
        else
        {
            fc = 0;
        }

        fc++;
    }
}