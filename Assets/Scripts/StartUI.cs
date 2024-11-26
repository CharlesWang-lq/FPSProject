using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Any initialization logic if needed
    }

    public void StartGame()
    {
        // Load the FPS scene, then set it as the active scene
        SceneManager.LoadScene("FPS", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        // Empty update can be removed if not used
    }
}
