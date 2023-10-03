using System;
using System.Collections;
using UnityEngine;

public class AxeCtrl : MonoBehaviour
{
    [SerializeField] private K_Manager manager;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rotateSpeed = 25.0f;
    [SerializeField] private float throwRange = 30.0f;

    // private variables
    private float distance = 0.0f;
    private Vector3 tempVelocity;
    private bool isActivate = false;
    private bool isRecall = false;

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
            transform.Rotate(0.0f, 0.0f, Time.deltaTime * -rotateSpeed);
            distance = Vector3.Distance(transform.position, manager.transform.position);
        }
    }

    private void FixedUpdate()
    {
        // axe reach the maximum range
        if (distance >= throwRange) ApplyDownforce();
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
        // axe head hits the object
        isActivate = false;
        StopVelocity();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    // Property Methods
    public bool GetIsActivate()
    {
        return isActivate;
    }

    public void SetIsActivate(bool value)
    {
        isActivate = value;
    }

    public void SetIsRecall(bool value) 
    {
        isRecall = value; 
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
