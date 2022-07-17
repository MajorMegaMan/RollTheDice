using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Reticle m_reticle;
    [SerializeField] TMP_Text m_gunNameText;
    [SerializeField] TMP_Text m_clipCountText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateWeapon(Weapon weapon)
    {
        UpdateReticleWeapon(weapon);
        UpdateGunName(weapon);
        SetClipCount(weapon.clipSize);
    }

    public void UpdateReticleWeapon(Weapon weapon)
    {
        if (weapon != null)
        {
            if (m_reticle != null)
            {
                m_reticle.m_bloomMin = weapon.minReticleSize;
                m_reticle.m_bloomMax = weapon.maxReticleSize;
                m_reticle.RemapBloom();
            }
        }
    }

    public void SetReticleBloom(float bloom)
    {
        m_reticle.SetBloomSize(bloom);
    }

    public void UpdateGunName(Weapon weapon)
    {
        if(m_gunNameText != null)
        {
            m_gunNameText.text = weapon.weaponName;
        }
    }

    public void SetClipCount(int clipCount)
    {
        m_clipCountText.text = clipCount.ToString();
    }
}
