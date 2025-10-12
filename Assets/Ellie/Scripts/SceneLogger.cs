using UnityEngine;

public class SceneLogger : MonoBehaviour
{
    public string sectionName;

    void Start()
    {
        if (ProgressLoggingManager.Instance != null)
        {
            ProgressLoggingManager.Instance.EnterSection(sectionName);
        }
    }
}
