using System;
using System.Collections;
using UnityEngine;

public static class CoroutineUtil
{
    public static IEnumerator DoActionAfterTime(Action toPerform, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        toPerform();
    }
}
