using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("fuck");
        Application.Quit();
    }

    public void GameLoad(string sceneName)
    {
        Debug.Log("fddduck");
        SceneManager.LoadScene(sceneName);
    }

}
