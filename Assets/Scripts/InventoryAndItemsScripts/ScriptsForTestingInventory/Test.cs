using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Items;

namespace Inventory.Testing
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private ItemData buttonItemData;
        [SerializeField] private int buttonUpAmount = 3;
        [SerializeField] private int buttonDownAmount = 2;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void IncreaseButton()
        {
            PlayerInventory.Instance.Add(this.buttonItemData, this.buttonUpAmount);
        }

        public void DecreaseButton()
        {
            PlayerInventory.Instance.Remove(this.buttonItemData, this.buttonDownAmount);
        }

        void OnEnable()
        {
            PlayerInventory.Instance.OnInventoryUpdate += UpdateInventoryDisplay;
        }

        void OnDisable()
        {
            PlayerInventory.Instance.OnInventoryUpdate -= UpdateInventoryDisplay;
        }

        void UpdateInventoryDisplay()
        {
            this.text.text = PlayerInventory.Instance.PrintInventory();
        }
    }
}
