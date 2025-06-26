using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectPlayback : MonoBehaviour
{
    [Tooltip("The CSV file name (e.g., RecordedData.csv), placed in Application.persistentDataPath")]
    public string csvFileName = "RecordedData.csv";

    public GameObject[] objectsToAnimate; // Assign in Inspector

    private class FrameData
    {
        public int frame;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    private Dictionary<string, List<FrameData>> objectTracks = new Dictionary<string, List<FrameData>>();
    private int currentFrame = 0;
    private bool isPlaying = false;

    void Start()
    {
        LoadCSVFromFile();
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying) return;

        foreach (GameObject obj in objectsToAnimate)
        {
            if (obj == null || !objectTracks.ContainsKey(obj.name)) continue;

            List<FrameData> track = objectTracks[obj.name];

            if (currentFrame < track.Count)
            {
                FrameData frame = track[currentFrame];
                obj.transform.position = frame.position;
                obj.transform.rotation = frame.rotation;
                obj.transform.localScale = frame.scale;
            }
        }

        currentFrame++;
    }

    void LoadCSVFromFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, csvFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV file not found: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] parts = line.Split(',');
            if (parts.Length < 13) continue;

            string objectType = parts[1];
            string objectName = parts[2];

            if (objectType != "Object") continue;

            int frame = int.Parse(parts[0]);

            Vector3 pos = new Vector3(
                float.Parse(parts[3]),
                float.Parse(parts[4]),
                float.Parse(parts[5])
            );

            Quaternion rot = new Quaternion(
                float.Parse(parts[7]),
                float.Parse(parts[8]),
                float.Parse(parts[9]),
                float.Parse(parts[6]) // w is stored first
            );

            Vector3 scale = new Vector3(
                float.Parse(parts[10]),
                float.Parse(parts[11]),
                float.Parse(parts[12])
            );

            if (!objectTracks.ContainsKey(objectName))
                objectTracks[objectName] = new List<FrameData>();

            objectTracks[objectName].Add(new FrameData
            {
                frame = frame,
                position = pos,
                rotation = rot,
                scale = scale
            });
        }

        Debug.Log("Loaded object animation data for " + objectTracks.Count + " objects from: " + filePath);
    }
}
