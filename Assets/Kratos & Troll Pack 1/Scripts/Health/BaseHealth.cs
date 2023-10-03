using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    //Public Events
    public event EventHandler HealthIsZero;

    //Health Variables
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private float followBarDelay = .2f;
    private float followBarTimer = 0;

    //Polish Variable
    private float followSpeed = .4f;

    //UI elements
    [SerializeField] private Image HealthBar;
    [SerializeField] private Image FollowingBar;


    private void Start()
    {
        currentHealth = maxHealth;
        GiveDamage(0);
    }

    public void GiveDamage(int health)
    {
        FollowingBar.fillAmount = currentHealth / maxHealth;
        currentHealth -= health;
        HealthBar.fillAmount = currentHealth / maxHealth;
        followBarTimer = followBarDelay;

        if(currentHealth < 0)
        {
            HealthIsZero?.Invoke(this, EventArgs.Empty);
        }
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
}
