using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the scenes loading with loading screen.
/// </summary>
public class ScenesCtrl : MonoBehaviour
{
    public static ScenesCtrl Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Image bg;
    [SerializeField] private GameObject loadingBar;

    [Header("Values")]
    [SerializeField] private float updateTime = 0.05f;
    [SerializeField] private float fadeAmount = 0.1f;

    // Private Variables
    private AsyncOperation asyncOperation;
    private Color temp;
    private bool isFade;
    private bool canLoad, isLoading;

    private void Awake()
    {
        // make singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    //Public Methods
    public void LoadScene(int sceneIndex)
    {
        if (isLoading) return;

        // start load scene
        Time.timeScale = 1.0f;
        isFade = false;
        isLoading = true;
        loadingBar.SetActive(false);
        StartCoroutine(C_LoadScene(sceneIndex));
    }

    // Private Methods
    private void FadeIn()
    {
        temp.a = (temp.a + fadeAmount >= 1) ? 1 : temp.a + fadeAmount;
        bg.color = temp;

        if (temp.a >= 1)
        {
            isFade = false;
            canLoad = true;
            CancelInvoke(nameof(FadeIn));
        }
    }

    private void FadeOut()
    {
        temp.a = (temp.a - fadeAmount <= 0) ? 0 : temp.a - fadeAmount;
        bg.color = temp;

        if (temp.a <= 0)
        {
            isFade = false;
            CancelInvoke(nameof(FadeOut));
        }
    }

    private IEnumerator C_LoadScene(int sceneIndex)
    {
        // fade in
        if (!isFade)
        {
            isFade = true;
            temp = bg.color;
            temp.a = 0;
            InvokeRepeating(nameof(FadeIn), 0, updateTime);
        }

        // wait to load scene
        yield return new WaitUntil(() => canLoad);

        // enable loading text
        loadingBar.SetActive(true);

        // load scene
        canLoad = false;
        asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        // wait to fade out
        yield return new WaitUntil(() => asyncOperation.isDone);

        // disable loading text
        loadingBar.SetActive(false);

        // fade out
        if (!isFade)
        {
            isFade = true;
            temp = bg.color;
            temp.a = 1;
            InvokeRepeating(nameof(FadeOut), 0, updateTime);
        }

        isLoading = false;
    }
}
