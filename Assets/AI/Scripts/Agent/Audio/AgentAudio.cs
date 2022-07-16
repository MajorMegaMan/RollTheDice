using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Head Clip")]
    public AudioClip idleClip;

    public AudioClip attackClip;
    public AudioClip hurtClip;
    public AudioClip deathClip;

    [Header("Foot Clip")]
    public AudioClip footClip;

    public void PlayClip(AudioClip targetClip)
    {
        audioSource.PlayOneShot(targetClip);
    }
}
