using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI textMode;

    // Start is called before the first frame update
    public void Start()
    {
        switch (LevelModeSetup.mode)
        {
            case Mode.Wfc:
                textMode.text = "WFC Mode";
                break;
            case Mode.Safety:
                textMode.text = "Safety Mode";
                break;
            default:
                textMode.text = "Safety Mode";
                break;
        }
    }

    public void GiveUp()
    {
        // 0 is the index of the menu scene on build settings;
        SceneManager.LoadScene(0);
    }

}
