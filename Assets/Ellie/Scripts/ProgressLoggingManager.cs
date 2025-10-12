using UnityEngine;
using System;
using System.IO;

public class ProgressLoggingManager : MonoBehaviour
{
    public static ProgressLoggingManager Instance;

    private string currentSection;
    private float sectionStartTime;
    private string logDirectory;
    private string logFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create /logs directory inside app's persistent data path
            logDirectory = Path.Combine(Application.persistentDataPath, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Create a new log file per session, timestamped for uniqueness
            string dateStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"ProgressLog_{dateStamp}.csv";
            logFilePath = Path.Combine(logDirectory, fileName);

            // Create file and write header
            File.WriteAllText(logFilePath, "Time, Scenario, TimeSpentSeconds\n");

            Debug.Log($"[ProgressLogging] Started new log file: {logFilePath}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterSection(string sectionName)
    {
        // Record previous section duration
        if (!string.IsNullOrEmpty(currentSection))
        {
            float duration = Time.time - sectionStartTime;
            LogSectionTime(currentSection, duration);
        }

        // Start tracking new section
        currentSection = sectionName;
        sectionStartTime = Time.time;
        Debug.Log($"[ProgressLogging] Entered section: {sectionName}");
    }

    private void LogSectionTime(string section, float duration)
    {
        string time = DateTime.Now.ToString("HH:mm:ss");
        string line = $"{time}, {section}, {duration:F1}\n";

        try
        {
            File.AppendAllText(logFilePath, line);
            Debug.Log($"[ProgressLogging] Logged: {line.Trim()}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ProgressLogging] Failed to write log: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        if (!string.IsNullOrEmpty(currentSection))
        {
            float duration = Time.time - sectionStartTime;
            LogSectionTime(currentSection, duration);
        }

        Debug.Log($"[ProgressLogging] Session ended. Log saved to {logFilePath}");
    }
}
