using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Items.Collectables
{
    //Could be something other than health--this is more of a proof of concept
    [RequireComponent(typeof(AreaThatChecksInventory))]
    public class AffectPlayerHealthFromArea : MonoBehaviour
    {
        [SerializeField] int healthIncreaseWhenPass;
        [SerializeField] int healthDecreaseWhenFail;

        private AreaThatChecksInventory area;

        private void Awake()
        {
            this.area = GetComponent<AreaThatChecksInventory>();
        }

        void OnEnable()
        {
            this.area.PassedInventoryCheck += IncreaseHealth;
            this.area.FailedInventoryCheck += DecreaseHealth;
        }

        void OnDisable()
        {
            this.area.PassedInventoryCheck -= IncreaseHealth;
            this.area.FailedInventoryCheck -= DecreaseHealth;
        }

        void IncreaseHealth(GameObject player)
        {
            if (this.healthIncreaseWhenPass > 0)
            {
                //Would replace with call to player and update player health
                Debug.Log("Yay! Health +" + this.healthIncreaseWhenPass);
            }
        }

        void DecreaseHealth(GameObject player)
        {
            if (this.healthDecreaseWhenFail > 0)
            {
                //Would replace with call to player and update player health
                Debug.Log("Ow! Health -" + this.healthDecreaseWhenFail);
            }
        }
    }
}
