using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    public Material glowMaterial;
    public float pulseSpeed = 2f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;

    private float originalIntensity;

    void Start()
    {
        if (glowMaterial != null)
        {
            originalIntensity = glowMaterial.GetFloat("_EmissionIntensity");
        }
    }

    void Update()
    {
        if (glowMaterial != null)
        {
            float intensity = Mathf.Lerp(minIntensity, maxIntensity,
                                        (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

            glowMaterial.SetFloat("_EmissionIntensity", intensity);
        }
    }

    void OnDisable()
    {
        if (glowMaterial != null)
        {
            glowMaterial.SetFloat("_EmissionIntensity", originalIntensity);
        }
    }
}
