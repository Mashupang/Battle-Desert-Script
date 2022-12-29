using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    private AudioSource bgAudioSource;
    public AudioClip bg_Music_1_Clip;
    public AudioClip bg_Music_2_Clip;
    public AudioClip win_Music_Clip;
    

    // Start is called before the first frame update
    void Start()
    {
        bgAudioSource = GetComponent<AudioSource>();

        bgAudioSource.clip = bg_Music_1_Clip;
        bgAudioSource.Play();
        bgAudioSource.loop = true;
    }


    public void PlayBgMusic()
    {
        bgAudioSource.clip = bg_Music_2_Clip;
        bgAudioSource.Play();
    }

    public void PlayWinMusic()
    {
        bgAudioSource.volume = 0.2f;
        bgAudioSource.clip = win_Music_Clip;
        bgAudioSource.Play();
    }



    public void StopMusic()
    {
        bgAudioSource.Stop();
    }
}
