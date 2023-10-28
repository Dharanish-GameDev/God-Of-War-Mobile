using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls and provide the essential gamedata presist through scenes.
/// </summary>
public class GameCtrl : MonoBehaviour
{
    public static GameCtrl Instance { get; private set; }

    public float MusicVolume;
    public float SFXVolume;
    public Vector2Int CameraRotationSpeed;
    public Vector2Int AimCameraRotationSpeed;
    public Vector2 CameraRotationSpeedValue;
    public Vector2 AimCameraRotationSpeedValue;
    public bool InvertHorizontalRotation;
    public bool InvertVerticalRotation;
    
    // Private Variables
    private float initialTimeScale, initialFixedDeltaTime;
    private float slowMotionTimer;

    private void Awake()
    {
        // create singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);

        // initialize settings
        InitSettings();

        initialTimeScale = Time.timeScale;
        initialFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (slowMotionTimer <= 0) return;

        slowMotionTimer -= Time.deltaTime;
        if (slowMotionTimer <= 0) StopSlowMotion();
    }

    // Public Methods
    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        AudioListener.volume = MusicVolume;

        // handle bg audio
        if (MusicVolume <= 0 && AudioManager.Instance.BGAudioSource.isPlaying) AudioManager.Instance.BGAudioSource.Stop();
        else if (MusicVolume > 0 && !AudioManager.Instance.BGAudioSource.isPlaying) AudioManager.Instance.BGAudioSource.Play();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        AudioManager.Instance.BtnClickSource.volume = Mathf.Lerp(0.0f, 0.1f, Mathf.InverseLerp(0.0f, 1.0f, SFXVolume));
    }

    public void StartSlowMotion(float slowTimeFactor, float duration = 0.0f)
    {
        slowMotionTimer = duration;
        Time.timeScale = slowTimeFactor;
        Time.fixedDeltaTime = initialFixedDeltaTime * Time.timeScale;
    }

    public void StopSlowMotion()
    {
        Time.timeScale = initialTimeScale;
        Time.fixedDeltaTime = initialFixedDeltaTime;
    }

    // Private Methods
    private void InitSettings()
    {
        // audio
        MusicVolume = 1.0f;
        SFXVolume = 1.0f;

        // camera
        CameraRotationSpeed.x = 4;
        CameraRotationSpeed.y = 4;
        AimCameraRotationSpeed.x = 5;
        AimCameraRotationSpeed.y = 5;
        InvertHorizontalRotation = false;
        InvertVerticalRotation = false;

        CameraRotationSpeedValue.x = CameraRotationSpeed.x * (InvertHorizontalRotation ? -1 : 1);
        CameraRotationSpeedValue.y = CameraRotationSpeed.y * (InvertVerticalRotation ? -1 : 1);
        AimCameraRotationSpeedValue.x = Mathf.Lerp(0.1f, 1.0f, Mathf.InverseLerp(1, 10, AimCameraRotationSpeed.x));
        AimCameraRotationSpeedValue.y = Mathf.Lerp(0.1f, 1.0f, Mathf.InverseLerp(1, 10, AimCameraRotationSpeed.y));
    }
}
