using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAudioController : MonoBehaviour
{
    private AudioSource explosionAudioSource;
    public AudioClip explosionClip;
    private float delayDestroyTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        explosionAudioSource = GetComponent<AudioSource>();
        explosionAudioSource.clip = explosionClip;
        explosionAudioSource.Play();
        Destroy(gameObject, delayDestroyTime);
    }
}
