using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibrationManager : WK.Unity.SingletonMonoBehaviour< VibrationManager > {
    public bool IsEnableVibration { get { return isEnableVibration; } }
    bool isEnableVibration = false;

    void Awake()
    {
        isEnableVibration = PlayerPrefs.GetInt( PrefKeys.VIBRATE, 1 ) == 1;
    }

    public void SetEnableVibration( bool enable )
    {
        isEnableVibration = enable;
        PlayerPrefs.SetInt( PrefKeys.VIBRATE, isEnableVibration ? 1 : 0 );
    }

    public void Vibrate()
    {
        if( !isEnableVibration )
        {
            return;
        }

        WK.Vibration.Snap();
    }
}
