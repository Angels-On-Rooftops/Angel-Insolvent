using System;
using UnityEngine;


class Integral
{
    // Integrate area under AnimationCurve between start and end time
    public static float IntegrateCurve(AnimationCurve curve, float startTime, float endTime, int steps)
    {
        return Integrate(curve.Evaluate, startTime, endTime, steps);
    }

    // Integrate function f(x) using the trapezoidal rule between x=x_low..x_high
    public static float Integrate(Func<float, float> f, float x_low, float x_high, int N_steps)
    {
        float h = (x_high - x_low) / N_steps;
        float res = (f(x_low) + f(x_high)) / 2;
        for (int i = 1; i < N_steps; i++)
        {
            res += f(x_low + i * h);
        }
        return h * res;
    }

}
