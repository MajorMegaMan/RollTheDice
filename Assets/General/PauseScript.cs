using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject m_pauseMenu;

    [SerializeField] KeyCode m_pauseKey = KeyCode.Escape;

    bool m_isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        m_pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(m_pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (m_isPaused)
        {
            // Unpause
            m_isPaused = false;
            m_pauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Pause
            m_isPaused = true;
            m_pauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
