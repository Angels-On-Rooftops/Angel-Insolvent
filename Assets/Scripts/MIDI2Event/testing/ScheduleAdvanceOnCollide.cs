using GameStateManagement;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScheduleAdvanceOnCollide : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity sys;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController controller))
        {
            if (sys.IsPlaying)
            {
                sys.ScheduleAdvance();
            }
        }
    }
}
