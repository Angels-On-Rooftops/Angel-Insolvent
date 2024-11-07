using System.Collections;
using UnityEngine;

public static class LayerUtil
{
    public static bool IsEnabledInMask(LayerMask mask, int layer)
    {
        return (mask & (1 << layer)) != 0;
    }
}
