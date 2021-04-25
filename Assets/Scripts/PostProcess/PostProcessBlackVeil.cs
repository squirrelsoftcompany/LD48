using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class PostProcessBlackVeil : MonoBehaviour {
    // Use the "global" depth value
    [SerializeField] private GameEvent depthEvent;
    [SerializeField] private PlayerData playerData;
    public PostProcessProfile mPPPBlackVeil;

    public void Update() {
        Vignette lVignette;
        ColorGrading lColorGrading;
        if (mPPPBlackVeil.TryGetSettings(out lVignette)) {
            lVignette.intensity.value = Mathf.Clamp(depthEvent.sentFloat / playerData.maxDepth, 0.1f, 0.6f);
        }

        if (mPPPBlackVeil.TryGetSettings(out lColorGrading)) {
            float lIntensity = Mathf.Clamp(1.0f - (depthEvent.sentFloat / playerData.maxDepth), 0.5f, 1.0f);
            lColorGrading.colorFilter.value = new Color(lIntensity, lIntensity, lIntensity);
        }
    }
}