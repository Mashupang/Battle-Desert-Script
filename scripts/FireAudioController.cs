using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAudioController : MonoBehaviour
{
    private AudioSource fireAudioSource;
    public AudioClip fireClip;
    private float delayDestroyTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        fireAudioSource = GetComponent<AudioSource>();
        fireAudioSource.clip = fireClip;
        fireAudioSource.Play();
        Destroy(gameObject, delayDestroyTime);
    }

}
