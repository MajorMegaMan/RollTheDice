using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FakeWaveTimer : MonoBehaviour
{
    public TextMeshProUGUI waveCountText;
    public TextMeshProUGUI timerCountText;
    public int waveCount;
    public float timerCount;

    private void Start()
    {
        timerCount = 60;
        waveCount = 1;
    }

    private void Update()
    {
        if (timerCount < 0)
        {
            waveCount++;
            timerCount = 60;
        }

        timerCount -= Time.deltaTime * 2;

        waveCountText.text = waveCount.ToString();
        timerCountText.text = "0:" + Mathf.RoundToInt(timerCount).ToString();
    }
}
