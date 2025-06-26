using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectPlayback : MonoBehaviour
{
    [Tooltip("The CSV file name inside the StreamingAssets folder, e.g. RecordedData.csv")]
    public string csvFileName = "RecordedData.csv";

    public GameObject[] objectsToAnimate;

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
        StartCoroutine(LoadCSVFromStreamingAssets());
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

    System.Collections.IEnumerator LoadCSVFromStreamingAssets()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFileName);

        string fileContents = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        // Android requires UnityWebRequest or WWW to access StreamingAssets
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load CSV file: " + www.error);
                yield break;
            }
            fileContents = www.downloadHandler.text;
        }
#else
        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV file not found at: " + filePath);
            yield break;
        }

        fileContents = File.ReadAllText(filePath);
#endif

        string[] lines = fileContents.Split('\n');

        for (int i = 1; i < lines.Length; i++) // skip header
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
                float.Parse(parts[6])
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
        isPlaying = true;
    }
}
