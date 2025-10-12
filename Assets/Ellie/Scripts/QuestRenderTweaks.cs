using UnityEngine;
using UnityEngine.XR;

public class QuestRenderTweaks : MonoBehaviour
{
    [SerializeField] private float RenderScale = 1.2f;
    void Start()
    {
        // Increase render scale for sharper edges
        XRSettings.eyeTextureResolutionScale = RenderScale;
    }
}
