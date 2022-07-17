using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundSettings
{
    public static float master = 0.7f;
    public static float music = 0.7f;
    public static float soundEffect = 0.7f;

    public static void SaveSound()
    {
        PlayerPrefs.SetFloat("vol_master", master);
        PlayerPrefs.SetFloat("vol_music", music);
        PlayerPrefs.SetFloat("vol_soundEffect", soundEffect);
    }

    public static void LoadSound()
    {
        master = PlayerPrefs.GetFloat("vol_Master", master);
        music = PlayerPrefs.GetFloat("vol_music", music);
        soundEffect = PlayerPrefs.GetFloat("vol_soundEffect", soundEffect);
    }
}