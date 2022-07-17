﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("Volume Settings Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider soundEffectSlider;

    public float master = 0.7f;
    public float music = 0.7f;
    public float soundEffect = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        SoundSettings.LoadSound();
        // Set sliders to static soundSettings values
        // Turns out this must be called during start and not during awake.
        masterSlider.value = SoundSettings.master;
        masterSlider.onValueChanged.AddListener(delegate { SetLevel(masterSlider, ref SoundSettings.master); });

        musicSlider.value = SoundSettings.music;
        musicSlider.onValueChanged.AddListener(delegate { SetLevel(musicSlider, ref SoundSettings.music); });

        soundEffectSlider.value = SoundSettings.soundEffect;
        soundEffectSlider.onValueChanged.AddListener(delegate { SetLevel(soundEffectSlider, ref SoundSettings.soundEffect); });
    }

    void SetLevel(Slider slider, ref float settingsValue)
    {
        settingsValue = slider.value;
    }

    private void Update()
    {
        master = SoundSettings.master;
        music = SoundSettings.music;
        soundEffect = SoundSettings.soundEffect;
    }

    private void OnApplicationQuit()
    {
        SoundSettings.SaveSound();
    }
}