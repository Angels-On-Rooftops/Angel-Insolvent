using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Collectables
{
    /// <summary>
    /// Class for a collectable item.
    /// Intended to be used as base class for derived classes,
    /// which are intended to be attached to the same object as AffectInventoryWhenCollect.
    /// </summary>
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private bool destroyOnCollect;

        //Will probably remove these eventually, but for now they demonstrate
        //how the co-routine can be used
        [SerializeField] float timeBeforeDestroyObj;
        [SerializeField] float speedFloatUpBeforeDestroy;

        public event Action<GameObject> OnCollection;

        protected IEnumerator CollectionRoutine(GameObject character)
        {
            if (OnCollection != null)
            {
                OnCollection(character);
            }

            if (this.destroyOnCollect)
            {
                //section that can be removed later
                float timeSinceCollision = 0;
                Vector3 positionWhenCollided = transform.position;
                while (timeSinceCollision <= this.timeBeforeDestroyObj)
                {
                    transform.position = new Vector3(positionWhenCollided.x,
                        positionWhenCollided.y + this.speedFloatUpBeforeDestroy *
        timeSinceCollision,
                                positionWhenCollided.z);
                    timeSinceCollision += Time.deltaTime;
                    yield return null;
                }
                //end section

                Destroy(gameObject);
            } 
        }
    }
}