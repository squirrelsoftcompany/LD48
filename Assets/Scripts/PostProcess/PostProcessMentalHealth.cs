using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessMentalHealth : MonoBehaviour
{
    // Use the "global" depth value
    [SerializeField] private GameEvent mentalHealthEvent;
    [SerializeField] private PlayerData playerData;
    public PostProcessProfile mPPPMentalHealth;
    private float mCurrentTimerValue = 0.0f;
    private bool mShiftUp= true;
    private float mMaxTimerValue = 2.0f;

    public void Update()
    {
        if (mCurrentTimerValue < mMaxTimerValue)
        {
            mCurrentTimerValue += Time.deltaTime;
            mCurrentTimerValue = Mathf.Clamp(mCurrentTimerValue, 0.0f, mMaxTimerValue);
        }
        else
        {
            mCurrentTimerValue = 0.0f;
            mShiftUp = !mShiftUp;
        }
        //Shift move between 0.0f and 1.0f
        float lShift;
        if (mShiftUp)
        {
            lShift = Mathf.InverseLerp(0.0f, mMaxTimerValue, mCurrentTimerValue);
        }
        else
        {
            lShift = Mathf.InverseLerp(mMaxTimerValue, 0.0f, mCurrentTimerValue);
        }

        ColorGrading lColorGrading;
        LensDistortion lLensDistortion;

        if (mPPPMentalHealth.TryGetSettings(out lColorGrading))
        {
            float lMentalHealth = mentalHealthEvent.sentFloat / playerData.maxHealth;
            float lGammaShift = (lShift* 2.0f * 0.2f - 1.0f) * (1.0f - lMentalHealth);
            // Change intensity between -0.2f and 0.2f 
            lGammaShift = Mathf.Clamp(lGammaShift, -0.2f, 0.2f);
            lColorGrading.gamma.value = new Vector4(1.0f, 1.0f, 1.0f, lGammaShift);
        }
        if (mPPPMentalHealth.TryGetSettings(out lLensDistortion))
        {
            float lMentalHealth = mentalHealthEvent.sentFloat / playerData.maxHealth;
            // Change intensity between -60 and +60 
            float lIntensityShift = (lShift* 2.0f * 60.0f - 60.0f) * (1.0f - lMentalHealth);
            lIntensityShift = Mathf.Clamp(lIntensityShift, -60.0f, 60.0f);
            lLensDistortion.intensity.value = lIntensityShift;
        }
    }
}