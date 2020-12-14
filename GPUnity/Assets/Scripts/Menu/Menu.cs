using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    
    public void Play(string mode)
    {
        switch (mode)
        {
            case "Wfc":
                LevelModeSetup.mode = Mode.Wfc;
                break;
            case "Safety":
                LevelModeSetup.mode = Mode.Safety;
                break;
            default:
                LevelModeSetup.mode = Mode.Safety;
                break;
        }

        // 1 is the index of the game scene on build settings;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
