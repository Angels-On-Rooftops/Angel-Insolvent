using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJump : MonoBehaviour, IAdvancedMovementStateSpec
{
    public Dictionary<AdvancedMovementState, bool> Transitions => throw new System.NotImplementedException();
    public Dictionary<string, object> MovementProperties => throw new System.NotImplementedException();

    public void TransitionedTo()
    {
        throw new System.NotImplementedException();
    }

    public void TransitioningFrom()
    {
        throw new System.NotImplementedException();
    }
}
