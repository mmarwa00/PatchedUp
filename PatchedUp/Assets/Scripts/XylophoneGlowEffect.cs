using System.Collections;
using UnityEngine;

public class XylophoneGlowEffect : MonoBehaviour
{
    [SerializeField] private Renderer keyRenderer;
    [SerializeField] private Color glowColor = new Color(1f, 0.9f, 0.3f);
    [SerializeField] private float glowIntensity = 3f;
    [SerializeField] private float glowDuration = 0.5f;

    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private MaterialPropertyBlock propBlock;
    private Coroutine activeGlow;

    void Awake()
    {
        if (keyRenderer == null) keyRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        
        keyRenderer.sharedMaterial.EnableKeyword("_EMISSION");
    }

    public void PlayGlow()
    {
        if (activeGlow != null) StopCoroutine(activeGlow);
        activeGlow = StartCoroutine(GlowRoutine());
    }

    private IEnumerator GlowRoutine()
    {
        float elapsed = 0f;
        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / glowDuration;
            
            float intensity = Mathf.Lerp(glowIntensity, 0f, t * t);
            
            keyRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColorId, glowColor * intensity);
            keyRenderer.SetPropertyBlock(propBlock);
            
            yield return null;
        }
        
        keyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(EmissionColorId, Color.black);
        keyRenderer.SetPropertyBlock(propBlock);
        
        activeGlow = null;
    }
}