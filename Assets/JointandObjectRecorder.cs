using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class JointandObjectPositionRecorder : MonoBehaviour
{
    private TextMeshProUGUI tmpText;

    public GameObject[] joint = new GameObject[59]; // Joints (hands)
    public GameObject[] objectsToTrack; // Interactive objects (e.g., cups, tools)

    private List<string> recordedData = new List<string>();
    private bool isRecording = false;
    private int frameCount = 0;

    void Start()
    {
        Debug.Log("JointandObjectPositionRecorder initialized.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRecording = !isRecording;

            if (isRecording)
            {
                recordedData.Clear();

                // Add header
                string header = "FrameIndex,ObjectType,ObjectName,PosX,PosY,PosZ,RotW,RotX,RotY,RotZ,ScaleX,ScaleY,ScaleZ";
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
            // Record joints
            for (int i = 0; i < joint.Length; i++)
            {
                RecordTransform(frameCount, "Joint", "Joint_" + i, joint[i].transform);
            }

            // Record tracked objects
            foreach (GameObject obj in objectsToTrack)
            {
                if (obj != null)
                    RecordTransform(frameCount, "Object", obj.name, obj.transform);
            }

            frameCount++;
        }
    }

    void RecordTransform(int frame, string objectType, string name, Transform t)
    {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;
        Vector3 scale = t.localScale;

        string line = frame + "," + objectType + "," + name + "," +
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

    void SaveDataToFile()
    {
        string filePath = Application.persistentDataPath + "/RecordedData.csv";
        File.WriteAllLines(filePath, recordedData);
        Debug.Log("Data saved to: " + filePath);
        Application.OpenURL("file://" + filePath);
    }
}
