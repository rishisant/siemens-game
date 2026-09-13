using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

/** Recognizes touch browsers as well as native mobile players. */
public static class MobileControls
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern int BC_IsTouchDevice();
#endif
    public static bool Enabled
    {
        get
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return BC_IsTouchDevice()!=0;
#else
            return Application.isMobilePlatform;
#endif
        }
    }
}
