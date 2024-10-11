using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Macros
{
    public static IEnumerator Buffer(float seconds, Func<bool> trueWhenDone)
    {
        while (seconds > 0)
        {
            if (trueWhenDone.Invoke())
            {
                yield break;
            };
            yield return new WaitForEndOfFrame();
            seconds -= Time.deltaTime;
        }
    }

}
