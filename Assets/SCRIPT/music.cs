using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] bg;
    private AudioClip bgClip;
    void Start()
    {
        background();
    }
 
    public  void background()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        bgClip = bg[Random.Range(0, bg.Length - 1)];
        audioSource.clip = bgClip;
        audioSource.Play();
    }
}
