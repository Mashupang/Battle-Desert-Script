using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAudioController : MonoBehaviour
{
    public AudioClip explosionClip;

    private AudioSource explosionAudioSource;
    private float delayDestroyTime = 2f;

    void Start()
    {
        explosionAudioSource = GetComponent<AudioSource>();
        explosionAudioSource.clip = explosionClip;
        explosionAudioSource.Play();
        Destroy(gameObject, delayDestroyTime);
    }
}
