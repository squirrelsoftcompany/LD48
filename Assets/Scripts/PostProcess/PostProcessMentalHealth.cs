using System.Collections;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessMentalHealth : MonoBehaviour
{
    // Use the "global" depth value
    [SerializeField] private GameEvent mentalHealthEvent;
    [SerializeField] private PlayerData playerData;
    public VolumeProfile mVPMentalHealth;
    public float mGammaDistorionShift = 40;
    public float mLensShift = 0.3f;
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

        LiftGammaGain lLiftGammaGain;
        LensDistortion lLensDistortion;
        float lMentalHealth = mentalHealthEvent.sentFloat / playerData.maxHealth;

        if (mVPMentalHealth.TryGet(out lLiftGammaGain))
        {
            // Change intensity between -mLensShift and mLensShift
            float lGammaShift = ((lShift * mLensShift - 1.0f) * mLensShift) * (1.0f - lMentalHealth);
            lGammaShift = Mathf.Clamp(lGammaShift, -mLensShift, mLensShift);
            lLiftGammaGain.gamma.value = new Vector4(1.0f, 1.0f, 1.0f, lGammaShift);
        }
        if (mVPMentalHealth.TryGet(out lLensDistortion))
        { // Change intensity between -mGammaDistorionShift and +mGammaDistorionShift
            float lIntensityShift = (lShift* 2.0f * mGammaDistorionShift - mGammaDistorionShift) * (1.0f - lMentalHealth);
            lIntensityShift = lIntensityShift / 100.0f; // URP PP use [-1;1] range
            lIntensityShift = Mathf.Clamp(lIntensityShift, -mGammaDistorionShift, mGammaDistorionShift);
            lLensDistortion.intensity.value = lIntensityShift;
        }
    }
}