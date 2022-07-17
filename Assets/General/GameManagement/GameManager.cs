using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Health m_playerHealth;

    [SerializeField] EnemySpawner m_spawner;
    [SerializeField] SpawnSettings m_dynamicSettings;

    // Start is called before the first frame update
    void Start()
    {
        CopySettings();
        m_spawner.settings = m_dynamicSettings;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDie()
    {
        SceneManager.LoadScene(0);
    }

    public void HealPlayer(int amount)
    {
        m_playerHealth.Heal(amount);
    }

    void CopySettings()
    {
        m_dynamicSettings.desiredWaveCount = m_spawner.settings.desiredWaveCount;
        m_dynamicSettings.maximumEnemyPopulation = m_spawner.settings.maximumEnemyPopulation;
        m_dynamicSettings.allowableOverSpawnLimit = m_spawner.settings.allowableOverSpawnLimit;
        m_dynamicSettings.waveSeperationTime = m_spawner.settings.waveSeperationTime;
        m_dynamicSettings.miniWaveTime = m_spawner.settings.miniWaveTime;
        m_dynamicSettings.m_environmentMask = m_spawner.settings.m_environmentMask;
        m_dynamicSettings.randomSpawnMask = m_spawner.settings.randomSpawnMask;
}

    public void RampUpSettings(int amount)
    {
        m_dynamicSettings.desiredWaveCount += amount;
        if(amount > 5)
        {
            m_dynamicSettings.allowableOverSpawnLimit += 1;
        }
    }
}
