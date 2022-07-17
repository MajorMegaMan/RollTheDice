using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    public AudioMixer mixer;

    public string groupName = "Master";

    public void SetVolume(float sliderValue)
    {
        float targetVol = -80;
        if(sliderValue != 0.0f)
        {
            targetVol = Mathf.Log10(sliderValue) * 20;
        }
        mixer.SetFloat(groupName, targetVol);
    }
}
