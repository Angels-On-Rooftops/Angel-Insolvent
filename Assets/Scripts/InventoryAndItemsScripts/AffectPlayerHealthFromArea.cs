using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Inventory;
using UnityEngine;

namespace Items.Collectables
{
    //Could be something other than health--this is more of a proof of concept
    [RequireComponent(typeof(AreaThatChecksInventory))]
    public class AffectPlayerHealthFromArea : MonoBehaviour
    {
        [SerializeField]
        int healthIncreaseWhenPass;

        [SerializeField]
        int healthDecreaseWhenFail;

        [SerializeField]
        PlayerHealth player;

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
                player.GetComponent<PlayerHealth>().IncreaseHealth(healthIncreaseWhenPass);
                Debug.Log("Yay! Health +" + this.healthIncreaseWhenPass);
            }
        }

        void DecreaseHealth(GameObject player)
        {
            if (this.healthDecreaseWhenFail > 0 && !wait)
            {
                wait = true;
                player.GetComponent<PlayerHealth>().DecreaseHealth(healthDecreaseWhenFail);
                Debug.Log("Ow! Health -" + this.healthDecreaseWhenFail);
            }
        }

        private void Update()
        {
            elapsed -= Time.deltaTime;
            if (elapsed <= 0)
            {
                elapsed = 3f;
                wait = false;
            }
        }
    }
}
