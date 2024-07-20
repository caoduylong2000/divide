using UnityEngine;
using System;
using System.Collections;

public static class MonoBehaviourExtentions
{
    //------------------------------------------------------------------------------
    public static IEnumerator DelayMethod( this MonoBehaviour behaviour, float delay, Action action )
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}

