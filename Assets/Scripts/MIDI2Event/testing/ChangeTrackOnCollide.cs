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
        Debug.Log("here");
        if (other.TryGetComponent(out CharacterController controller))
        {
            Debug.Log("here2");
            sys.SwitchToSong(SongIndex);
        }
    }
}
