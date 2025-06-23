using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class SettingsManager
{
    static bool showTooltip = true,
                enableOst = true;

    static public void SetShowTooltip(bool show)
    {
        showTooltip = show;
        Debug.Log(show);
    }

    static public bool GetShowTooltip() { return showTooltip; }

    static public void SetEnableOst(bool enable)
    {
        enableOst = enable;
    }

    static public bool GetEnableOst()
    {
        return enableOst;
    }
}