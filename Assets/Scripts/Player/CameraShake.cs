using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    public AnimationCurve shakeCurve;

    CinemachineVirtualCamera vCam;
    CinemachineBasicMultiChannelPerlin noisePerlin;

    bool isShaking = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This will prevent the object from being destroyed when loading new scenes
        }
        else
        {
            Destroy(gameObject); // If there's already an instance, destroy this one
        }
        vCam = GetComponent<CinemachineVirtualCamera>();
        noisePerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float hitAmplitudeGain, float hitFrequencyGain, float shakeDuration)
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine(hitAmplitudeGain, hitFrequencyGain, shakeDuration));
        }
    }

    IEnumerator ShakeCoroutine(float hitAmplitudeGain, float hitFrequencyGain, float shakeDuration)
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Calculate shake intensity using the curve
            float shakeIntensity = shakeCurve.Evaluate(elapsedTime / shakeDuration);

            noisePerlin.m_AmplitudeGain = hitAmplitudeGain * shakeIntensity;
            noisePerlin.m_FrequencyGain = hitFrequencyGain * shakeIntensity;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset camera shake
        noisePerlin.m_AmplitudeGain = 0f;
        noisePerlin.m_FrequencyGain = 0f;
        isShaking = false;
    }
}
