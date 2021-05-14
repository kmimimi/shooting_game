#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public static class KLog
{
    public const string DEBUG_COLOR_EDITOR  = "#800080";
    public const string DEBUG_COLOR_UNLOCK  = "#003300";




    public static void LogError(string message, Object context = null)
    {
        Debug.LogError(message, context);
        //Fabric.Crashlytics.Crashlytics.RecordCustomException(title, message, StackTraceUtility.ExtractStackTrace());
    }


    [Conditional("ENABLE_LOG")]
    public static void LogWarning(string message, Object context = null)
    {
        Debug.LogWarning(message, context);
    }

    [Conditional("ENABLE_LOG")]
    public static void Log(string message, Object context = null)
    {
        Debug.Log(message, context);
    }

    [Conditional("ENABLE_LOG")]
    public static void EditorLog(string message, Object context = null)
    {
        Log("<color=" + DEBUG_COLOR_EDITOR + ">" + message + "</color>", context);
    }

    [Conditional("ENABLE_LOG")]
    public static void UnlockLog(string message, Object context = null)
    {
        LogWarning("<color=" + DEBUG_COLOR_UNLOCK + ">" + message + "</color>", context);
    }
}