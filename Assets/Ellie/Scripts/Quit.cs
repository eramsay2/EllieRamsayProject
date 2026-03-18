using UnityEngine;

public class Quit : MonoBehaviour
{
    public void QuitGame()
    {
        if (ProgressLoggingManager.Instance != null)
        {
            ProgressLoggingManager.Instance.EnterSection("Quit");

            Application.Quit();
        }
    }
}
