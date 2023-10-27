using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the behaviour of heal orb such as spawn, despawn, heal the player.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class HealOrb : MonoBehaviour
{
    [SerializeField] private GameObject idleEffect;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private GameObject activeOrbEffect;
    [SerializeField] private ParticleSystemStopCallback stopCallback;

    [Space(10)]
    [SerializeField] private int healAmount = 10;
    [SerializeField] private float spawnTime = 10;

    // Private Variables
    private BoxCollider coll;
    private float timer = 0;

    private void Start()
    {
        coll = GetComponent<BoxCollider>();

        // show the heal orb
        Spawn();
        stopCallback.OnParticleStopped += OnParticleStopped;
    }

    private void OnDestroy()
    {
        stopCallback.OnParticleStopped -= OnParticleStopped;
    }

    private void Update()
    {
        HandleHealOrbSpawn();
    }

    // Event Methods
    private void OnParticleStopped()
    {
        // despawn the heal orb
        Despawn();
    }

    // Public Methods
    public void ShowActive(bool isActive)
    {
        if (isActive) activeOrbEffect.SetActive(true);
        else activeOrbEffect.SetActive(false);
    }

    public void Heal(K_Manager manager)
    {
        // disable collider
        coll.enabled = false;

        // show collect effect
        idleEffect.SetActive(false);    
        collectEffect.SetActive(true);

        // give heal to player
        manager.KratosHealth.GiveHealth(healAmount);
    }

    // Private Methods
    private void Spawn()
    {
        timer = 0;
        coll.enabled = true;
        gameObject.SetActive(true);
        idleEffect.SetActive(true);
        collectEffect.SetActive(false);
        activeOrbEffect.SetActive(false);
    }

    private void Despawn()
    {
        timer = spawnTime;
        idleEffect.SetActive(false);
        collectEffect.SetActive(false);
    }

    private void HandleHealOrbSpawn()
    {
        if (timer <= 0) return;

        // wait to spawn
        timer -= Time.deltaTime;
        if (timer <= 0) Spawn();
    }
}
