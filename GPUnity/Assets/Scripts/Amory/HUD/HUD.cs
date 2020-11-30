using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI textMode;

    public Animator transition;

    public float transitionTime = 1f;

    // Start is called before the first frame update
    public void Start()
    {
        switch (LevelModeSetup.mode)
        {
            case Mode.Discover:
                textMode.text = "Discover Mode";
                break;
            case Mode.Timer:
                textMode.text = "Timer Mode";
                break;
            default:
                textMode.text = "Discover Mode";
                break;
        }
    }

    public void GiveUp()
    {
        // 0 is the index of the menu scene on build settings;
        StartCoroutine(LoadScene(0));
    }

    private IEnumerator LoadScene(int index)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(index);
    }
}
