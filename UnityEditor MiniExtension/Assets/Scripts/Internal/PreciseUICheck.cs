using UnityEngine;

[AddComponentMenu("Event/Precise UI Check"), ExecuteInEditMode]
public class PreciseUICheck : MonoBehaviour 
{
    [Range(0, 1), Tooltip("Below that value of alpha this component won't react to raycast.")]
    public float AlphaThreshold = .9f;
    [Tooltip("Include material tint color when checking alpha.")]
    public bool IncludeMaterialAlpha;

    private void OnEnable()
    {
        if (!FindObjectOfType<PreciseUI>())
        {
            var canvas = FindObjectOfType<Canvas>();
            if (!canvas) return;

            var alphaCaster = canvas.gameObject.AddComponent<PreciseUI>();
            alphaCaster.SelectiveMode = true;
        }
    }
}
