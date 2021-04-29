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
            float lClampedValue = Mathf.Max(depthEvent.sentFloat / playerData.maxDepth, 0.3f);
            float lIntensityRange = Mathf.InverseLerp(0.0f, 1.0f, lClampedValue);
            lVignette.intensity.value = Mathf.InverseLerp(0.0f, 0.6f, lIntensityRange);
        }

        if (mVPBlackVeil.TryGet(out lColorAdjustments)) {
            float lClampedValue = Mathf.Max(depthEvent.sentFloat / playerData.maxDepth, 0.5f);
            float lColorRange = Mathf.InverseLerp(0.0f, 1.0f, lClampedValue);
            lVignette.intensity.value = Mathf.InverseLerp(0.5f, 1.0f, lColorRange);
        }
    }
}