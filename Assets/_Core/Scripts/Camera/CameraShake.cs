using UnityEngine;
using Cinemachine;

/// <summary>
/// Handles camera shake using cinemachine.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera followVCam;
    [SerializeField] private NoiseSettings noiseSettings;
    [SerializeField] private float frequency = 5.0f;

    // Private Variables
    private CinemachineBasicMultiChannelPerlin cinemachineNoise;
    private NoiseSettings defaultNoiseSettings;
    private float timer, totatShakeTime;
    private float startIntensity;

    private void Awake()
    {
        Instance = this;
        cinemachineNoise = followVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        defaultNoiseSettings = cinemachineNoise.m_NoiseProfile;
    }

    private void Update()
    {
        if (timer <= 0) return;

        // reduce the camera shake slowly
        timer -= Time.deltaTime;
        cinemachineNoise.m_AmplitudeGain = Mathf.Lerp(startIntensity, 1f, 1 - (timer / totatShakeTime));

        // reset to default noise
        if (timer <= 0)
        {
            cinemachineNoise.m_NoiseProfile = defaultNoiseSettings;
            cinemachineNoise.m_FrequencyGain = 1;
            cinemachineNoise.m_AmplitudeGain = 1;
        }
    }

    // Public Methods
    public void StartShake(float intensity, float time)
    {
        cinemachineNoise.m_NoiseProfile = noiseSettings;
        cinemachineNoise.m_FrequencyGain = frequency;
        cinemachineNoise.m_AmplitudeGain = intensity;

        timer = time;
        totatShakeTime = time;
        startIntensity = intensity;
    }
}
