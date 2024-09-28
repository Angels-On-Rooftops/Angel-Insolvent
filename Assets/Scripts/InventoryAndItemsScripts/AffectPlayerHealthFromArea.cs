using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using System.Threading;

namespace Items.Collectables
{
    //Could be something other than health--this is more of a proof of concept
    [RequireComponent(typeof(AreaThatChecksInventory))]
    public class AffectPlayerHealthFromArea : MonoBehaviour
    {
        [SerializeField] int healthIncreaseWhenPass;
        [SerializeField] int healthDecreaseWhenFail;
        [SerializeField] PlayerManager player;

        private AreaThatChecksInventory area;
        private float elapsed = 3f;
        private bool wait = false;

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
            if (this.healthIncreaseWhenPass > 0 && !wait)
            {
                wait = true;
                player.GetComponent<PlayerManager>().IncreaseHealth(healthIncreaseWhenPass);
                Debug.Log("Yay! Health +" + this.healthIncreaseWhenPass);
            } else
            {
                elapsed -= Time.deltaTime;
            }
        }

        void DecreaseHealth(GameObject player)
        {
            if (this.healthDecreaseWhenFail > 0 && !wait)
            {
                wait = true;
                player.GetComponent<PlayerManager>().DecreaseHealth(healthDecreaseWhenFail);
                Debug.Log("Ow! Health -" + this.healthDecreaseWhenFail);
            } else
            {
                elapsed -= Time.deltaTime;
            }
        }

        private void Update()
        {
            if (elapsed <= 0)
            {
                elapsed = 3f;
                wait = false;
            }
        }
    }
}
