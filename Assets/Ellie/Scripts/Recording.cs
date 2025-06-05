using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class PositionRecorder : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    public GameObject[] joint = new GameObject[59]; // 59 joints
    private List<string> recordedData = new List<string>();
    private bool isRecording = false;
    private int frameCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRecording = !isRecording;

            if (isRecording)
            {
                recordedData.Clear();

                // Proper header with 'FrameIndex'
                string header = "FrameIndex,JointIndex,PosX,PosY,PosZ,RotW,RotX,RotY,RotZ,ScaleX,ScaleY,ScaleZ";
                recordedData.Add(header);

                frameCount = 0;
            }
            else
            {
                SaveDataToFile();
                Debug.Log("Recording stopped.");
            }
        }

        if (isRecording)
        {
            for (int i = 0; i < joint.Length; i++)
            {
                Transform t = joint[i].transform;

                Vector3 pos = t.position;
                Quaternion rot = t.rotation;
                Vector3 scale = t.localScale;

                string line = frameCount + "," + i + "," +
                              pos.x.ToString("F5") + "," +
                              pos.y.ToString("F5") + "," +
                              pos.z.ToString("F5") + "," +
                              rot.w.ToString("F5") + "," +
                              rot.x.ToString("F5") + "," +
                              rot.y.ToString("F5") + "," +
                              rot.z.ToString("F5") + "," +
                              scale.x.ToString("F5") + "," +
                              scale.y.ToString("F5") + "," +
                              scale.z.ToString("F5");

                recordedData.Add(line);
            }

            // Increment frame count once per full joint set
            frameCount++;
        }
    }

    void SaveDataToFile()
    {
        string filePath = Application.persistentDataPath + "/RecordedData.csv";
        File.WriteAllLines(filePath, recordedData);
        Debug.Log("Data saved to: " + filePath);
        Application.OpenURL("file://" + filePath);
    }
}