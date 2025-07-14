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
        if (Load) //Used for debugging in the editor. Tick the Load box in the inspector and it will load that scene
        {
            Load = false;
            LoadScene();
        }
    }
    public void LoadScene()
    {
        //Loads the scene
        SceneManager.LoadScene(NewSceneNumber);
    }
}
