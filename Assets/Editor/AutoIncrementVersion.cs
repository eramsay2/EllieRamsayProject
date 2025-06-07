using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AutoIncrementVersion : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.Android)
        {
            int currentVersionCode = PlayerSettings.Android.bundleVersionCode;
            int newVersionCode = currentVersionCode + 1;

            PlayerSettings.Android.bundleVersionCode = newVersionCode;

            Debug.Log($"[AutoIncrementVersion] Incremented Bundle Version Code to: {newVersionCode}");
        }
    }
}
