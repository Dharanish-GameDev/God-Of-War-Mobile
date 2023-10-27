using System;
using UnityEngine;

/// <summary>
/// Fire an event when the particle system is stopped.
/// </summary>
public class ParticleSystemStopCallback : MonoBehaviour
{
    public event Action OnParticleStopped;

    // called when the attached particle is stopped
    private void OnParticleSystemStopped()
    {
        OnParticleStopped?.Invoke();
    }
}
