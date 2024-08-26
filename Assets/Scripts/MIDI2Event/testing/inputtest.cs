using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity eventSys;

    bool playing = false;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!playing)
            {
                eventSys.Play();
                playing = true;
            }
            else
            {
                eventSys.Stop();
                playing = false;
            }
        }
    }
}
