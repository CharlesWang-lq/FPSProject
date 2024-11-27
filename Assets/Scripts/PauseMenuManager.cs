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
        // Unload the Pause Menu Scene additively
        if (SceneManager.GetSceneByName("Main Menu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Main Menu"); // Unload the Pause Menu Scene
        }

        Time.timeScale = 1f; // Resume game time
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor for gameplay
        Cursor.visible = false; // Hide the cursor
    }

    public void RestartGame()
    {
        // Reload the current game scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start"); // Load the start scene
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Keep the cursor visible
    }

    public void QuitGame()
    {
        // Quit the application
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start"); // Load the start scene
        // Application.Quit(); Uncomment this for a built version of the game
    }

    public void PauseGame()
    {
        // Load the Pause Menu Scene additively
        if (!SceneManager.GetSceneByName("Main Menu").isLoaded)
        {
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
        }

        Time.timeScale = 0f; // Pause game time
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor for UI interaction
        Cursor.visible = true; // Show the cursor
    }
}
