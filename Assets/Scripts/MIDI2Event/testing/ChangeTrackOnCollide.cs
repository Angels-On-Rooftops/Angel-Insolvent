using GameStateManagement;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeTrackOnCollide : MonoBehaviour
{
    [SerializeField]
    AudioSystem sys;

    [SerializeField]
    int SongIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (
            other.TryGetComponent(out CharacterController controller)
            && sys.CurrentIndex != SongIndex
        )
        {
            sys.SwitchToSong(SongIndex);
        }
    }
}
