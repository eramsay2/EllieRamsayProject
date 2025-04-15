using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class PositionRecorder : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    public GameObject[] joint = new GameObject[59]; // Updated array size to 59
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
                string header = "Joint"; // First column will contain "Joint" label
                // Add frame labels as columns (e.g., Frame1, Frame2, ...)
                for (int i = 0; i < joint.Length; i++)
                {
                    header += ",Frame" + (frameCount + 1); // One column per frame
                }
                recordedData.Add(header); // Add header row
                //tmpText.text = "Recording Started";
            }
            else
            {
                SaveDataToFile();
                //tmpText.text = "Recording Stopped. Data Saved.";
            }
        }
        
        if (isRecording)
        {
            // Create data for each joint for the current frame
            string data = "";
            for (int i = 0; i < joint.Length; i++)
            {
                // Record the joint's name and its position in that frame
                data += "Joint" + i + "_X," + joint[i].transform.position.x.ToString("0.000");
                data += "," + joint[i].transform.position.y.ToString("0.000");
                data += "," + joint[i].transform.position.z.ToString("0.000");
                recordedData.Add(data);
            }

            //tmpText.text = "Recording Frame: " + frameCount;
            frameCount++;
        }
    }

    void SaveDataToFile()
    {
        string filePath = Application.persistentDataPath + "/RecordedData.csv";
        File.WriteAllLines(filePath, recordedData);
        Debug.Log("Data saved to: " + filePath);
        Application.OpenURL("file://" + filePath); // Automatically open file after saving
    }
}