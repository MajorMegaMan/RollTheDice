using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public float m_bloomMin = 0.0f;
    public float m_bloomMax = 1.0f;
    float m_bloomSize = 1.0f;

    float m_normalisedBloomSize = 1.0f;

    [SerializeField] RectTransform m_bloomImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // expects a value from 0.0f to 1.0f
    public void SetBloomSize(float bloom)
    {
        m_normalisedBloomSize = bloom;
        m_bloomSize = Remap(bloom, 0.0f, 1.0f, m_bloomMin, m_bloomMax);
        m_bloomImage.localScale = Vector3.one * m_bloomSize;
    }

    float GetRange(float min, float max)
    {
        return max - min;
    }

    float GetScale(float value, float min, float max)
    {
         return (value - min) / GetRange(min, max);
    }

    float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float scale = GetScale(value, oldMin, oldMax);
        float newValue = scale * GetRange(newMin, newMax);
        return newValue + newMin;
    }

    public void RemapBloom()
    {
        m_bloomSize = Remap(m_normalisedBloomSize, 0.0f, 1.0f, m_bloomMin, m_bloomMax);
        m_bloomImage.localScale = Vector3.one * m_bloomSize;
    }
}
