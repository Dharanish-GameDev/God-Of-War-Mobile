using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the damage indication given to player based on the kratos health.
/// </summary>
[RequireComponent(typeof(K_Manager))]
public class K_DamageIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private K_Manager manager;
    [SerializeField] private Image damageImage;

    [Header("Value")]
    [SerializeField] private float updateTime = 0.05f;
    [SerializeField] private float fadeAmount = 0.1f;
    [SerializeField] private float alpha = 0.9f;

    // Private variables
    private Color tempColor;
    private bool canShowDamage;
    private bool isFade, isFadeComplete;

    private void Start()
    {
        damageImage.gameObject.SetActive(false);

        // subscribe to event
        LevelManager.Instance.KratosHealth.OnDamage += Event_OnKratosDamage;
        LevelManager.Instance.KratosHealth.OnHeal += Event_OnKratosHeal;
    }

    private void OnDestroy()
    {
        // unsubscribe from event
        LevelManager.Instance.KratosHealth.OnDamage -= Event_OnKratosDamage;
        LevelManager.Instance.KratosHealth.OnHeal -= Event_OnKratosHeal;
    }

    private void FixedUpdate()
    {
        if (manager.IsDead) return;

        // show damage indication
        if (canShowDamage) ShowDamageIndication();
    }

    // Event Methods
    private void Event_OnKratosDamage(float currentHealth)
    {
        canShowDamage = currentHealth < 30;

        // cannot show damage
        if (!canShowDamage) return;

        damageImage.gameObject.SetActive(true);

        // dead
        if (currentHealth <= 0)
        {
            // set damage indication to maximum
            isFade = false;
            isFadeComplete = false;
            damageImage.gameObject.SetActive(false);
            CancelInvoke();
        }
    }

    private void Event_OnKratosHeal(float currentHealth)
    {
        if (!canShowDamage) return;

        // good condition
        if (currentHealth >= 30 && canShowDamage)
        {
            // hide damage indicator
            canShowDamage = false;
            isFade = false;
            isFadeComplete = false;
            damageImage.gameObject.SetActive(false);
        }
    }

    // Public Methods
    public void HideDamageIndicator()
    {
        isFade = false;
        isFadeComplete = false; 
        damageImage.gameObject.SetActive(false);
        CancelInvoke();
    }

    // Private Methods
    private void ShowDamageIndication()
    {
        // fadein damage image
        if (!isFade && !isFadeComplete)
        {
            isFade = true;
            tempColor = damageImage.color;
            tempColor.a = 0;
            InvokeRepeating(nameof(FadeIn), 0, updateTime);
        }

        // wait for fadein to complete
        if (!isFadeComplete) return;

        // fadeout damage image
        if (!isFade)
        {
            isFade = true;
            tempColor = damageImage.color;
            tempColor.a = alpha;
            InvokeRepeating(nameof(FadeOut), 0, updateTime);
        }
    }

    private void FadeIn()
    {
        tempColor.a = (tempColor.a + fadeAmount >= alpha) ? alpha : tempColor.a + fadeAmount;
        damageImage.color = tempColor;

        // fade in complete
        if (tempColor.a >= alpha)
        {
            isFade = false;
            isFadeComplete = true;
            CancelInvoke(nameof(FadeIn));
        }
    }

    private void FadeOut()
    {
        tempColor.a = (tempColor.a - fadeAmount <= 0) ? 0 : tempColor.a - fadeAmount;
        damageImage.color = tempColor;

        // fade out complete
        if (tempColor.a <= 0)
        {
            isFade = false;
            isFadeComplete = false;
            CancelInvoke(nameof(FadeOut));
        }
    }
}
