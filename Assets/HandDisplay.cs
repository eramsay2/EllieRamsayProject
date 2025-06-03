using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionReplayer : MonoBehaviour
{
    [Tooltip("Name of the CSV file to load from persistent data path (e.g., RecordedData.csv)")]
    public string csvFileName = "RecordedData.csv";

    public GameObject[] joint = new GameObject[59]; // Joints to apply transforms to
    public float playbackSpeed = 0.033f; // Playback speed (approx. 30 FPS)

    private List<FrameData> frames = new List<FrameData>();
    private bool isPlaying = false;
    private int currentFrame = 0;

    private class FrameData
    {
        public Dictionary<int, TransformData> jointData = new Dictionary<int, TransformData>();
    }

    private class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    void Start()
    {
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        string fullPath = Path.Combine(Application.persistentDataPath, csvFileName);
        LoadCSV(fullPath);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isPlaying = !isPlaying;
            if (isPlaying)
            {
                currentFrame = 0;
                StartCoroutine(Replay());
            }
            else
            {
                StopCoroutine(Replay());
            }
        }
    }

    IEnumerator Replay()
    {
        while (currentFrame < frames.Count)
        {
            FrameData frame = frames[currentFrame];

            foreach (var kvp in frame.jointData)
            {
                int jointIndex = kvp.Key;
                TransformData data = kvp.Value;

                if (jointIndex >= 0 && jointIndex < joint.Length && joint[jointIndex] != null)
                {
                    Transform t = joint[jointIndex].transform;
                    t.position = data.position;
                    t.rotation = data.rotation;
                    t.localScale = data.scale;
                }
            }

            currentFrame++;
            yield return new WaitForSeconds(playbackSpeed);
        }

        isPlaying = false;
    }

    void LoadCSV(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("CSV file not found at: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        frames.Clear();

        Dictionary<int, FrameData> tempFrames = new Dictionary<int, FrameData>();

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string[] tokens = lines[i].Split(',');

            if (tokens.Length < 12) continue; // Skip malformed lines

            int frameNumber = int.Parse(tokens[0]);
            int jointIndex = int.Parse(tokens[1]);

            Vector3 pos = new Vector3(
                float.Parse(tokens[2]),
                float.Parse(tokens[3]),
                float.Parse(tokens[4]));

            Quaternion rot = new Quaternion(
                float.Parse(tokens[6]),
                float.Parse(tokens[7]),
                float.Parse(tokens[8]),
                float.Parse(tokens[5])); // w, x, y, z

            Vector3 scale = new Vector3(
                float.Parse(tokens[9]),
                float.Parse(tokens[10]),
                float.Parse(tokens[11]));

            if (!tempFrames.ContainsKey(frameNumber))
                tempFrames[frameNumber] = new FrameData();

            tempFrames[frameNumber].jointData[jointIndex] = new TransformData
            {
                position = pos,
                rotation = rot,
                scale = scale
            };
        }

        for (int i = 0; i < tempFrames.Count; i++)
        {
            if (tempFrames.ContainsKey(i))
                frames.Add(tempFrames[i]);
        }

        Debug.Log("Loaded " + frames.Count + " frames from " + filePath);
    }
}