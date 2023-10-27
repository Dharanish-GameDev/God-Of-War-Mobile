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
        //InitSettings();

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
    public void SetMusicVolume(float volume) => MusicVolume = volume;

    public void SetSFXVolume(float volume) => SFXVolume = volume;

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
        CameraRotationSpeed.x = 5;
        CameraRotationSpeed.y = 5;
        AimCameraRotationSpeed.x = 5;
        AimCameraRotationSpeed.y = 5;
        InvertHorizontalRotation = false;
        InvertVerticalRotation = false;
    }
}
