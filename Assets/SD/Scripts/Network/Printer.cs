using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Printer
{
    public static void Print(string msg)
    {
        var printed = false;
#if UNITY_EDITOR
        // Debug.Log(msg);
        printed = true;
#endif
        if (!printed)
        {
            Debug.Log(msg);
        }
    }

    public static void PrintWarning(string msg)
    {
        var printed = false;
#if UNITY_EDITOR
        Debug.LogWarning(msg);
        printed = true;
#endif
        if (!printed)
        {

            Debug.LogWarning(msg);
        }
    }

    public static void PrintError(string msg)
    {
        var printed = false;
#if UNITY_EDITOR
        Debug.LogError(msg);
        printed = true;
#endif
        if (!printed)
        {
            Debug.LogError(msg);
        }
    }
}