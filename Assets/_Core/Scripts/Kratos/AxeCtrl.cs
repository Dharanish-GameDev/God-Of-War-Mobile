using System;
using System.Collections;
using UnityEngine;

public class AxeCtrl : MonoBehaviour
{
    [SerializeField] private K_Manager manager;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rotateSpeed = 25.0f;
    [SerializeField] private float throwRange = 30.0f;
    [SerializeField] private int damageAmount = 20;

    [Header("Effects")]
    [SerializeField] private GameObject smokeEffectObj;
    [SerializeField] private GameObject returnedEffectObj;
    [SerializeField] private ParticleSystemStopCallback returnedEffectCallback;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip axeThrowClip;
    [SerializeField] private AudioClip axeRecallClip;

    // private variables
    private float distance = 0.0f;
    private Vector3 tempVelocity;
    private bool isActivate = false;
    private bool isRecall = false;
    private bool isCancel = false;
    private bool isDamaged = false;

    // Properties
    public GameObject SmokeEffectObj { get { return smokeEffectObj; } }
    public GameObject ReturnedEffectObj { get { return returnedEffectObj; } }

    private void Start()
    {
        smokeEffectObj.SetActive(false);
        returnedEffectObj.SetActive(false);

        returnedEffectCallback.OnParticleStopped += Event_ReturnedParticleStopped;
    }

    private void OnDestroy()
    {
        returnedEffectCallback.OnParticleStopped -= Event_ReturnedParticleStopped;
    }

    private void Update()
    {
        // spin the axe when isactivated
        if (!isActivate) return;

        // when recall the axe
        if (isRecall)
        {
            distance = 0.0f;
            rotateSpeed = 1500.0f;
            transform.Rotate(0.0f, 0.0f, Time.deltaTime * rotateSpeed); // prviously rotate in x axis}
        }
        // throw the axe
        else
        {
            if (isCancel)
            {
                transform.Rotate(0.0f, 0.0f, Time.deltaTime * -rotateSpeed);
                ApplyDownforce();
                return;
            }

            transform.Rotate(0.0f, 0.0f, Time.deltaTime * -rotateSpeed);
            distance = (transform.position - manager.transform.position).sqrMagnitude;
        }
    }

    private void FixedUpdate()
    {
        if (isCancel) return;

        // axe reach the maximum range
        if (distance >= throwRange * throwRange) ApplyDownforce();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // axe handle hits the object
        if (isRecall) return;

        rb.useGravity = true;
        rotateSpeed = 900.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRecall) return;

        // stop audio
        if (source.isPlaying) source.Stop();

        // damage the enemy
        if (other.gameObject.CompareTag("Enemy") && !isDamaged)
        {
            isDamaged = true;
            LevelManager.Instance.TrollManager.TrollHealth.GiveDamage(damageAmount);
        }

        // axe head hits the object
        isActivate = false;
        StopVelocity();
        rb.useGravity = false;
        rb.isKinematic = true;
        isCancel = false;

        // stop playing effects
        smokeEffectObj.SetActive(false);
        returnedEffectObj.SetActive(false);
    }

    // Property Methods
    public bool GetIsActivate()
    {
        return isActivate;
    }

    public void SetIsActivate(bool value)
    {
        isActivate = value;

        if (isActivate)
        {
            // play axe throw audio
            source.clip = axeThrowClip;
            source.Play();
        }
        else source.Stop();
    }

    public void SetIsRecall(bool value, bool isCancel = false) 
    {
        isDamaged = false;
        isRecall = value;
        this.isCancel = isCancel;

        // play axe recall audio
        if (isRecall)
        {
            source.clip = axeRecallClip;
            source.Play();
        }
        else source.Stop();
    }

    // Event Methods
    private void Event_ReturnedParticleStopped()
    {
        returnedEffectObj.SetActive(false);
    }

    // Private Methods
    private void StopVelocity()
    {
        // stop the rigidbody movement
        tempVelocity = Vector3.zero;
        rb.velocity = tempVelocity;
        rb.inertiaTensor = tempVelocity;
    }

    private void ApplyDownforce()
    {
        // apply downforce slowly
        rb.useGravity = true;
        tempVelocity = rb.velocity;
        tempVelocity.y -= 2.0f;
        rb.velocity = tempVelocity;
    }
}
