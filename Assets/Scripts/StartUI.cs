using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void StartGame()
    {
        SceneManager.LoadScene("FPS"); // Replace "FPSGameScene" with the name of your game scene
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
