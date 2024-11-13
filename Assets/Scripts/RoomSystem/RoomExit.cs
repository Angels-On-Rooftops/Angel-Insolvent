using Assets.Scripts.Libs;
using UnityEngine;

namespace Assets.Scripts.RoomSystem
{
    public class RoomExit : MonoBehaviour
    {
        [SerializeField]
        Room TransitionToScene;

        [SerializeField]
        InitialRoomPoint TransitionToPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (SpecialObjects.IsCharacter(other))
            {
                Debug.Log($"Exiting Room! Going to {TransitionToPoint} in {TransitionToScene}");
                RoomSystem.LoadRoom(TransitionToScene, TransitionToPoint);
            }
        }
    }
}
