using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAudioController : MonoBehaviour
{
    public AudioClip fireClip;

    private AudioSource fireAudioSource;
    private float delayDestroyTime = 3f;

    void Start()
    {
        fireAudioSource = GetComponent<AudioSource>();
        fireAudioSource.clip = fireClip;
        fireAudioSource.Play();
        Destroy(gameObject, delayDestroyTime);
    }

}
