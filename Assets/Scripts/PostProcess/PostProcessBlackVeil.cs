using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class PostProcessBlackVeil : MonoBehaviour {
    // Use the "global" depth value
    [SerializeField] private GameEvent depthEvent;
    [SerializeField] private PlayerData playerData;
    public VolumeProfile mVPBlackVeil;

    public void Update() {
        Vignette lVignette;
        ColorAdjustments lColorAdjustments;
        if (mVPBlackVeil.TryGet(out lVignette)) {
            lVignette.intensity.value = Mathf.Clamp(depthEvent.sentFloat / playerData.maxDepth, 0.1f, 0.6f);
        }

        if (mVPBlackVeil.TryGet(out lColorAdjustments)) {
            float lIntensity = Mathf.Clamp(1.0f - (depthEvent.sentFloat / playerData.maxDepth), 0.5f, 1.0f);
            lColorAdjustments.colorFilter.value = new Color(lIntensity, lIntensity, lIntensity);
        }
    }
}