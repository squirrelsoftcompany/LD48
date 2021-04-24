using UnityEngine;

[ExecuteInEditMode]
public class PostProcessBlackVeil : MonoBehaviour
{

    public Material mMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mMaterial);
    }

    void Update()
    {
        //Temporary example of dynamic change of the blackout effect as a function of depth (y axis of the camera) with temporary magic number values
        float lBlackVeilScale = Mathf.Clamp(1+(transform.position.y / 500.0f), 0.6f, 1.0f);
        mMaterial.SetFloat("_VRadius", lBlackVeilScale);
    }
}
