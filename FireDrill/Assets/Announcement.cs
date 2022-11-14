using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Announcement : MonoBehaviour
{
    private AudioSource audioSource;


    [SerializeField] private float intervalTime = 0f;
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private int currentClip = 0;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        play = StartCoroutine(Play());
    }


    private Coroutine play;
    private IEnumerator Play()
    {
        WaitForSeconds wait = new WaitForSeconds(intervalTime);
        audioSource.clip = audioClips[currentClip % audioClips.Count];

        while (true)
        {
            if(!audioSource.isPlaying)
            {
                yield return wait;

                currentClip++;
                audioSource.clip = audioClips[currentClip % audioClips.Count];
                audioSource.Play();
            }

            yield return null;
        }
    }

    public void PlayAudio()
    {
        play = StartCoroutine(Play());
    }

    public void StopAudio()
    {
        if(play != null)
        {
            StopCoroutine(play);
            play = null;
        }

        audioSource.Stop();
    }
}
