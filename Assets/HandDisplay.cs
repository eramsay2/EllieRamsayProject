using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class PositionReplayer : MonoBehaviour
{
    public string csvFileName = "RecordedData.csv";
    public GameObject[] joint = new GameObject[59];
    public float playbackSpeed = 0.033f;
    public GameObject handTrackingRoot;

    private List<FrameData> frames = new List<FrameData>();
    private int currentFrame = 0;
    private bool isPlaying = false;
    private Coroutine replayCoroutine;

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
        string fullPath = Path.Combine(Application.persistentDataPath, csvFileName);
        Debug.Log("[DEBUG] Loading CSV from: " + fullPath);
        LoadCSV(fullPath);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPlaying = !isPlaying;
            Debug.Log("[DEBUG] Playback toggled: " + isPlaying);

            if (isPlaying)
            {
                if (handTrackingRoot != null) handTrackingRoot.SetActive(false);
                currentFrame = 0;
                replayCoroutine = StartCoroutine(Replay());
            }
            else
            {
                if (replayCoroutine != null) StopCoroutine(replayCoroutine);
                if (handTrackingRoot != null) handTrackingRoot.SetActive(true);
            }
        }
    }

    IEnumerator Replay()
    {
        while (currentFrame < frames.Count)
        {
            FrameData frame = frames[currentFrame];
            Debug.Log($"[DEBUG] Frame {currentFrame} — Joint count: {frame.jointData.Count}");

            foreach (var kvp in frame.jointData)
            {
                int jointIndex = kvp.Key;
                TransformData data = kvp.Value;

                if (jointIndex >= 0 && jointIndex < joint.Length)
                {
                    GameObject jointObj = joint[jointIndex];

                    if (jointObj != null)
                    {
                        Transform t = jointObj.transform;
                        t.position = data.position;
                        t.rotation = data.rotation;
                        t.localScale = data.scale;

                        if (jointIndex == 0) // Focus on the first joint for debug clarity
                        {
                            Debug.Log($"[DEBUG] Joint 0 — Pos: {t.position} | Rot: {t.rotation.eulerAngles} | Scale: {t.localScale}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[WARNING] joint[{jointIndex}] is null.");
                    }
                }
                else
                {
                    Debug.LogWarning($"[WARNING] Invalid joint index: {jointIndex}");
                }
            }

            currentFrame++;
            yield return new WaitForSeconds(playbackSpeed);
        }

        isPlaying = false;
        if (handTrackingRoot != null) handTrackingRoot.SetActive(true);
        Debug.Log("[DEBUG] Playback complete.");
    }

    void LoadCSV(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("[ERROR] CSV file not found at: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        frames.Clear();
        Dictionary<int, FrameData> tempFrames = new Dictionary<int, FrameData>();

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            string[] tokens = lines[i].Split(',');
            if (tokens.Length < 12) continue;

            int frameNumber = int.Parse(tokens[0]);
            int jointIndex = int.Parse(tokens[1]);

            Vector3 pos = new Vector3(
                float.Parse(tokens[2], CultureInfo.InvariantCulture),
                float.Parse(tokens[3], CultureInfo.InvariantCulture),
                float.Parse(tokens[4], CultureInfo.InvariantCulture));

            Quaternion rot = new Quaternion(
                float.Parse(tokens[6], CultureInfo.InvariantCulture),
                float.Parse(tokens[7], CultureInfo.InvariantCulture),
                float.Parse(tokens[8], CultureInfo.InvariantCulture),
                float.Parse(tokens[5], CultureInfo.InvariantCulture)); // w, x, y, z

            Vector3 scale = new Vector3(
                float.Parse(tokens[9], CultureInfo.InvariantCulture),
                float.Parse(tokens[10], CultureInfo.InvariantCulture),
                float.Parse(tokens[11], CultureInfo.InvariantCulture));

            if (!tempFrames.ContainsKey(frameNumber))
                tempFrames[frameNumber] = new FrameData();

            tempFrames[frameNumber].jointData[jointIndex] = new TransformData
            {
                position = pos,
                rotation = rot,
                scale = scale
            };
        }

        foreach (var key in tempFrames.Keys.OrderBy(k => k))
        {
            frames.Add(tempFrames[key]);
        }

        Debug.Log($"[DEBUG] Loaded {frames.Count} frames from CSV.");
    }
}
