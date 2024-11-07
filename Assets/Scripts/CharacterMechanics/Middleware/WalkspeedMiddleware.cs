using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class WalkspeedMiddleware
{
    public static Func<float, float, float> MultiplyOverTime(AnimationCurve multiplierOverTime)
    {
        float t = 0;

        return (float walkspeed, float dt) =>
        {
            t += dt;
            return walkspeed * multiplierOverTime.Evaluate(t);
        };
    }
}
