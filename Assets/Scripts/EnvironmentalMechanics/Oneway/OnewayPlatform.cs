using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnewayPlatform : MonoBehaviour
{
    [SerializeField]
    bool EnabledWhenClimbing = true;

    [SerializeField]
    TriggerEventPassThrough CollideSideTrigger;

    [SerializeField]
    TriggerEventPassThrough PassThroughSideTrigger;

    bool OnCollideSide = false;
    bool OnPassThroughSide = false;

    

    new Collider collider;

    void Awake()
    {
        collider = GetComponent<Collider>();
    }

    void OnEnable()
    {
        CollideSideTrigger.PlayerEnter += EnterCollideSide;
        CollideSideTrigger.PlayerExit += ExitCollideSide;

        PassThroughSideTrigger.PlayerEnter += EnterPassThroughSide;
        PassThroughSideTrigger.PlayerExit += ExitPassThroughSide;
    }

    void OnDisable()
    {
        CollideSideTrigger.PlayerEnter -= EnterCollideSide;
        CollideSideTrigger.PlayerExit -= ExitCollideSide;

        PassThroughSideTrigger.PlayerEnter -= EnterPassThroughSide;
        PassThroughSideTrigger.PlayerExit -= ExitPassThroughSide;
    }

    void EnterCollideSide()
    {
        OnCollideSide = true;
    }

    void EnterPassThroughSide()
    {
        OnPassThroughSide = true;
    }

    void ExitCollideSide()
    {
        OnCollideSide = false;
    }

    void ExitPassThroughSide()
    {
        OnPassThroughSide = false;
    }

    void Update()
    {
        bool abovePlatform = OnCollideSide && !OnPassThroughSide;
        bool onLadder = false;

        collider.enabled = abovePlatform && (EnabledWhenClimbing || !onLadder);
    }
}
