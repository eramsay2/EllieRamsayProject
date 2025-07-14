using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("New Scene Build Number")]
    [SerializeField] private int NewSceneNumber;

    [Header("Debugging")]
    [SerializeField] private bool Load = false;

    private void Update()
    {
        if (Load)
        {
            Load = false;
            LoadScene();
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(NewSceneNumber);
    }
}
