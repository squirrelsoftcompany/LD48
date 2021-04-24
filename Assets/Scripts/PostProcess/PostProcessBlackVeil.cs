using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class PostProcessBlackVeil : MonoBehaviour
{
    // TODO : Use the "global" depth value instead of an ugly script value
    public float mDepth = 0;    
    public PostProcessProfile mPPPBlackVeil;

    public void Update()
    {
        Vignette lVignette;
        ColorGrading lColorGrading;
        if (mPPPBlackVeil.TryGetSettings(out lVignette))
        {
            lVignette.intensity.value = Mathf.Clamp(mDepth / 500.0f, 0.1f, 0.6f);
        }

        if (mPPPBlackVeil.TryGetSettings(out lColorGrading))
        {
            float lIntensity = Mathf.Clamp(1.0f-(mDepth / 500.0f), 0.5f, 1.0f);
            lColorGrading.colorFilter.value = new Color(lIntensity, lIntensity, lIntensity);
        }
    }
}
