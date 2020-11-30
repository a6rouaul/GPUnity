using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public Animator transition;

    public float transitionTime = 1f;
    
    public void Play(string mode)
    {
        switch (mode)
        {
            case "Discover":
                LevelModeSetup.mode = Mode.Discover;
                break;
            case "Timer":
                LevelModeSetup.mode = Mode.Timer;
                break;
            default:
                LevelModeSetup.mode = Mode.Discover;
                break;
        }

        // 1 is the index of the game scene on build settings;
        StartCoroutine(LoadScene(1));
    }

    private IEnumerator LoadScene(int index)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(index);
    }

    public void Quit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
