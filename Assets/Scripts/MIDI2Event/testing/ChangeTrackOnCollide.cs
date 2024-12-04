using System.Collections;
using System.Collections.Generic;
using GameStateManagement;
using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeTrackOnCollide : MonoBehaviour
{
    [SerializeField]
    int SongIndex;

    private void OnTriggerEnter(Collider other)
    {
        //if (
        //    other.TryGetComponent(out CharacterController controller)
        //    && AudioSystem.CurrentIndex != SongIndex
        //)
        //{
        //    AudioSystem.SwitchToSong(SongIndex);
        //}
    }
}
