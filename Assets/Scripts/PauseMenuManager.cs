using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private void Start()
    {
        // Unlock and make the cursor visible when the Pause Menu Scene is loaded
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    public void ResumeGame()
    {
        SceneManager.UnloadSceneAsync("Main Menu"); // Unload the Pause Menu Scene
        Time.timeScale = 1f; // Resume game time
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    public void RestartGame()
    {
        // Reload the current game scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start");
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Keep the cursor visible
    }

    public void QuitGame()
    {
        
        // Quit the application
        Application.Quit();
    }
}