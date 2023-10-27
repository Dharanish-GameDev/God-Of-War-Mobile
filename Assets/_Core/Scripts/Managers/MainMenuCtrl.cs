using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the mainmenu behaviours such as playbtn, settings menu, etc...
/// </summary>
public class MainMenuCtrl : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Button MusicIncreaseBtn, MusicDecreaseBtn;
    [SerializeField] private Button SFXIncreaseBtn, SFXDecreaseBtn;
    [SerializeField] private TMP_Text MusicText, SFXText;

    [Header("Camera Settings")]
    [SerializeField] private Slider HRSSlider;
    [SerializeField] private Slider VRSSlider;
    [SerializeField] private Button HRSIncreaseBtn, HRSDecreaseBtn;
    [SerializeField] private Button VRSIncreaseBtn, VRSDecreaseBtn;
    [SerializeField] private Button IHCOnBtn, IHCOffBtn;
    [SerializeField] private Button IVCOnBtn, IVCOffBtn;
    [SerializeField] private Color normalColor, disableColor;
    [SerializeField] private TMP_Text HRSText, VRSText, IHCText, IVCText;
    [SerializeField] private bool IsIHCOn = false;
    [SerializeField] private bool IsIVCOn = false;

    [Space(10)]
    [SerializeField] private Slider AimHRSSlider;
    [SerializeField] private Slider AimVRSSlider;
    [SerializeField] private Button AimHRSIncreaseBtn, AimHRSDecreaseBtn;
    [SerializeField] private Button AimVRSIncreaseBtn, AimVRSDecreaseBtn;
    [SerializeField] private TMP_Text AimHRSText, AimVRSText;

    private void Start()
    {
        // initialize settings
        AudioSettingsInit();
        CameraSettingsInit();
    }

    // UI Events Methods
    public void LoadScene(int sceneIndex)
    {
        ScenesCtrl.Instance.LoadScene(sceneIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Audio Settings Methods
    public void MusicSliderChange(float value)
    {
        MusicText.text = Mathf.RoundToInt(value * 100).ToString();
        MusicIncreaseBtn.interactable = value < MusicSlider.maxValue;
        MusicDecreaseBtn.interactable = value > MusicSlider.minValue;
        GameCtrl.Instance.SetMusicVolume(MusicSlider.value);
    }

    public void MusicDecreaseBtnPressed()
    {
        MusicSlider.value -= 0.1f;
        GameCtrl.Instance.SetMusicVolume(MusicSlider.value);
    }

    public void MusicIncreaseBtnPressed()
    {
        MusicSlider.value += 0.1f;
        GameCtrl.Instance.SetMusicVolume(MusicSlider.value);
    }

    public void SFXSliderChange(float value)
    {
        SFXText.text = Mathf.RoundToInt(value * 100).ToString();
        SFXIncreaseBtn.interactable = value < SFXSlider.maxValue;
        SFXDecreaseBtn.interactable = value > SFXSlider.minValue;
        GameCtrl.Instance.SetSFXVolume(SFXSlider.value);
    }

    public void SFXDecreaseBtnPressed()
    {
        SFXSlider.value -= 0.1f;
        GameCtrl.Instance.SetSFXVolume(SFXSlider.value);
    }

    public void SFXIncreaseBtnPressed()
    {
        SFXSlider.value += 0.1f;
        GameCtrl.Instance.SetSFXVolume(SFXSlider.value);
    }

    // Camera Settings
    public void HRSSliderChange(float value)
    {
        HRSText.text = Mathf.RoundToInt(value).ToString();
        HRSIncreaseBtn.interactable = value < HRSSlider.maxValue;
        HRSDecreaseBtn.interactable = value > HRSSlider.minValue;
        GameCtrl.Instance.CameraRotationSpeed.x = Mathf.RoundToInt(value);
    }

    public void HRSDecreaseBtnPressed()
    {
        HRSSlider.value -= 1;
        GameCtrl.Instance.CameraRotationSpeed.x = Mathf.RoundToInt(HRSSlider.value);
    }

    public void HRSIncreaseBtnPressed()
    {
        HRSSlider.value += 1;
        GameCtrl.Instance.CameraRotationSpeed.x = Mathf.RoundToInt(HRSSlider.value);
    }

    public void VRSSliderChange(float value)
    {
        VRSText.text = Mathf.RoundToInt(value).ToString();
        VRSIncreaseBtn.interactable = value < VRSSlider.maxValue;
        VRSDecreaseBtn.interactable = value > VRSSlider.minValue;
        GameCtrl.Instance.CameraRotationSpeed.y = Mathf.RoundToInt(value);
    }

    public void VRSDecreaseBtnPressed()
    {
        VRSSlider.value -= 1;
        GameCtrl.Instance.CameraRotationSpeed.y = Mathf.RoundToInt(VRSSlider.value);
    }

    public void VRSIncreaseBtnPressed()
    {
        VRSSlider.value += 1;
        GameCtrl.Instance.CameraRotationSpeed.y = Mathf.RoundToInt(VRSSlider.value);
    }

    public void IHCPressed()
    {
        if (!IsIHCOn)
        {
            IsIHCOn = true;
            IHCText.text = "On";
            IHCOffBtn.interactable = true;
            IHCOnBtn.interactable = false;
            GameCtrl.Instance.InvertHorizontalRotation = true;
        }
        else
        {
            IsIHCOn = false;
            IHCText.text = "Off";
            IHCOffBtn.interactable = false;
            IHCOnBtn.interactable = true;
            GameCtrl.Instance.InvertHorizontalRotation = false;
        }
    }

    public void IHCOnBtnPressed()
    {
        if (!IsIHCOn)
        {
            IsIHCOn = true;
            IHCText.text = "On";
            IHCOffBtn.interactable = true;
            IHCOnBtn.interactable = false;
            GameCtrl.Instance.InvertHorizontalRotation = true;
        }
    }

    public void IHCOffBtnPressed()
    {
        if (IsIHCOn)
        {
            IsIHCOn = false;
            IHCText.text = "Off";
            IHCOffBtn.interactable = false;
            IHCOnBtn.interactable = true;
            GameCtrl.Instance.InvertHorizontalRotation = false;
        }
    }

    public void IVCPressed()
    {
        if (!IsIVCOn)
        {
            IsIVCOn = true;
            IVCText.text = "On";
            IVCOnBtn.interactable = false;
            IVCOffBtn.interactable = true;
            GameCtrl.Instance.InvertVerticalRotation = true;
        }
        else
        {
            IsIVCOn = false;
            IVCText.text = "Off";
            IVCOnBtn.interactable = true;
            IVCOffBtn.interactable = false;
            GameCtrl.Instance.InvertVerticalRotation = false;
        }
    }

    public void IVCOnBtnPressed()
    {
        if (!IsIVCOn)
        {
            IsIVCOn = true;
            IVCText.text = "On";
            IVCOnBtn.interactable = false;
            IVCOffBtn.interactable = true;
            GameCtrl.Instance.InvertVerticalRotation = true;
        }
    }

    public void IVCOffBtnPressed()
    {
        if (IsIVCOn)
        {
            IsIVCOn = false;
            IVCText.text = "Off";
            IVCOnBtn.interactable = true;
            IVCOffBtn.interactable = false;
            GameCtrl.Instance.InvertVerticalRotation = false;
        }
    }

    public void AimHRSSliderChange(float value)
    {
        AimHRSText.text = Mathf.RoundToInt(value).ToString();
        AimHRSIncreaseBtn.interactable = value < AimHRSSlider.maxValue;
        AimHRSDecreaseBtn.interactable = value > AimHRSSlider.minValue;
        GameCtrl.Instance.AimCameraRotationSpeed.x = Mathf.RoundToInt(value);
    }

    public void AimHRSDecreaseBtnPressed()
    {
        AimHRSSlider.value -= 1;
        GameCtrl.Instance.AimCameraRotationSpeed.x = Mathf.RoundToInt(AimHRSSlider.value);
    }

    public void AimHRSIncreaseBtnPresssed()
    {
        AimHRSSlider.value += 1;
        GameCtrl.Instance.AimCameraRotationSpeed.x = Mathf.RoundToInt(AimHRSSlider.value);
    }

    public void AimVRSSliderChange(float value)
    {
        AimVRSText.text = Mathf.RoundToInt(value).ToString();
        AimVRSIncreaseBtn.interactable = value < AimVRSSlider.maxValue;
        AimVRSDecreaseBtn.interactable = value > AimVRSSlider.minValue;
        GameCtrl.Instance.AimCameraRotationSpeed.y = Mathf.RoundToInt(value);
    }

    public void AimVRSDecreaseBtnPressed()
    {
        AimVRSSlider.value -= 1;
        GameCtrl.Instance.AimCameraRotationSpeed.y = Mathf.RoundToInt(AimVRSSlider.value);
    }

    public void AimVRSIncreaseBtnPressed()
    {
        AimVRSSlider.value += 1;
        GameCtrl.Instance.AimCameraRotationSpeed.y = Mathf.RoundToInt(AimVRSSlider.value);
    }

    // Private Methods
    private void AudioSettingsInit()
    {
        // music settings
        MusicSlider.value = GameCtrl.Instance.MusicVolume;
        MusicText.text = Mathf.RoundToInt(MusicSlider.value * 100).ToString();
        MusicIncreaseBtn.interactable = MusicSlider.value < MusicSlider.maxValue;
        MusicDecreaseBtn.interactable = MusicSlider.value > MusicSlider.minValue;

        // sfx settings
        MusicSlider.value = GameCtrl.Instance.SFXVolume;
        SFXText.text = Mathf.RoundToInt(SFXSlider.value * 100).ToString();
        SFXIncreaseBtn.interactable = SFXSlider.value < SFXSlider.maxValue;
        SFXDecreaseBtn.interactable = SFXSlider.value > SFXSlider.minValue;
    }

    private void CameraSettingsInit()
    {
        // horizontal rotation speed
        HRSSlider.value = GameCtrl.Instance.CameraRotationSpeed.x;
        HRSText.text = Mathf.RoundToInt(HRSSlider.value).ToString();
        HRSIncreaseBtn.interactable = HRSSlider.value < HRSSlider.maxValue;
        HRSDecreaseBtn.interactable = HRSSlider.value > HRSSlider.minValue;

        // vertical rotation speed
        VRSSlider.value = GameCtrl.Instance.CameraRotationSpeed.y;
        VRSText.text = Mathf.RoundToInt(VRSSlider.value).ToString();
        VRSIncreaseBtn.interactable = VRSSlider.value < VRSSlider.maxValue;
        VRSDecreaseBtn.interactable = VRSSlider.value > VRSSlider.minValue;

        // invert horizontal rotation
        IsIHCOn = GameCtrl.Instance.InvertHorizontalRotation;
        if (IsIHCOn)
        {
            IHCText.text = "On";
            IHCOffBtn.interactable = true;
            IHCOnBtn.interactable = false;
        }
        else
        {
            IHCText.text = "Off";
            IHCOffBtn.interactable = false;
            IHCOnBtn.interactable = true;
        }

        // invert vertical rotation
        IsIVCOn = GameCtrl.Instance.InvertVerticalRotation;
        if (IsIVCOn)
        {
            IVCText.text = "On";
            IVCOnBtn.interactable = false;
            IVCOffBtn.interactable = true;
        }
        else
        {
            IVCText.text = "Off";
            IVCOnBtn.interactable = true;
            IVCOffBtn.interactable = false;
        }

        // invert horizontal rotation speed
        AimHRSSlider.value = GameCtrl.Instance.AimCameraRotationSpeed.x;
        AimHRSText.text = Mathf.RoundToInt(AimHRSSlider.value).ToString();
        AimHRSIncreaseBtn.interactable = AimHRSSlider.value < AimHRSSlider.maxValue;
        AimHRSDecreaseBtn.interactable = AimHRSSlider.value > AimHRSSlider.minValue;

        // invert horizontal rotation speed
        AimVRSSlider.value = GameCtrl.Instance.AimCameraRotationSpeed.y;
        AimVRSText.text = Mathf.RoundToInt(AimVRSSlider.value).ToString();
        AimVRSIncreaseBtn.interactable = AimVRSSlider.value < AimVRSSlider.maxValue;
        AimVRSDecreaseBtn.interactable = AimVRSSlider.value > AimVRSSlider.minValue;
    }
}
