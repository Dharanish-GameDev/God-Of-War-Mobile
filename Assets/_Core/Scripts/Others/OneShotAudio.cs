using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotAudio : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    // Private Variable
    private bool isStartPlaying;

    // Properties
    public AudioSource Source { get { return source; } }

    private void OnEnable()
    {
        isStartPlaying = false;
    }

    private void Update()
    {
        if (!isStartPlaying || source.isPlaying) return;

        // return this audio to pool
        isStartPlaying = false;
        AudioManager.Instance.ReturnAudioToPool(this);
    }

    public void PlayAudio()
    {
        if (!source.clip || isStartPlaying) return;

        source.Play();
        isStartPlaying = true;
    }
}
