using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private BaseHealth k_health;
    [SerializeField] private K_Manager k_Manager;
    [SerializeField] private Troll_Manager troll_Manager;
    [SerializeField] private CameraCtrl camCtrl;

    [Header("UI")]
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private CanvasGroup gameoverPanel;
    [SerializeField] private CanvasGroup[] victoryPanelItems;
    [SerializeField] private float updateTime = 0.05f;
    [SerializeField] private float fadeAmount = 0.1f;

    // Private Variables
    private WaitForSeconds waitFor1Sec;
    private WaitUntil waitUntilFadeComplete;
    private CanvasGroup currentCanvasGroup;
    private float temp;
    private bool isFade, isFadeComplete;
    private bool isVictoryPanelShown;

    // Properties
    public BaseHealth KratosHealth { get { return k_health; } }
    public K_Manager KratosManager { get { return k_Manager; } }
    public Troll_Manager TrollManager { get { return troll_Manager; } }
    public CameraCtrl CamCtrl { get { return camCtrl; } }

    private void Awake()
    {
        Instance = this;
        AudioManager.Instance.PlayBGAudio(BGAudio.Name.TrollBattle);
    }

    private void Start()
    {
        waitFor1Sec = new WaitForSeconds(1.0f);
        waitUntilFadeComplete = new WaitUntil(() => isFadeComplete);

        gameoverPanel.gameObject.SetActive(false);
        for (int i = 0; i < victoryPanelItems.Length; i++)
        {
            victoryPanelItems[i].alpha = 0;
            victoryPanelItems[i].gameObject.SetActive(false);
        }

        k_health.OnDead += Event_KratosDead;
    }

    private void OnDestroy()
    {
        k_health.OnDead -= Event_KratosDead;
    }

    // Event Methods
    private void Event_KratosDead()
    {
        Invoke(nameof(ShowGameoverPanel), 2f);
    }

    // UI Event Methods
    public void LoadLevel(int sceneIndex)
    {
        ScenesCtrl.Instance.LoadScene(sceneIndex);
    }

    public void PlayBtnClickSound()
    {
        AudioManager.Instance.BtnClickSource.Play();
    }

    // Public Methods
    public void StopGameplay()
    {
        mobileControls.SetActive(false);
        camCtrl.enabled = false;
        k_Manager.enabled = false;
        k_Manager.K_Axe.CancelAxeRecall();
        k_Manager.K_Shield.CloseShield();
    }

    public void StopKratosDamageIndication()
    {
        k_Manager.K_DamageIndicator.HideDamageIndicator();
        k_Manager.BreatheAudioSource.Stop();
    }

    public void ShowVictoryPanel()
    {
        if (isVictoryPanelShown) return;

        isVictoryPanelShown = true;
        StartCoroutine(C_ShowVictoryPanel());
    }

    // Private Methods
    private void ShowGameoverPanel()
    {
        if (isFade) return;

        isFade = true;
        temp = 0;
        gameoverPanel.gameObject.SetActive(true);
        InvokeRepeating(nameof(FadeInGameoverPanel), 0, 0.05f);
    }

    private void FadeInGameoverPanel()
    {
        temp = (temp + fadeAmount >= 1.0f) ? 1.0f : temp + fadeAmount;
        gameoverPanel.alpha = temp;

        // fade in complete
        if (temp >= 1.0f)
        {
            isFade = false;
            isFadeComplete = true;
            CancelInvoke(nameof(FadeInGameoverPanel));
        }
    }

    private IEnumerator C_ShowVictoryPanel()
    {
        // show all victory items one by one
        for (int i = 0; i < victoryPanelItems.Length; i++)
        {
            if (!isFade)
            {
                temp = 0;
                isFade = true;
                isFadeComplete = false;
                currentCanvasGroup = victoryPanelItems[i];
                currentCanvasGroup.gameObject.SetActive(true);
                InvokeRepeating(nameof(FadeIn), 0, updateTime);
            }

            // wait untill current canvas group is fully fade in
            yield return waitUntilFadeComplete;

            // wait 1sec before showing next camvas group
            yield return waitFor1Sec;
        }
    }

    private void FadeIn()
    {
        temp = (temp + fadeAmount >= 1.0f) ? 1.0f : temp + fadeAmount;
        currentCanvasGroup.alpha = temp;

        // fade in complete
        if (temp >= 1.0f)
        {
            isFade = false;
            isFadeComplete = true;
            CancelInvoke(nameof(FadeIn));
        }
    }
}
