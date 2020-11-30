using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Discover,
    Timer
}

public static class LevelModeSetup
{
    public static Mode mode = Mode.Discover;
}
