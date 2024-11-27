using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    void Start()
    {
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("FPS");
    }

    void Update()
    {
    }
}
