// IngameManagement.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameManagement : MonoBehaviour //ai help generator some of the code
{
    private bool isPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu UI
        Time.timeScale = 0f; // Freeze game time
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for UI interaction
        Cursor.visible = true; // Make the cursor visible
        isPaused = true;
        DisablePlayerActions();
        DisableEnemyActions();
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu UI
        Time.timeScale = 1f; // Resume game time
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor for gameplay
        Cursor.visible = false; // Hide the cursor
        isPaused = false;
        EnablePlayerActions();
        EnableEnemyActions();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure game time is running before restarting
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Ensure game time is running before quitting
        #if UNITY_EDITOR
        SceneManager.LoadScene("Start"); // Load the Main Menu scene in the editor
        #else
        Application.Quit(); // Quit the application in a built version
        #endif
    }

    private void DisablePlayerActions()
    {
        // Disable scripts that handle player actions, such as shooting
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    private void EnablePlayerActions()
    {
        // Enable scripts that handle player actions, such as shooting
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void DisableEnemyActions()
    {
        // Disable all enemy AI scripts
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.PauseEnemy();
        }
    }

    private void EnableEnemyActions()
    {
        // Enable all enemy AI scripts
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.ResumeEnemy();
        }
    }
}


