using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagTrigger : MonoBehaviour
{

    public LevelDesigner levelDesigner;

    void OnTriggerEnter2D()
    {
        levelDesigner.LevelCompleted();
    }
}
