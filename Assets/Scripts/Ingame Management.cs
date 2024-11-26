using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameManagement : MonoBehaviour
{
    public string pauseMenuSceneName = "Main Menu"; // Name of the Pause Menu Scene

    void Update()
    {
        // Detect ESC key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadPauseMenu();
        }
    }

    public void LoadPauseMenu()
    {
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive); // Load the Pause Menu Scene (additively)
    }
}
