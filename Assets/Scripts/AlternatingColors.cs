using UnityEngine;

public class AlternatingColors : MonoBehaviour
{
    public Color[] alternatingColors; // Array of 15 colors
    private Renderer[] subObjectRenderers;

    void Start()
    {
        // Fetch all Renderers in the sub-objects
        subObjectRenderers = GetComponentsInChildren<Renderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        for (int i = 0; i < subObjectRenderers.Length; i++)
        {
            // subObjectRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Disable shadow casting
            // subObjectRenderers[i].receiveShadows = false; // Disable receiving shadows
            // subObjectRenderers[i].motionVectorGenerationMode = MotionVectorGenerationMode.Camera; // Disable object motion vectors
            // subObjectRenderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off; // Disable light probes
            // subObjectRenderers[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off; // Disable reflection probes
            // Calculate the alternating color
            Color color = alternatingColors[i % alternatingColors.Length];
            
            // Assign the color via MaterialPropertyBlock
            propertyBlock.SetColor("_BaseColor", color); // Use "_BaseColor" or your shader's color property
            subObjectRenderers[i].SetPropertyBlock(propertyBlock);
        }
    }
}
