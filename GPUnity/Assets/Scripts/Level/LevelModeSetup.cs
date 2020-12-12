using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Wfc,
    Safety
}

public static class LevelModeSetup
{
    public static Mode mode = Mode.Safety;
}
