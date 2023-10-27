using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    //Public Events
    public event Action OnDead;
    public event Action<float> OnDamage;
    public event Action<float> OnHeal;

    //Health Variables
    [SerializeField] private float maxHealth;
    private float followBarDelay = .2f;
    private float followBarTimer = 0;

    //Polish Variable
    private float followSpeed = .4f;

    //UI elements
    [SerializeField] private Image HealthBar;
    [SerializeField] private Image FollowingBar;

    // Properties
    public float CurrentHealth { get; private set; }
    

    private void Start()
    {
        CurrentHealth = maxHealth;
        GiveDamage(0);
    }

    private void Update()
    {
        if(followBarTimer >= 0)
        {
            followBarTimer -= Time.deltaTime;
        }
        else
        {
            if(FollowingBar.fillAmount != HealthBar.fillAmount)
                FollowingBar.fillAmount = Mathf.MoveTowards(FollowingBar.fillAmount, HealthBar.fillAmount, followSpeed * Time.deltaTime);
        }
    }

    // Public Methods
    public void GiveDamage(int damageAmount)
    {
        if (CurrentHealth <= 0) return;

        FollowingBar.fillAmount = CurrentHealth / maxHealth;
        CurrentHealth = (CurrentHealth - damageAmount <= 0) ? 0 : CurrentHealth - damageAmount;
        HealthBar.fillAmount = CurrentHealth / maxHealth;
        followBarTimer = followBarDelay;

        // fire damage event
        OnDamage?.Invoke(CurrentHealth);

        // fire dead event
        if(CurrentHealth <= 0) OnDead?.Invoke();
    }

    public void GiveHealth(int healAmount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = (CurrentHealth + healAmount >= maxHealth) ? maxHealth : CurrentHealth + healAmount;
        HealthBar.fillAmount = CurrentHealth / maxHealth;
        FollowingBar.fillAmount = HealthBar.fillAmount;

        // fire heal event
        OnHeal?.Invoke(CurrentHealth);
    }
}
