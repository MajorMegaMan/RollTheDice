using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimeScaleControl : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float timeScale = 1.0f;

    float prev = 0.0f;

    private void Update()
    {
        if(prev != Time.timeScale)
        {
            timeScale = Time.timeScale;
        }
        prev = Time.timeScale;
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            Time.timeScale = timeScale;
            prev = timeScale;
        }
    }
}
