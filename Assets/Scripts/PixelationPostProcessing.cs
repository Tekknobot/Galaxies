using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PixelationPostProcessing : MonoBehaviour
{
    public Shader pixelationShader;
    private Material pixelationMaterial;

    public float pixelSize = 10.0f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (pixelationMaterial == null)
        {
            pixelationMaterial = new Material(pixelationShader);
        }
        pixelationMaterial.SetFloat("_PixelSize", pixelSize);
        Graphics.Blit(src, dest, pixelationMaterial);
    }
}
