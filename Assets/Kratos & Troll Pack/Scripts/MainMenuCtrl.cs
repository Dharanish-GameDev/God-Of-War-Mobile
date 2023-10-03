using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCtrl : MonoBehaviour
{
    public GameObject LoadScene;
    public Slider MusicSlider, SFXSlider, HRSSlider, VRSSlider, AimHRSSlider, AimVRSSlider;
    public Image ImgMusicIncrease, ImgMusicDecrease;
    public Image ImgSFXIncrease, ImgSFXDecrease;
    public Image ImgHRSIncrease, ImgHRSDecrease;
    public Image ImgVRSIncrease, ImgVRSDecrease;
    public Image ImgAimHRSIncrease, ImgAimHRSDecrease;
    public Image ImgAimVRSIncrease, ImgAimVRSDecrease;
    public Image ImgIHCOn, ImgIHCOff;
    public Image ImgIVCOn, ImgIVCOff;
    public Color normalColor, disableColor;
    public TMP_Text MusicText, SFXText, HRSText, VRSText, IHCText, IVCText, AimHRSText, AimVRSText;
    public bool IsIHCOn = false;
    public bool IsIVCOn = false;

    //Start game from MainMenu
    public void StartGame(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    //Quit application from MainMenu
    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    public void MusicSliderChange(float value)
    {
        ImgMusicDecrease.color = value <= MusicSlider.minValue ? disableColor : normalColor;
        ImgMusicIncrease.color = value >= MusicSlider.maxValue ? disableColor : normalColor;
        MusicText.text = Mathf.RoundToInt(value * 100).ToString();
    }

    public void MusicDecreaseBtn()
    {
        MusicSlider.value -= 0.1f;
    }

    public void MusicIncreaseBtn()
    {
        MusicSlider.value += 0.1f;
    }

    public void SFXSliderChange(float value)
    {
        ImgSFXDecrease.color = value <= SFXSlider.minValue ? disableColor : normalColor;
        ImgSFXIncrease.color = value >= SFXSlider.maxValue ? disableColor : normalColor;
        SFXText.text = Mathf.RoundToInt(value * 100).ToString();
    }

    public void SFXDecreaseBtn()
    {
        SFXSlider.value -= 0.1f;
    }

    public void SFXIncreaseBtn()
    {
        SFXSlider.value += 0.1f;
    }

    public void HRSSliderChange(float value)
    {
        ImgHRSDecrease.color = value <= HRSSlider.minValue ? disableColor : normalColor;
        ImgHRSIncrease.color = value >= HRSSlider.maxValue ? disableColor : normalColor;
        HRSText.text = Mathf.RoundToInt(value * 10).ToString();
    }

    public void HRSDecreaseBtn()
    {
        HRSSlider.value -= 0.1f;
    }

    public void HRSIncreaseBtn()
    {
        HRSSlider.value += 0.1f;
    }

    public void VRSSliderChange(float value)
    {
        ImgVRSDecrease.color = value <= VRSSlider.minValue ? disableColor : normalColor;
        ImgVRSIncrease.color = value >= VRSSlider.maxValue ? disableColor : normalColor;
        VRSText.text = Mathf.RoundToInt(value * 10).ToString();
    }

    public void VRSDecreaseBtn()
    {
        VRSSlider.value -= 0.1f;
    }

    public void VRSIncreaseBtn()
    {
        VRSSlider.value += 0.1f;
    }

    public void IHCPressed()
    {
        if (!IsIHCOn)
        {
            IsIHCOn = true;
            IHCText.text = "On";
            ImgIHCOff.color = disableColor;
            ImgIHCOn.color = normalColor;
        }
        else
        {
            IsIHCOn = false;
            IHCText.text = "Off";
            ImgIHCOff.color = normalColor;
            ImgIHCOn.color = disableColor;
        }
    }

    public void IHCOnBtn()
    {
        if (!IsIHCOn)
        {
            IsIHCOn = true;
            IHCText.text = "On";
            ImgIHCOff.color = disableColor;
            ImgIHCOn.color = normalColor;
        }
    }

    public void IHCOffBtn()
    {
        if (IsIHCOn)
        {
            IsIHCOn = false;
            IHCText.text = "Off";
            ImgIHCOff.color = normalColor;
            ImgIHCOn.color = disableColor;
        }
    }

    public void IVCPressed()
    {
        if (!IsIVCOn)
        {
            IsIVCOn = true;
            IVCText.text = "On";
            ImgIVCOn.color = disableColor;
            ImgIVCOff.color = normalColor;
        }
        else
        {
            IsIVCOn = false;
            IVCText.text = "Off";
            ImgIVCOn.color = normalColor;
            ImgIVCOff.color = disableColor;
        }
    }

    public void IVCOnBtn()
    {
        if (!IsIVCOn)
        {
            IsIVCOn = true;
            IVCText.text = "On";
            ImgIVCOn.color = disableColor;
            ImgIVCOff.color = normalColor;
        }
    }

    public void IVCOffBtn()
    {
        if (IsIVCOn)
        {
            IsIVCOn = false;
            IVCText.text = "Off";
            ImgIVCOn.color = normalColor;
            ImgIVCOff.color = disableColor;
        }
    }

    public void AimHRSSliderChange(float value)
    {
        ImgAimHRSDecrease.color = value <= AimHRSSlider.minValue ? disableColor : normalColor;
        ImgAimHRSIncrease.color = value >= AimHRSSlider.maxValue ? disableColor : normalColor;
        AimHRSText.text = Mathf.RoundToInt(value * 10).ToString();
    }

    public void AimHRSDecreaseBtn()
    {
        AimHRSSlider.value -= 0.1f;
    }

    public void AimHRSIncreaseBtn()
    {
        AimHRSSlider.value += 0.1f;
    }

    public void AimVRSSliderChange(float value)
    {
        ImgAimVRSDecrease.color = value <= AimVRSSlider.minValue ? disableColor : normalColor;
        ImgAimVRSIncrease.color = value >= AimVRSSlider.maxValue ? disableColor : normalColor;
        AimVRSText.text = Mathf.RoundToInt(value * 10).ToString();
    }

    public void AimVRSDecreaseBtn()
    {
        AimVRSSlider.value -= 0.1f;
    }

    public void AimVRSIncreaseBtn()
    {
        AimVRSSlider.value += 0.1f;
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            Debug.Log("awdd");
            LoadScene.SetActive(true);
            yield return null;
        }
    }
}
