using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Items;

namespace Inventory.Testing
{
    public class Test : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private ItemData buttonItemData;

        [SerializeField]
        private int buttonUpAmount = 3;

        [SerializeField]
        private int buttonDownAmount = 2;

        [SerializeField]
        private GameObject player;

        [SerializeField]
        private Material redMaterial;

        [SerializeField]
        private Material greenMaterial;

        [SerializeField]
        private Material blueMaterial;

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }

        public void IncreaseButton()
        {
            PlayerInventory.Instance.Add(this.buttonItemData, this.buttonUpAmount);
        }

        public void DecreaseButton()
        {
            PlayerInventory.Instance.Remove(this.buttonItemData, this.buttonDownAmount);
        }

        public void ChangeEquipped()
        {
            PlayerInventory.Instance.EquipNext();
        }

        void OnEnable()
        {
            PlayerInventory.Instance.OnInventoryUpdate += UpdateInventoryDisplay;
            //PlayerInventory.Instance.OnEquippedChange += ChangePlayerMaterial;
        }

        void OnDisable()
        {
            PlayerInventory.Instance.OnInventoryUpdate -= UpdateInventoryDisplay;
            // PlayerInventory.Instance.OnEquippedChange -= ChangePlayerMaterial;
        }

        void UpdateInventoryDisplay()
        {
            this.text.text = PlayerInventory.Instance.PrintInventory();
        }

        /*
        void ChangePlayerMaterial()
        {
            string equipStr = PlayerInventory.Instance.CurrentlyEquippedItem.itemName;

            if (equipStr.Equals("Red"))
            {
                this.player.GetComponent<MeshRenderer>().material = this.redMaterial;
            }
            else if (equipStr.Equals("Green"))
            {
                this.player.GetComponent<MeshRenderer>().material = this.greenMaterial;
            }
            else if (equipStr.Equals("Blue"))
            {
                this.player.GetComponent<MeshRenderer>().material = this.blueMaterial;
            }
        }
        */
    }
}
